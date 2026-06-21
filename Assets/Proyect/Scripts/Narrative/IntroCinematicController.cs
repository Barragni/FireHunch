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
        if (NarrativeState.ReturningFromDeath)
        {
            NarrativeState.ReturningFromDeath = false;

            Camera skipCamera = player.GetPlayerCamera();

            skipCamera.transform.position =
                introCameraEnd.position;

            skipCamera.transform.rotation =
                introCameraEnd.rotation;

            player.SetMovementEnabled(true);

            yield break;
        }

        Camera playerCamera = player.GetPlayerCamera();

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
    }

    private IEnumerator PlayApproachPhase(Camera playerCamera)
    {
        float elapsed = 0f;

        while (elapsed < firstPhaseDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / firstPhaseDuration
            );

            playerCamera.transform.position =
                Vector3.Lerp(
                    introCameraStart.position,
                    introCameraMiddle.position,
                    t
                );

            Vector3 direction =
                lookTarget.position -
                playerCamera.transform.position;

            playerCamera.transform.rotation =
                Quaternion.LookRotation(
                    direction
                );

            yield return null;
        }

        playerCamera.transform.position =
            introCameraMiddle.position;
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