using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MagicCircle : MonoBehaviour
{
    public GameObject defaultCircle;
    public GameObject glowingCircle;
    public FirstPersonController firstPersonController;
    public ParticleSystem _particleSys;
    public ParticleSystem _contParticleSys;

    private float normalMoveSpeed;
    private float normalMouseSens;

    [SerializeField]
    private AudioMixer _audioMixer;

    private void Start()
    {
        normalMoveSpeed = firstPersonController.walkSpeed;
        normalMouseSens = firstPersonController.mouseSensitivity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Psychedelia>().hasCollided)
        {
            other.GetComponent<Psychedelia>().hasCollided = true;
            firstPersonController.walkSpeed = normalMoveSpeed / 3;
            firstPersonController.bobSpeed = 10 * 1/3;
            firstPersonController.mouseSensitivity = normalMouseSens / 2;
            defaultCircle.SetActive(false);
            glowingCircle.SetActive(true);
            firstPersonController.GetComponent<Psychedelia>().doubleVisionEffect.ToggleDoubleVisionEffect(true);
            firstPersonController.GetComponent<Psychedelia>().chromaticAberration.intensity.value = 1.0f;
            AudioManager.AudioM.PlayMagicCircleBGM();
            _audioMixer.SetFloat("BirdsEcho", 311f);
            _contParticleSys.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Psychedelia>().isHexed)
        {
            other.GetComponent<Psychedelia>().isHexed = true;
            firstPersonController.GetComponent<Psychedelia>().Blink();
            glowingCircle.SetActive(false);
            _particleSys.Play();
            _contParticleSys.Stop();
            firstPersonController.walkSpeed = normalMoveSpeed;
            firstPersonController.bobSpeed = 10;
            firstPersonController.mouseSensitivity = normalMouseSens;
            AudioManager.AudioM.StopMagicCircleBGM();
            AudioManager.AudioM.PlayMagicSmite();
            _audioMixer.SetFloat("BirdsEcho", 1f);
        }
    }
}
