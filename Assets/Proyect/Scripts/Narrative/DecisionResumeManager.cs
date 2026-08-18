using System.Collections;
using UnityEngine;

public class DecisionResumeManager : MonoBehaviour
{
    [SerializeField] private DecisionPlayer decisionPlayer;

    [SerializeField] private PlayerController player;

    private DecisionSequence savedDecision;

    private string savedNodeID;

    private void Start()
    {
        if (!NarrativeState.ReturningFromDeath)
        {
            return;
        }

        if (NarrativeState.PendingDecision == null)
        {
            Debug.LogWarning(
                "DecisionResumeManager: No existe PendingDecision."
            );

            NarrativeState.ReturningFromDeath = false;

            return;
        }

        savedDecision =
            NarrativeState.PendingDecision;

        savedNodeID =
            NarrativeState.ReturnNodeID;

        // Si por alguna razón no se guardó el nodeID,
        // utilizar el que ya conserva GameState.
        if (string.IsNullOrEmpty(savedNodeID))
        {
            if (GameState.Instance != null)
            {
                savedNodeID =
                    GameState.Instance.currentNode;
            }
        }

        NarrativeState.PendingDecision = null;

        StartCoroutine(
            RestoreDecisionState()
        );
    }

    private IEnumerator RestoreDecisionState()
    {
        // Esperar a que todos los objetos de la escena
        // terminen de inicializarse.
        yield return null;

        // ========================================================
        // RESTAURAR POSICIÓN DEL JUGADOR
        // ========================================================

        player.transform.position =
            NarrativeState.SavedPlayerPosition;

        player.transform.rotation =
            NarrativeState.SavedPlayerRotation;

        // ========================================================
        // BLOQUEAR JUGADOR
        // ========================================================

        player.SetMovementEnabled(false);

        player.SetHeadBobEnabled(false);

        // ========================================================
        // RECUPERAR LA CINEMÁTICA DE DECISIÓN ORIGINAL
        // ========================================================
        //
        // NO colocar directamente la cámara en SavedCameraPosition.
        //
        // PlayDecisionCamera() es el que hace:
        //
        // 1. Mostrar el modelo de Marcos.
        // 2. Colocar la cámara en decisionCameraStart.
        // 3. Moverla hacia decisionCameraEnd.
        // 4. Activar el overlay azul.
        // 5. Reducir Time.timeScale progresivamente.
        // 6. Dejar la escena congelada al llegar.
        //
        // Esto es exactamente el comportamiento original.
        // ========================================================

        yield return player.StartCoroutine(
            player.PlayDecisionCamera()
        );

        // ========================================================
        // MOSTRAR NUEVAMENTE LA MISMA DECISIÓN
        // ========================================================

        decisionPlayer.ShowDecision(
            savedDecision,
            savedNodeID
        );

        // ========================================================
        // YA NO ESTAMOS EN EL ESTADO DE RETORNO
        // ========================================================

        NarrativeState.ReturningFromDeath =
            false;

        NarrativeState.SkipDialogue =
            false;
    }
}