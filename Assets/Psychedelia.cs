using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Psychedelia : MonoBehaviour
{
    [SerializeField]
    private FirstPersonController firstPersonController;

    private float normalMoveSpeed;
    private float trippedMoveSpeed;
    private bool isTripping = false;

    [SerializeField]
    private PostProcessVolume postProcessVol;
    private ChromaticAberration chromaticAberration;

    public float fadeDuration = 2.0f;
    private float timer = 0.0f;
    private bool isFading = false;

    private void Start()
    {
        normalMoveSpeed = firstPersonController.walkSpeed;
        trippedMoveSpeed = normalMoveSpeed - 1.5f;

        postProcessVol.profile.TryGetSettings(out chromaticAberration);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTripping = !isTripping;

            if (isTripping)
            {
                StartFade();
                firstPersonController.walkSpeed = trippedMoveSpeed;
            }
            else
            {
                chromaticAberration.intensity.value = 0;
                firstPersonController.walkSpeed = trippedMoveSpeed;
            }
        }

        if (isFading)
        {
            timer += Time.deltaTime;

            // Calculate the intensity based on the timer
            float t = Mathf.Clamp01(timer / fadeDuration);
            chromaticAberration.intensity.value = Mathf.Lerp(0.0f, 1.0f, t);

            if (t >= 1.0f)
            {
                isFading = false;
            }
        }

        if (isTripping)
        {

        }
    }

    public void StartFade()
    {
        isFading = true;
        timer = 0.0f;
    }
}
