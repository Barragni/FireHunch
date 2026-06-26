using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SmokeDangerZone : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private PlayerController player;

    [Header("Overlay")]
    [SerializeField] private DamageOverlayUI overlay;

    [Header("Fin de la zona")]
    [SerializeField] private Transform endPoint;

    private bool activated;
    private float maxProgress;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        activated = true;
    }

    private void Update()
    {
        if (!activated)
            return;

        Vector3 start = transform.position;
        Vector3 end = endPoint.position;
        Vector3 current = player.transform.position;

        float totalDistance =
            Vector3.Distance(start, end);

        float travelled =
            Vector3.Distance(start, current);

        float progress =
            Mathf.Clamp01(travelled / totalDistance);

        if (progress > maxProgress)
        {
            maxProgress = progress;
            overlay.SetIntensity(maxProgress);
        }
    }
}