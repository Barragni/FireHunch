using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    private float radius;

    private void Awake()
    {
        if (background == null || handle == null)
            return;

        radius = background.rect.width / 2f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveHandle(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveHandle(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
    }

    private void MoveHandle(PointerEventData eventData)
    {
        if (handle == null || background == null)
            return;

        Vector2 localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out localPosition
        );

        float maxDistance = radius - (handle.rect.width / 2f);

        localPosition = Vector2.ClampMagnitude(
            localPosition,
            maxDistance
        );

        handle.anchoredPosition = localPosition;
    }

    public Vector2 GetInput()
    {
        if (handle == null || radius <= 0f)
            return Vector2.zero;

        return handle.anchoredPosition / radius;
    }
}