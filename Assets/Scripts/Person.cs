using UnityEngine;

public class Person : MonoBehaviour
{
    private int _detectorsCount;

    public bool IsDetected => _detectorsCount > 0;

    public virtual void OnDetected()
    {
        ++_detectorsCount;
    }

    public virtual void OnNotDetected()
    {
        --_detectorsCount;
    }
}
