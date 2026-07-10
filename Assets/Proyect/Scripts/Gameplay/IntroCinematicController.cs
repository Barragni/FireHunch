using System.Collections;
using UnityEngine;

public class IntroCinematicController : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private PlayerController player;

    [Header("Puntos de cámara")]
    [SerializeField] private Transform introCameraStart;

    [SerializeField] private Transform introCameraMiddle;

    [SerializeField] private Transform introCameraEnd;

    [Header("Objetivo a observar")]
    [SerializeField] private Transform lookTarget;

    [Header("Duración fase 1")]
    [SerializeField] private float firstPhaseDuration = 3f;

    [Header("Duración fase 2")]
    [SerializeField] private float secondPhaseDuration = 3f;

    private IEnumerator Start()
    {
        // Desactivar Head Bob durante la cinemática
        player.SetHeadBobEnabled(false);

        if (NarrativeState.ReturningFromDeath)
        {
            NarrativeState.ReturningFromDeath = false;

            Camera skipCamera =
                player.GetPlayerCamera();

            skipCamera.transform.position =
                introCameraEnd.position;

            skipCamera.transform.rotation =
                introCameraEnd.rotation;

            player.SetMovementEnabled(true);
            player.SetHeadBobEnabled(true);

            yield break;
        }

        Camera playerCamera =
            player.GetPlayerCamera();

        playerCamera.transform.position =
            introCameraStart.position;

        playerCamera.transform.rotation =
            introCameraStart.rotation;

        yield return StartCoroutine(
            PlayApproachPhase(playerCamera)
        );

        yield return StartCoroutine(
            PlayPOVTransition(playerCamera)
        );

        player.SetMovementEnabled(true);
        player.SetHeadBobEnabled(true);
    }

    private IEnumerator PlayApproachPhase(Camera playerCamera)
    {
        Vector3 startPosition =
            introCameraStart.position;

        Vector3 endPosition =
            introCameraMiddle.position;

        Quaternion fixedRotation =
            introCameraStart.rotation;

        float elapsed = 0f;

        while (elapsed < firstPhaseDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / firstPhaseDuration
            );

            t = 1f - Mathf.Pow(1f - t, 1.5f);

            playerCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            playerCamera.transform.rotation =
                fixedRotation;

            yield return null;
        }

        playerCamera.transform.position =
            endPosition;

        playerCamera.transform.rotation =
            fixedRotation;
    }

    private IEnumerator PlayPOVTransition(Camera playerCamera)
    {
        Vector3 startPosition =
            introCameraMiddle.position;

        Vector3 endPosition =
            introCameraEnd.position;

        Quaternion startRotation =
            playerCamera.transform.rotation;

        Quaternion endRotation =
            introCameraEnd.rotation;

        float elapsed = 0f;

        while (elapsed < secondPhaseDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / secondPhaseDuration
            );

            playerCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            playerCamera.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    t
                );

            yield return null;
        }

        playerCamera.transform.position =
            endPosition;

        playerCamera.transform.rotation =
            endRotation;
    }
}