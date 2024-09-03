using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private SecurityCamera[] _cameras;

    private int _currentCameraIndex;

    private void Start()
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            if (i == _currentCameraIndex)
                _cameras[i].Activate();
            else
                _cameras[i].Deactivate();
        }
    }

    public void Switch()
    {
        if (_cameras.Length < 2)
            return;

        _cameras[_currentCameraIndex].Deactivate();
        _currentCameraIndex = ++_currentCameraIndex % _cameras.Length;
        _cameras[_currentCameraIndex].Activate();
    }
}
