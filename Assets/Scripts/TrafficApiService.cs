using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TrafficApiService : MonoBehaviour
{
    [SerializeField] private string currentFile = "traffic_status.json";

    [SerializeField]
    private List<string> jsonFiles = new List<string>
    {
        "traffic_status1.json","traffic_status2.json","traffic_status3.json","traffic_status4.json"
    };

    public void FetchTrafficStatus(Action<TrafficResponse> onSuccess, Action<string> onError)
    {
        currentFile = jsonFiles[UnityEngine.Random.Range(0, jsonFiles.Count)];
        StartCoroutine(ReadLocalJson(onSuccess, onError));
    }

    private IEnumerator ReadLocalJson(Action<TrafficResponse> onSuccess, Action<string> onError)
    {
        string path = "file://" + Path.Combine(Application.streamingAssetsPath, currentFile);

        using UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke($"Erro ao ler arquivo: {request.error}");
            yield break;
        }

        try
        {
            TrafficResponse response = JsonUtility.FromJson<TrafficResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        catch (Exception e)
        {
            onError?.Invoke($"Erro ao deserializar JSON: {e.Message}");
        }
    }
}