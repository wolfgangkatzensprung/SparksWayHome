using UnityEngine;

[RequireComponent(typeof(Light))]
public class ActivatableLightSource : Activatable
{
    [SerializeField]
    private float targetRange;
    private Light lightSource;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    void FixedUpdate()
    {
        lightSource.range = Mathf.Lerp(lightSource.range, targetRange, 0.1f);
    }
}