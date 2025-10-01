using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private LeaderboardRow rowPrefab;
    [SerializeField] private GameObject fillerPrefab;

    [Header("UI")]
    [SerializeField] private GameObject content;

    private string playerId;
    private SortedList<int, LeaderboardResponse> rows = new();

    private void Awake()
    {
        for (int i = 1; i < 11; i++)
        {
            rows.Add(i, null);
        }
    }

    private void OnDisable()
    {
        ResetLeaderboard();
    }

    public void SetPlayerId(string docId)
    {
        playerId = docId;
    }

    private void ResetLeaderboard()
    {
        foreach (Transform row in content.transform)
        {
            Destroy(row.gameObject);
        }
        rows.Clear();
    }

    private LeaderboardRow FillRowData(LeaderboardResponse rowData)
    {
        // if (previousPosition + 1 != rowData.Key)
        // {
        //     GameObject filler = Instantiate(fillerPrefab, content.transform);
        // }

        LeaderboardRow newRow = Instantiate(rowPrefab, content.transform);
        // newRow.transform.SetSiblingIndex(rowData.Key);
        string recordName = rowData.data.name;
        if (playerId == rowData.id)
        {
            // TODO: localize
            recordName += " (You)";
        }
        newRow.SetData(rowData.position, recordName, rowData.data.score);
        return newRow;
    }

    public void SetTopScores(LeaderboardList topScoreList)
    {
        for (int i = topScoreList.scores.Length - 1; i >= 0; i--)
        {
            LeaderboardResponse rowData = topScoreList.scores[i];
            AddLeaderboardRow(rowData);
            LeaderboardRow newRow = FillRowData(rowData);
            newRow.transform.SetAsFirstSibling();
        }
    }

    public void SetProximatePlayerScores(LeaderboardList scoreList)
    {
        if (scoreList.scores.Length > 0)
        {
            LeaderboardResponse firstScore = scoreList.scores.First();
            if (firstScore.position > 6)
            {
                GameObject filler = Instantiate(fillerPrefab, content.transform);
                filler.transform.SetAsLastSibling();
            }
        }

        for (int i = 0; i < scoreList.scores.Length; i++)
        {
            LeaderboardResponse rowData = scoreList.scores[i];
            if (rowData.position <= 5)
            {
                continue;
            }

            AddLeaderboardRow(rowData);
            LeaderboardRow newRow = FillRowData(rowData);
            newRow.transform.SetAsLastSibling();
        }
    }

    public void AddLeaderboardRow(LeaderboardResponse rowData)
    {
        if (!rows.ContainsKey(rowData.position))
        {
            rows.Add(rowData.position, rowData);
        }
    }
}
