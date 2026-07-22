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

    private void Start()
    {
        // Si el jugador vuelve desde la muerte,
        // reaparece justo en la decisión,
        // por lo que el humo debe estar al máximo.
        if (NarrativeState.ReturningFromDeath)
        {
            activated = true;
            maxProgress = 1f;

            if (overlay != null)
            {
                overlay.SetIntensity(1f);
            }
        }
    }

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

            if (overlay != null)
            {
                overlay.SetIntensity(maxProgress);
            }
        }
    }
}