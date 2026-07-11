using UnityEngine;

[System.Serializable]
public class FocusPoint
{
    [Header("Objeto a observar")]
    public Transform target;

    [Header("Tiempo mirando este objeto")]
    [Min(0f)]
    public float focusDuration = 2f;
}