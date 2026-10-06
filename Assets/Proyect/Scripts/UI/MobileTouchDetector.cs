using UnityEngine;
using UnityEngine.EventSystems;

public class MobileTouchDetector : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.touchCount > 0)
        {
            if (DeviceModeManager.Instance != null)
            {
                DeviceModeManager.Instance.SetMobileMode();
            }
        }
    }
}