using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Runtime.InteropServices;

public class ApiClient : MonoBehaviour
{
    const string k_baseUrl = "https://us-central1-arena-game-leaderboard.cloudfunctions.net/api";
    public static ApiClient Instance;

    [DllImport("__Internal")]
    private static extern void GetAppCheckTokenFromJS(string gameObjectName, string callbackMethod);

    private string appCheckToken;

    public void OnAppCheckToken(string token)
    {
        appCheckToken = token;
    }

    public void RequestToken()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        GetAppCheckTokenFromJS(gameObject.name, "OnAppCheckToken");
#else
        appCheckToken = "token";
#endif
    }

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    public IEnumerator PostPlayerScore(SubmitScoreData data, Action<string> callback, Action<string> onError)
    {
        string jsonOutput = "";
        using (StringWriter stringWriter = new())
        {
            JsonSerializer serializer = new();
            serializer.Serialize(stringWriter, data);
            jsonOutput = stringWriter.ToString();
        }

        RequestToken();
        yield return new WaitUntil(() => !string.IsNullOrEmpty(appCheckToken));
        UnityWebRequest postScoreRequest = GetBaseWebRequest("score", "POST", jsonOutput);
        postScoreRequest.SetRequestHeader("X-Firebase-AppCheck", appCheckToken);

        yield return postScoreRequest.SendWebRequest();

        if (postScoreRequest.result == UnityWebRequest.Result.Success)
        {
            string raw = postScoreRequest.downloadHandler.text;
            string docId = JsonConvert.DeserializeObject<string>(raw);
            callback(docId);
        }
        else
        {
            Debug.LogError($"Error: {postScoreRequest.error}");
            onError(postScoreRequest.error);
        }
    }

    public IEnumerator GetPlayerOnLeaderboard(string docId, Action<LeaderboardList> callback)
    {
        RequestToken();
        yield return new WaitUntil(() => !string.IsNullOrEmpty(appCheckToken));
        UnityWebRequest getPlayerLeaderboardPosition = GetBaseWebRequest($"leaderboard/{docId}?limit=5", "GET");
        getPlayerLeaderboardPosition.SetRequestHeader("X-Firebase-AppCheck", appCheckToken);

        yield return getPlayerLeaderboardPosition.SendWebRequest();

        if (getPlayerLeaderboardPosition.result == UnityWebRequest.Result.Success)
        {
            string raw = getPlayerLeaderboardPosition.downloadHandler.text;
            LeaderboardList response = ParseLeaderboardList(raw);
            callback(response);
        }
        else
        {
            Debug.LogError($"Error: {getPlayerLeaderboardPosition.error}");
        }
    }

    public IEnumerator GetLeaderboardTop(Action<LeaderboardList> callback)
    {
        RequestToken();
        yield return new WaitUntil(() => !string.IsNullOrEmpty(appCheckToken));
        UnityWebRequest getLeaderboard = GetBaseWebRequest("leaderboard?limit=5", "GET");
        getLeaderboard.SetRequestHeader("X-Firebase-AppCheck", appCheckToken);

        yield return getLeaderboard.SendWebRequest();

        if (getLeaderboard.result == UnityWebRequest.Result.Success)
        {
            string raw = getLeaderboard.downloadHandler.text;
            LeaderboardList response = ParseLeaderboardList(raw);
            callback(response);
        }
        else
        {
            Debug.LogError($"Error: {getLeaderboard.error}");
        }
    }

    private LeaderboardList ParseLeaderboardList(string raw)
    {
        LeaderboardList response = JsonConvert.DeserializeObject<LeaderboardList>(raw);
        return response;
    }

    private UnityWebRequest GetBaseWebRequest(string endpoint, string method, string jsonBody = "")
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        UploadHandler uploadHandler = new UploadHandlerRaw(bodyRaw)
        {
            contentType = "application/json"
        };

        DownloadHandler downloadHandler = new DownloadHandlerBuffer();

        string url = $"{k_baseUrl}/{endpoint}";
        UnityWebRequest request = new(url, method, downloadHandler, uploadHandler);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        return request;
    }
}
