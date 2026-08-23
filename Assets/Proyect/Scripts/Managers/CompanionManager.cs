using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    private readonly List<GameObject> spawnedCompanions = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnCompanions(List<CompanionData> companions)
    {
        ClearCompanions();

        if (companions == null || companions.Count == 0)
        {
            return;
        }

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "CompanionManager: No se encontró el Player."
            );

            return;
        }

        CompanionSpawnPoint[] spawnPoints =
            FindObjectsByType<CompanionSpawnPoint>(
                FindObjectsSortMode.None
            );

        foreach (CompanionData companion in companions)
        {
            if (companion == null)
            {
                continue;
            }

            CompanionSpawnPoint spawnPoint = null;

            foreach (CompanionSpawnPoint point in spawnPoints)
            {
                if (
                    point.companionType ==
                    companion.companionType
                )
                {
                    spawnPoint = point;
                    break;
                }
            }

            if (spawnPoint == null)
            {
                Debug.LogWarning(
                    $"No existe un CompanionSpawnPoint para {companion.companionType}."
                );

                continue;
            }

            if (companion.prefab == null)
            {
                Debug.LogWarning(
                    $"El CompanionData de {companion.companionType} no tiene un prefab asignado."
                );

                continue;
            }

            // ====================================================
            // INSTANCIAR COMPAÑERO
            // ====================================================

            GameObject instance =
                Instantiate(
                    companion.prefab,
                    spawnPoint.transform.position,
                    spawnPoint.transform.rotation
                );

            // ====================================================
            // CONFIGURAR ANIMATOR
            // ====================================================

            Animator animator =
                instance.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogWarning(
                    $"El prefab de {companion.companionType} " +
                    $"no tiene un Animator."
                );
            }
            else
            {
                RuntimeAnimatorController selectedController =
                    spawnPoint.GetSelectedController();

                if (selectedController != null)
                {
                    animator.runtimeAnimatorController =
                        selectedController;
                }
                else
                {
                    Debug.LogWarning(
                        $"CompanionSpawnPoint '{spawnPoint.name}' " +
                        $"no tiene asignado un Animator Controller " +
                        $"para el estado '{spawnPoint.animationState}'. " +
                        $"Se conservará el Controller original del prefab."
                    );
                }
            }

            // ====================================================
            // CONFIGURAR FOLLOWER
            // ====================================================

            CompanionFollower follower =
                instance.GetComponent<CompanionFollower>();

            if (follower != null)
            {
                follower.SetTarget(
                    player.transform
                );
            }

            spawnedCompanions.Add(
                instance
            );
        }
    }

    public void ClearCompanions()
    {
        foreach (GameObject companion in spawnedCompanions)
        {
            if (companion != null)
            {
                Destroy(companion);
            }
        }

        spawnedCompanions.Clear();
    }
}