using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footstepVariations = new AudioClip[5];
    public AudioSource _audioSource;

    public void PlayFootstep()
    {
        AudioClip footstepClip = footstepVariations[Random.Range(0, footstepVariations.Length)];
        _audioSource.volume = Random.Range(0.8f, 1.0f);
        _audioSource.pitch = Random.Range(0.85f, 1.1f);

        _audioSource.clip = footstepClip;
        _audioSource.Play();
    }

    public void StopFootstep()
    {
        _audioSource.Stop();
    }

    public bool IsPlayingFootstep()
    {
        return _audioSource.isPlaying;
    }
}
