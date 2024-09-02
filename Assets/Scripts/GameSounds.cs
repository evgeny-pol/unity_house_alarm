using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSounds : MonoBehaviour
{
    [SerializeField] private AudioClip _click;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        _audioSource.PlayOneShot(_click);
    }
}
