using System.Runtime.InteropServices;
using UnityEngine;

public class FirebaseAppCheckHandler : MonoBehaviour
{
    public string LatestToken { get; private set; }

    [DllImport("__Internal")]
    private static extern string GetAppCheckToken();

    public static string AddAuthentication()
    {
        return GetAppCheckToken();
    }
}