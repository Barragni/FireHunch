using UnityEngine;

public class MobileOnlyUI : MonoBehaviour
{
    private void Awake()
    {
        bool isMobileDevice =
            !Application.isEditor &&
            Input.touchSupported;

        gameObject.SetActive(isMobileDevice);
    }
}