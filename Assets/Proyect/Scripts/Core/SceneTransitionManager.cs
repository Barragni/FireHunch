using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.4f;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        // Mantener la pantalla completamente negra.
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        // Esperar dos frames para que la escena termine de dibujarse.
        yield return null;
        yield return null;

        Time.timeScale = 1f;

        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
            return;

        StartCoroutine(Transition(sceneName));
    }

    /// <summary>
    /// Reproduce un fade a negro y luego un fade desde negro
    /// sin cambiar de escena. Si se proporciona una acción,
    /// ésta se ejecuta mientras la pantalla está completamente negra.
    /// </summary>
    public IEnumerator PlayFadeOnly(Action onBlack = null)
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        // Fade a negro.
        yield return Fade(0f, 1f);

        // Ejecutar acción durante la pantalla negra.
        onBlack?.Invoke();

        // Esperar un frame para que los cambios se reflejen.
        yield return null;

        // Fade desde negro.
        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;

        yield return Fade(0f, 1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;

        Color c = fadeImage.color;
        c.a = from;
        fadeImage.color = c;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}