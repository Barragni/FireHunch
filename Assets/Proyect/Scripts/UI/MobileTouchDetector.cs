using UnityEngine;
using UnityEngine.EventSystems;

public class MobileTouchDetector : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // Solo activa modo móvil si realmente hubo un toque.
        // Un clic de mouse en PC no activa el modo móvil.
        if (Input.touchCount > 0)
        {
            if (DeviceModeManager.Instance != null)
            {
                DeviceModeManager.Instance.SetMobileMode();
            }
        }
    }
}