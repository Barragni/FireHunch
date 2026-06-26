using UnityEngine;
using UnityEngine.UI;

public class DamageOverlayUI : MonoBehaviour
{
    [SerializeField] private Image overlayImage;

    [SerializeField]
    [Range(0f, 1f)]
    private float currentIntensity;

    public void SetIntensity(float value)
    {
        currentIntensity = Mathf.Clamp01(value);

        Color color = overlayImage.color;
        color.a = currentIntensity;

        overlayImage.color = color;
    }

    public float CurrentIntensity => currentIntensity;
}