using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    private readonly List<GameObject> spawnedCompanions = new();

    private readonly List<GameObject> spawnedRescueCharacters = new();

    private readonly List<GameObject> spawnedRouteCharacters = new();

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

    // ============================================================
    // COMPANIONS NORMALES
    // ============================================================

    public void SpawnCompanions(
        List<CompanionData> companions
    )
    {
        ClearCompanions();

        if (
            companions == null ||
            companions.Count == 0
        )
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

        foreach (
            CompanionData companion
            in companions
        )
        {
            if (companion == null)
            {
                continue;
            }

            CompanionSpawnPoint spawnPoint = null;

            foreach (
                CompanionSpawnPoint point
                in spawnPoints
            )
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
                    $"No existe un CompanionSpawnPoint " +
                    $"para {companion.companionType}."
                );

                continue;
            }

            if (companion.prefab == null)
            {
                Debug.LogWarning(
                    $"El CompanionData de " +
                    $"{companion.companionType} " +
                    $"no tiene un prefab asignado."
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
                    $"El prefab de " +
                    $"{companion.companionType} " +
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
                        $"CompanionSpawnPoint " +
                        $"'{spawnPoint.name}' " +
                        $"no tiene asignado un Animator Controller " +
                        $"para el estado " +
                        $"'{spawnPoint.animationState}'."
                    );
                }
            }

            // ====================================================
            // FOLLOWER
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

    // ============================================================
    // PERSONAJE DE RESCATE
    // ============================================================

    public void SpawnRescueCharacter(
        RescueCharacterData rescueCharacter
    )
    {
        ClearRescueCharacter();

        if (rescueCharacter == null)
        {
            return;
        }

        if (rescueCharacter.prefab == null)
        {
            Debug.LogWarning(
                $"El RescueCharacterData de " +
                $"{rescueCharacter.characterType} " +
                $"no tiene un prefab asignado."
            );

            return;
        }

        RescueSpawnPoint[] spawnPoints =
            FindObjectsByType<RescueSpawnPoint>(
                FindObjectsSortMode.None
            );

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "No existe un RescueSpawnPoint en la escena."
            );

            return;
        }

        // Usamos el primer RescueSpawnPoint encontrado.
        RescueSpawnPoint spawnPoint =
            spawnPoints[0];

        // ========================================================
        // INSTANCIAR PERSONAJE DE RESCATE
        // ========================================================

        GameObject instance =
            Instantiate(
                rescueCharacter.prefab,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation
            );

        // ========================================================
        // CONFIGURAR ANIMATOR
        // ========================================================

        Animator animator =
            instance.GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning(
                $"El prefab de rescate de " +
                $"{rescueCharacter.characterType} " +
                $"no tiene un Animator."
            );
        }
        else
        {
            if (rescueCharacter.injuredController != null)
            {
                animator.runtimeAnimatorController =
                    rescueCharacter.injuredController;
            }
            else
            {
                Debug.LogWarning(
                    $"El RescueCharacterData de " +
                    $"{rescueCharacter.characterType} " +
                    $"no tiene asignado un Injured Controller."
                );
            }
        }

        // ========================================================
        // NO FOLLOWER
        // ========================================================
        //
        // El personaje de rescate es completamente estático.
        //

        spawnedRescueCharacters.Add(
            instance
        );
    }

    // ============================================================
    // PERSONAJE CONDICIONADO POR RUTA
    // ============================================================

    public void SpawnRouteCharacter(
        RouteCharacterData routeCharacter
    )
    {
        ClearRouteCharacter();

        // Si la ruta no tiene personaje asignado,
        // simplemente no aparece ninguno.
        if (routeCharacter == null)
        {
            return;
        }

        if (routeCharacter.prefab == null)
        {
            Debug.LogWarning(
                "El RouteCharacterData no tiene un prefab asignado."
            );

            return;
        }

        RouteCharacterSpawnPoint[] spawnPoints =
            FindObjectsByType<RouteCharacterSpawnPoint>(
                FindObjectsSortMode.None
            );

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "No existe un RouteCharacterSpawnPoint en la escena."
            );

            return;
        }

        // Usamos el primer RouteCharacterSpawnPoint encontrado.
        RouteCharacterSpawnPoint spawnPoint =
            spawnPoints[0];

        // ========================================================
        // INSTANCIAR PERSONAJE
        // ========================================================

        GameObject instance =
            Instantiate(
                routeCharacter.prefab,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation
            );

        // ========================================================
        // NO FOLLOWER
        // ========================================================
        //
        // Este personaje solamente aparece en determinadas rutas.
        // No se configura CompanionFollower.
        //

        spawnedRouteCharacters.Add(
            instance
        );
    }

    // ============================================================
    // LIMPIAR COMPANIONS
    // ============================================================

    public void ClearCompanions()
    {
        foreach (
            GameObject companion
            in spawnedCompanions
        )
        {
            if (companion != null)
            {
                Destroy(companion);
            }
        }

        spawnedCompanions.Clear();
    }

    // ============================================================
    // LIMPIAR PERSONAJE DE RESCATE
    // ============================================================

    public void ClearRescueCharacter()
    {
        foreach (
            GameObject character
            in spawnedRescueCharacters
        )
        {
            if (character != null)
            {
                Destroy(character);
            }
        }

        spawnedRescueCharacters.Clear();
    }

    // ============================================================
    // LIMPIAR PERSONAJE CONDICIONADO POR RUTA
    // ============================================================

    public void ClearRouteCharacter()
    {
        foreach (
            GameObject character
            in spawnedRouteCharacters
        )
        {
            if (character != null)
            {
                Destroy(character);
            }
        }

        spawnedRouteCharacters.Clear();
    }
}