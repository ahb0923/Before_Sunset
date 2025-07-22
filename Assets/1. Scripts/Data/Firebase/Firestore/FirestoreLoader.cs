using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FirestoreLoader : MonoBehaviour
{
    private const string FIREBASE_PROJECT_ID = "before-sunset-58e9c";
    private const string COLLECTION = "MineralData";
    private const string DOCUMENT = "구리";
    
    private string GetFirestoreUrl()
    {
        return $"https://firestore.googleapis.com/v1/projects/{FIREBASE_PROJECT_ID}/databases/(default)/documents/{COLLECTION}/{DOCUMENT}";
    }

    private void Awake()
    {
       // LoadData();
    }

    public void LoadData()
    {
        StartCoroutine(GetDocument());
    }

    private IEnumerator GetDocument()
    {
        string url = GetFirestoreUrl();
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Firebase에 보안 규칙이 있으면, Bearer 토큰 필요
        // request.SetRequestHeader("Authorization", "Bearer [YOUR_ID_TOKEN]");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Firestore Data: " + request.downloadHandler.text);
        }
    }
}
