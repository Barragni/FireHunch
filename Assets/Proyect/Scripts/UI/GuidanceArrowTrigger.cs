using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidanceArrowTrigger : MonoBehaviour
{
    [Header("Flechas")]
    [Tooltip("Arrastra aquí todas las flechas 3D que quieres controlar.")]
    [SerializeField] private GameObject[] arrows;

    [Header("Apariencia")]
    [SerializeField] private string arrowColorHex = "#0FD900";

    [Range(0f, 1f)]
    [SerializeField] private float visibleAlpha = 0.55f;

    [SerializeField] private float fadeDuration = 0.25f;


    [Header("Movimiento")]
    [SerializeField] private bool enableOscillation = true;

    [Tooltip("Eje sobre el que se moverán las flechas.")]
    [SerializeField] private MovementAxis movementAxis = MovementAxis.X;

    [Tooltip("Distancia que se mueve la flecha hacia delante y atrás.")]
    [SerializeField] private float oscillationDistance = 0.15f;

    [Tooltip("Velocidad del movimiento.")]
    [SerializeField] private float oscillationSpeed = 2f;


    private Color arrowColor;

    private enum MovementAxis
    {
        X,
        Y,
        Z
    }


    private class ArrowData
    {
        public GameObject gameObject;
        public Renderer[] renderers;
        public Material[] materials;
        public Vector3 initialPosition;
        public Coroutine fadeCoroutine;
    }


    private readonly List<ArrowData> arrowData =
        new List<ArrowData>();


    private void Awake()
    {
        ColorUtility.TryParseHtmlString(
            arrowColorHex,
            out arrowColor
        );

        if (arrows == null || arrows.Length == 0)
        {
            Debug.LogWarning(
                $"GuidanceArrowTrigger en '{gameObject.name}' " +
                "no tiene ninguna flecha asignada."
            );

            return;
        }


        foreach (GameObject arrow in arrows)
        {
            if (arrow == null)
                continue;

            SetupArrow(arrow);
        }


        Collider trigger = GetComponent<Collider>();

        if (trigger != null)
        {
            trigger.isTrigger = true;
        }
        else
        {
            Debug.LogError(
                $"GuidanceArrowTrigger en '{gameObject.name}' " +
                "necesita un Collider."
            );
        }
    }


    private void SetupArrow(GameObject arrow)
    {
        ArrowData data = new ArrowData();

        data.gameObject = arrow;

        data.initialPosition =
            arrow.transform.position;


        data.renderers =
            arrow.GetComponentsInChildren<Renderer>(true);


        List<Material> materialList =
            new List<Material>();


        foreach (Renderer renderer in data.renderers)
        {
            if (renderer == null)
                continue;


            foreach (Material material in renderer.materials)
            {
                if (material != null &&
                    !materialList.Contains(material))
                {
                    materialList.Add(material);
                }
            }
        }


        data.materials =
            materialList.ToArray();


        ConfigureMaterials(data);

        SetAlpha(data, 0f);

        arrowData.Add(data);
    }


    private void Update()
    {
        if (!enableOscillation)
            return;


        foreach (ArrowData arrow in arrowData)
        {
            if (arrow == null ||
                arrow.gameObject == null)
            {
                continue;
            }


            Transform arrowTransform =
                arrow.gameObject.transform;


            float movement =
                Mathf.Sin(
                    Time.time * oscillationSpeed
                ) * oscillationDistance;


            Vector3 direction =
                GetMovementDirection(arrowTransform);


            arrowTransform.position =
                arrow.initialPosition +
                direction * movement;
        }
    }


    private Vector3 GetMovementDirection(
        Transform arrowTransform)
    {
        switch (movementAxis)
        {
            case MovementAxis.X:
                return arrowTransform.right;

            case MovementAxis.Y:
                return arrowTransform.up;

            case MovementAxis.Z:
                return arrowTransform.forward;
        }


        return arrowTransform.right;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;


        foreach (ArrowData arrow in arrowData)
        {
            StartFade(
                arrow,
                visibleAlpha
            );
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;


        foreach (ArrowData arrow in arrowData)
        {
            StartFade(
                arrow,
                0f
            );
        }
    }


    private void StartFade(
        ArrowData arrow,
        float targetAlpha)
    {
        if (arrow.fadeCoroutine != null)
        {
            StopCoroutine(
                arrow.fadeCoroutine
            );
        }


        arrow.fadeCoroutine =
            StartCoroutine(
                FadeRoutine(
                    arrow,
                    targetAlpha
                )
            );
    }


    private IEnumerator FadeRoutine(
        ArrowData arrow,
        float targetAlpha)
    {
        float startAlpha =
            GetCurrentAlpha(arrow);

        float elapsed = 0f;


        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );


            float alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );


            SetAlpha(
                arrow,
                alpha
            );


            yield return null;
        }


        SetAlpha(
            arrow,
            targetAlpha
        );


        arrow.fadeCoroutine = null;
    }


    private void ConfigureMaterials(
        ArrowData arrow)
    {
        foreach (Material material in arrow.materials)
        {
            if (material == null)
                continue;


            Color color =
                arrowColor;

            color.a = 1f;


            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor(
                    "_BaseColor",
                    color
                );
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor(
                    "_Color",
                    color
                );
            }


            SetMaterialTransparent(material);
        }
    }


    private void SetMaterialTransparent(
        Material material)
    {
        if (material.HasProperty("_Surface"))
        {
            material.SetFloat(
                "_Surface",
                1f
            );
        }


        if (material.HasProperty("_Blend"))
        {
            material.SetFloat(
                "_Blend",
                0f
            );
        }


        if (material.HasProperty("_AlphaClip"))
        {
            material.SetFloat(
                "_AlphaClip",
                0f
            );
        }


        if (material.HasProperty("_Mode"))
        {
            material.SetFloat(
                "_Mode",
                3f
            );
        }


        material.SetOverrideTag(
            "RenderType",
            "Transparent"
        );


        material.SetInt(
            "_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha
        );


        material.SetInt(
            "_DstBlend",
            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
        );


        material.SetInt(
            "_ZWrite",
            0
        );


        material.DisableKeyword(
            "_ALPHATEST_ON"
        );


        material.EnableKeyword(
            "_ALPHABLEND_ON"
        );


        material.DisableKeyword(
            "_ALPHAPREMULTIPLY_ON"
        );


        material.renderQueue =
            (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }


    private void SetAlpha(
        ArrowData arrow,
        float alpha)
    {
        foreach (Material material in arrow.materials)
        {
            if (material == null)
                continue;


            Color color =
                arrowColor;

            color.a = alpha;


            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor(
                    "_BaseColor",
                    color
                );
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor(
                    "_Color",
                    color
                );
            }
        }
    }


    private float GetCurrentAlpha(
        ArrowData arrow)
    {
        if (arrow.materials == null ||
            arrow.materials.Length == 0)
        {
            return 0f;
        }


        Material material =
            arrow.materials[0];


        if (material.HasProperty("_BaseColor"))
        {
            return material
                .GetColor("_BaseColor")
                .a;
        }


        if (material.HasProperty("_Color"))
        {
            return material
                .GetColor("_Color")
                .a;
        }


        return 0f;
    }
}