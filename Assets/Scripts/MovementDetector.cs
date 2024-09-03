using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovementDetector : MonoBehaviour
{
    private const float AlarmIntensityMin = 0f;
    private const float AlarmIntensityMax = 1f;

    [SerializeField] private Light _alarmLight;
    [SerializeField] private AudioSource _alarmAudio;
    [SerializeField, Min(0f)] private float _alarmLightIntensity = 1f;
    [SerializeField, Min(0f)] private float _intensityChangeSpeed = 0.3f;
    [SerializeField, Min(0f)] private float _alarmLightFlashFrequency = 4f;

    private int _intrudersCount;
    private float _alarmIntensity;
    private Coroutine _alarmCoroutine;
    private Coroutine _alarmLightIntensityCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            person.OnDetected();
            ++_intrudersCount;

            if (_intrudersCount == 1)
            {
                _alarmAudio.Play();
                StartUpdateAlarmIntensity(AlarmIntensityMax);
                StartUpdateAlarmLightIntensity(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            person.OnNotDetected();
            --_intrudersCount;

            if (_intrudersCount == 0)
            {
                StartUpdateAlarmIntensity(AlarmIntensityMin);
                StartUpdateAlarmLightIntensity(false);
            }
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
