using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovementDetector : MonoBehaviour
{
    public event Action<Person> IntruderDetected;
    public event Action<Person> IntruderDisappeared;

    private int _intrudersCount;

    public int IntrudersCount => _intrudersCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            ++_intrudersCount;
            IntruderDetected?.Invoke(person);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Person person))
        {
            --_intrudersCount;
            IntruderDisappeared?.Invoke(person);
        }
    }
}
