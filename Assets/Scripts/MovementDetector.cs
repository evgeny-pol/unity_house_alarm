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
    private Coroutine _alarmCoroutine;
    private Coroutine _alarmLightIntensityCoroutine;
    private float _alarmIntensity;
    private float _alarmIntensityTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            person.OnDetected();
            ++_intrudersCount;
            _alarmIntensityTarget = AlarmIntensityMax;

            if (_intrudersCount == 1)
            {
                _alarmAudio.Play();
                _alarmCoroutine ??= StartCoroutine(UpdateAlarmIntensity());
                _alarmLightIntensityCoroutine ??= StartCoroutine(UpdateLightIntensity());
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
                _alarmIntensityTarget = AlarmIntensityMin;
                _alarmCoroutine ??= StartCoroutine(UpdateAlarmIntensity());
            }
        }
    }

    private IEnumerator UpdateAlarmIntensity()
    {
        while (enabled)
        {
            _alarmIntensity = Mathf.MoveTowards(_alarmIntensity, _alarmIntensityTarget, _intensityChangeSpeed * Time.deltaTime);
            _alarmAudio.volume = _alarmIntensity;

            if (_alarmIntensity == _alarmIntensityTarget)
            {
                if (_alarmIntensity == AlarmIntensityMin)
                    _alarmAudio.Stop();

                _alarmCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator UpdateLightIntensity()
    {
        while (enabled)
        {
            _alarmLight.intensity = _alarmLightIntensity * _alarmIntensity * Mathf.Sin(Time.time * _alarmLightFlashFrequency);

            if (_alarmIntensity == AlarmIntensityMin && _alarmIntensityTarget == AlarmIntensityMin)
            {
                _alarmLightIntensityCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }
}
