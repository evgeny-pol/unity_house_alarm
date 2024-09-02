using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private SecurityCamera[] _cameras;

    private int _currentCameraIndex;

    private void Start()
    {
        for (int i = 0; i < _cameras.Length; i++)
            _cameras[i].IsActive = i == _currentCameraIndex;
    }

    public void Switch()
    {
        int prevIndex = _currentCameraIndex++;
        _currentCameraIndex %= _cameras.Length;

        if (_currentCameraIndex != prevIndex)
        {
            _cameras[prevIndex].IsActive = false;
            _cameras[_currentCameraIndex].IsActive = true;
        }
    }
}
