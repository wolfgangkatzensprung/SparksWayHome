using Cinemachine;
using GlobalEvents;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    [SerializeField] float maxAmplitude = 1f;
    [SerializeField] float maxFrequency = 2f;
    [SerializeField] float defaultShakeTime = 0.2f;

    // Distance based Screenshake
    [SerializeField] float maxDistance = 25f;

    float _strength01;
    float strength01
    {
        get => _strength01;
        set => _strength01 = Mathf.Clamp01(value);
    }

    private float shakeTime;
    private float timer;
    CinemachineBasicMultiChannelPerlin channelPerlin;

    private void OnEnable()
    {
        GameEvents.Connect<ScreenShakeEventArgs>(Events.ScreenShake, AddShakeStrength);

    }
    private void OnDisable()
    {
        StopShake();
        GameEvents.Disconnect<ScreenShakeEventArgs>(Events.ScreenShake, AddShakeStrength);
    }

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }
    private void Start()
    {
        channelPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (PlayerPrefsHandler.TryGetBool("ScreenShakeEnabled", out bool state))
        {
            //Debug.Log($"ScreenShakeEnabled is {state}");
            enabled = state;
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            strength01 = Mathf.Lerp(1f, 0f, 1 - (timer / shakeTime));
            Shake();
        }
        else
        {
            StopShake();
            timer = 0f;
        }
    }

    public void AddShakeStrength(ScreenShakeEventArgs args) => AddShakeStrength(args.position, args.strength, args.duration);
    public void AddShakeStrength(float strengthDelta)
    {
        timer = defaultShakeTime;
        strength01 += strengthDelta;
    }
    public void AddShakeStrength(Vector3 pos, float maxStrengthDelta) => AddShakeStrength(pos, maxStrengthDelta, defaultShakeTime);
    public void AddShakeStrength(Vector3 pos, float maxStrengthDelta, float shakeTime)
    {
        float distance = Vector3.Distance(transform.position, pos);
        float normalizedDistance = Mathf.Clamp01(1f - (distance / maxDistance));

        float strengthDelta = maxStrengthDelta * normalizedDistance;
        //Debug.Log($"Shake by distance: {distance} from position: {pos} results in strengthDelta {strengthDelta}");

        this.shakeTime = shakeTime;
        timer = shakeTime;
        strength01 += strengthDelta;
    }

    void Shake()
    {
        var delta = 1 - (timer / shakeTime);
        //Debug.Log($"Shake Lerp Delta: {delta}");
        channelPerlin.m_AmplitudeGain = Mathf.Lerp(maxAmplitude * strength01, 0f, delta);
        channelPerlin.m_FrequencyGain = Mathf.Lerp(maxFrequency * strength01, 1f, delta);
    }
    void StopShake()
    {
        channelPerlin.m_AmplitudeGain = 0f;
    }
}
