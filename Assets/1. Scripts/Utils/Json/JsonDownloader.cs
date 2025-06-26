using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class JsonDownloader
{
    public static async Task<string> DownloadJson(string url)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        var op = www.SendWebRequest();

        while (!op.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[JsonDownloader] 오류: {www.error}");
            return null;
        }

        return www.downloadHandler.text;
    }
}