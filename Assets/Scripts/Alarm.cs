using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementDetector))]
public class Alarm : MonoBehaviour
{
    private const float AlarmIntensityMin = 0f;
    private const float AlarmIntensityMax = 1f;

    [SerializeField] private Light _alarmLight;
    [SerializeField] private AudioSource _alarmAudio;
    [SerializeField, Min(0f)] private float _alarmLightIntensity = 1f;
    [SerializeField, Min(0f)] private float _intensityChangeSpeed = 0.3f;
    [SerializeField, Min(0f)] private float _alarmLightFlashFrequency = 4f;

    private float _alarmIntensity;
    private Coroutine _alarmCoroutine;
    private Coroutine _alarmLightIntensityCoroutine;
    private MovementDetector _movementDetector;

    private void Awake()
    {
        _movementDetector = GetComponent<MovementDetector>();
    }

    private void OnEnable()
    {
        _movementDetector.IntruderDetected += OnIntruderDetected;
        _movementDetector.IntruderDisappeared += OnIntruderDisappeared;
    }

    private void OnDisable()
    {
        _movementDetector.IntruderDetected -= OnIntruderDetected;
        _movementDetector.IntruderDisappeared -= OnIntruderDisappeared;
    }

    public void OnIntruderDetected(Person person)
    {
        person.OnDetected();

        if (_movementDetector.IntrudersCount == 1)
        {
            _alarmAudio.Play();
            StartUpdateAlarmIntensity(AlarmIntensityMax);
            StartUpdateAlarmLightIntensity(true);
        }
    }

    public void OnIntruderDisappeared(Person person)
    {
        person.OnNotDetected();

        if (_movementDetector.IntrudersCount == 0)
        {
            StartUpdateAlarmIntensity(AlarmIntensityMin);
            StartUpdateAlarmLightIntensity(false);
        }
    }

    private void StartUpdateAlarmIntensity(float intensityTarget)
    {
        if (_alarmCoroutine != null)
            StopCoroutine(_alarmCoroutine);

        _alarmCoroutine = StartCoroutine(UpdateAlarmIntensity(intensityTarget));
    }

    private void StartUpdateAlarmLightIntensity(bool isAlarmWorking)
    {
        if (_alarmLightIntensityCoroutine != null)
            StopCoroutine(_alarmLightIntensityCoroutine);

        _alarmLightIntensityCoroutine = StartCoroutine(UpdateLightIntensity(isAlarmWorking));
    }

    private IEnumerator UpdateAlarmIntensity(float intensityTarget)
    {
        while (enabled && _alarmIntensity != intensityTarget)
        {
            _alarmIntensity = Mathf.MoveTowards(_alarmIntensity, intensityTarget, _intensityChangeSpeed * Time.deltaTime);
            _alarmAudio.volume = _alarmIntensity;
            yield return null;
        }

        if (_alarmIntensity == AlarmIntensityMin)
            _alarmAudio.Stop();

        _alarmCoroutine = null;
    }

    private IEnumerator UpdateLightIntensity(bool isAlarmWorking)
    {
        bool isWorking = true;

        while (isWorking && enabled)
        {
            _alarmLight.intensity = _alarmLightIntensity * _alarmIntensity * Mathf.Sin(Time.time * _alarmLightFlashFrequency);

            if (_alarmIntensity == AlarmIntensityMin && isAlarmWorking == false)
                isWorking = false;
            else
                yield return null;
        }

        _alarmLightIntensityCoroutine = null;
    }
}
