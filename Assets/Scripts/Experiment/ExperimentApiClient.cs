using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ExperimentApiClient : MonoBehaviour {
    [SerializeField] private string baseUrl = "http://localhost:8000";
    [SerializeField] private string unityAuthToken = "your_unity_auth_token";

    public IEnumerator FetchAndStartExperiment(
        SessionData session,
        Action<PracticeDataDTO> onSuccess,
        Action<string> onError
    ) {
        string url = $"{baseUrl}/unity/initialize-session?practiceId={session.PracticeId}&userId={session.UserId}&courseId={session.CourseId}";
        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SetRequestHeader("x-unity-token", unityAuthToken);

        Debug.Log($"Sending request to API... ({url})");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            onError?.Invoke($"Error loading experiment from API: {webRequest.error}");
            yield break;
        }

        PracticeDataDTO practiceData = ParsePracticeResponse(webRequest.downloadHandler.text);
        if (practiceData == null) {
            onError?.Invoke("API returned an error or empty data.");
            yield break;
        }

        onSuccess?.Invoke(practiceData);
    }

    public IEnumerator SendDataToServer(SessionData session, int score, double timeTakenSeconds, Action onSuccess, Action<string> onError) {
        string url = $"{baseUrl}/unity/results";
        var resultData = new {
            practiceId = session.PracticeId,
            userId = session.UserId,
            courseId = session.CourseId,
            score,
            timeTaken = timeTakenSeconds
        };

        string jsonData = JsonConvert.SerializeObject(resultData);
        using UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        webRequest.SetRequestHeader("x-unity-token", unityAuthToken);
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"Error sending result data: {webRequest.error}");
            onError?.Invoke($"Error sending result data: {webRequest.error}");
        }
    }

    private static PracticeDataDTO ParsePracticeResponse(string json) {
        try {
            var data = JsonConvert.DeserializeObject<APIResult<PracticeDataDTO>>(json);
            if (data != null && data.Ok() && data.Data != null) {
                Debug.Log("Experiment data parsed successfully.");
                return data.Data;
            }
        }
        catch (Exception e) {
            Debug.LogError($"Failed to parse experiment data: {e.Message}");
            Debug.LogError($"Response content: {json}");
        }

        return null;
    }
}
