using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager AudioM { get; private set; }

    public AudioSource eerieAS;
    public float fadeDuration = 2.0f;
    public float targetVolume = 1.0f;
    public float minVolume = 0.0f;

    public AudioSource buzzingAS;
    public AudioSource magicCircleAS;
    public AudioSource magicSmiteAS;
    public AudioSource heartbeatAS;

    private void Awake()
    {
        AudioM = this;
    }

    public void PlayMagicCircleBGM()
    {
        magicCircleAS.volume = 0.0f;
        StartCoroutine(FadeInVolumeMagic());

        if (!magicCircleAS.isPlaying) magicCircleAS.Play();
    }

    public void StopMagicCircleBGM()
    {
        StartCoroutine(FadeOutVolumeMagic());
    }

    public void PlayMagicSmite()
    {
        magicSmiteAS.Play();
    }

    public void PlayEerieBGM()
    {
        if (eerieAS != null && eerieAS.clip != null)
        {
            eerieAS.volume = 0.0f;
            StartCoroutine(FadeInVolume());
        }

        if (!eerieAS.isPlaying) eerieAS.Play();
    }

    public void PauseEerieBGM()
    {
        if (eerieAS != null)
        {
            StartCoroutine(FadeOutVolume());
        }
    }

    public void PlayBuzzing()
    {
        buzzingAS.Play();
    }

    public void PlayHeartbeat()
    {
        heartbeatAS.Play();
    }

    private IEnumerator FadeInVolumeMagic()
    {
        float startVolume = magicCircleAS.volume;
        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            magicCircleAS.volume = Mathf.Lerp(startVolume, 0.7f, timer / fadeDuration);
            yield return null;
        }

        magicCircleAS.volume = 0.7f;
    }

    private IEnumerator FadeOutVolumeMagic()
    {
        float startVolume = magicCircleAS.volume;
        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            magicCircleAS.volume = Mathf.Lerp(startVolume, minVolume, timer / fadeDuration);
            yield return null;
        }

        magicCircleAS.volume = minVolume;
        magicCircleAS.Stop();
    }

    private IEnumerator FadeInVolume()
    {
        float startVolume = eerieAS.volume;
        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            eerieAS.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }

        eerieAS.volume = targetVolume;
    }

    private IEnumerator FadeOutVolume()
    {
        float startVolume = eerieAS.volume;
        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            eerieAS.volume = Mathf.Lerp(startVolume, minVolume, timer / fadeDuration);
            yield return null;
        }

        eerieAS.volume = minVolume;
        eerieAS.Pause();
    }
}
