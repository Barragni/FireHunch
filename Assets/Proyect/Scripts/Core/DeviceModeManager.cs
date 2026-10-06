using UnityEngine;

public class DeviceModeManager : MonoBehaviour
{
    public static DeviceModeManager Instance;

    private bool isMobile = false;

    public bool IsMobile => isMobile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        isMobile = false;
    }

    public void SetMobileMode()
    {
        isMobile = true;
    }
}