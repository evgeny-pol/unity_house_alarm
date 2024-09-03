using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class SecurityCamera : MonoBehaviour
{
    private Camera _camera;
    private AudioListener _audioListener;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _audioListener = GetComponent<AudioListener>();
    }

    public void Activate() => SetActivity(true);

    public void Deactivate() => SetActivity(false);

    private void SetActivity(bool isActive)
    {
        _camera.enabled = isActive;
        _audioListener.enabled = isActive;
    }
}
