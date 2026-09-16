using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ResultsUploader : MonoBehaviour
{
    public static ResultsUploader Instance;

    [Header("Backend MongoDB")]
    [SerializeField]
    private string backendUrl =
        "https://firehunchbackend.onrender.com/api/attempts";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Despierta Render al iniciar la aplicación.
        StartCoroutine(WakeBackend());
    }

    // =========================================================
    // DESPERTAR BACKEND
    // =========================================================

    private IEnumerator WakeBackend()
    {
        using UnityWebRequest request =
            UnityWebRequest.Get(
                "https://firehunchbackend.onrender.com/api/health");

        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER

        if (request.result == UnityWebRequest.Result.Success)

#else

        if (!request.isNetworkError && !request.isHttpError)

#endif
        {
            Debug.Log("BACKEND: Render está activo.");
        }
        else
        {
            Debug.LogWarning(
                "BACKEND: No se pudo despertar Render. " +
                request.error);
        }
    }

    /// <summary>
    /// Envía un intento tanto a Google Sheets como al backend de MongoDB.
    /// </summary>
    public void Upload(AttemptData attempt)
    {
        if (attempt == null)
        {
            Debug.LogError("AttemptData es null.");
            return;
        }

        string json = JsonUtility.ToJson(attempt, true);

        Debug.Log("========== ENVIANDO RESULTADOS ==========");
        Debug.Log(json);
        Debug.Log("=========================================");

        // Cada envío es independiente.
        StartCoroutine(UploadGoogleSheets(json));
        StartCoroutine(UploadMongoDB(json));
    }

    // =========================================================
    // GOOGLE SHEETS
    // =========================================================

    private IEnumerator UploadGoogleSheets(string json)
    {
        byte[] body =
            System.Text.Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(
                GoogleSheetsConfig.UploadUrl,
                UnityWebRequest.kHttpVerbPOST);

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER

        if (request.result == UnityWebRequest.Result.Success)

#else

        if (!request.isNetworkError && !request.isHttpError)

#endif
        {
            Debug.Log("=========================================");
            Debug.Log("GOOGLE SHEETS: enviado correctamente.");
            Debug.Log(request.downloadHandler.text);
            Debug.Log("=========================================");
        }
        else
        {
            Debug.LogError("=========================================");
            Debug.LogError("GOOGLE SHEETS: error enviando resultados.");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
            Debug.LogError("=========================================");
        }
    }

    // =========================================================
    // MONGODB / BACKEND
    // =========================================================

    private IEnumerator UploadMongoDB(string json)
    {
        byte[] body =
            System.Text.Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(
                backendUrl,
                UnityWebRequest.kHttpVerbPOST);

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER

        if (request.result == UnityWebRequest.Result.Success)

#else

        if (!request.isNetworkError && !request.isHttpError)

#endif
        {
            Debug.Log("=========================================");
            Debug.Log("MONGODB: enviado correctamente.");
            Debug.Log(request.downloadHandler.text);
            Debug.Log("=========================================");
        }
        else
        {
            Debug.LogError("=========================================");
            Debug.LogError("MONGODB: error enviando resultados.");
            Debug.Log(request.downloadHandler.text);
            Debug.LogError(request.error);
            Debug.Log("=========================================");
        }
    }
}