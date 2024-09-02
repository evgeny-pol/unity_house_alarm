using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class SecurityCamera : MonoBehaviour
{
    private Camera _camera;
    private AudioListener _audioListener;

    public bool IsActive
    {
        set
        {
            _camera.enabled = value;
            _audioListener.enabled = value;
        }
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _audioListener = GetComponent<AudioListener>();
    }
}
