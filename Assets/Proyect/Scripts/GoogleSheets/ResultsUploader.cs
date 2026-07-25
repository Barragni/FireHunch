using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ResultsUploader : MonoBehaviour
{
    public static ResultsUploader Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Envía un intento a Google Sheets.
    /// </summary>
    public void Upload(AttemptData attempt)
    {
        if (attempt == null)
        {
            Debug.LogError("AttemptData es null.");
            return;
        }

        StartCoroutine(UploadCoroutine(attempt));
    }

    private IEnumerator UploadCoroutine(AttemptData attempt)
    {
        string json = JsonUtility.ToJson(attempt, true);

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(
                GoogleSheetsConfig.UploadUrl,
                UnityWebRequest.kHttpVerbPOST);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json");

        Debug.Log("========== ENVIANDO RESULTADOS ==========");
        Debug.Log(json);

        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER

        if (request.result == UnityWebRequest.Result.Success)

#else

        if (!request.isNetworkError && !request.isHttpError)

#endif
        {
            Debug.Log("=========================================");
            Debug.Log("Resultados enviados correctamente.");
            Debug.Log(request.downloadHandler.text);
            Debug.Log("=========================================");
        }
        else
        {
            Debug.LogError("=========================================");
            Debug.LogError("Error enviando resultados.");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
            Debug.LogError("=========================================");
        }
    }
}