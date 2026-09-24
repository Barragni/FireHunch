using UnityEngine;

public class DeviceModeManager : MonoBehaviour
{
    public static DeviceModeManager Instance;

    private bool isMobile = false;

    public bool IsMobile => isMobile;

    private void Awake()
    {
        // Evita duplicados al cambiar de escena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Mantener este objeto entre escenas
        DontDestroyOnLoad(gameObject);

        // Por defecto siempre comienza como PC
        isMobile = false;
    }

    /// <summary>
    /// Cambia el dispositivo a modo móvil.
    /// Este método debe llamarse cuando se detecte
    /// una interacción táctil real.
    /// </summary>
    public void SetMobileMode()
    {
        isMobile = true;
    }
}