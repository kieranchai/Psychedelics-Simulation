using System;
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
    private float normalMouseSens;
    private float trippedMouseSens;

    [SerializeField]
    private PostProcessVolume postProcessVol;
    private ColorGrading colorGrading;
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;
    [HideInInspector]
    public DepthOfField depthOfField;
    [HideInInspector]
    public AutoExposure autoExposure;
    [HideInInspector]
    public LensDistortion lensDistortion;

    public float fadeDuration = 0.1f;
    private float timer = 0.0f;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private bool isBlinking = false;

    public float newScrollSpeed = 0.005f;
    private Camera mainCamera;

    [SerializeField]
    private DoubleVisionEffect doubleVisionEffect;
    private float doubleVisionInterval = 0f;
    private float doubleVisionTimer = 0f;

    [SerializeField]
    private Animator eyesAnim;

    Renderer[] renderers;

    private enum tripLevel
    {
        Low,
        Medium,
        High,
        Highest
    }
    private tripLevel currentTripLevel = tripLevel.Low;
    [HideInInspector]
    public float tripTime = 0.0f;
    [HideInInspector]
    public bool hasTripped = false;
    private float currentChromaticAberrationIntensity = 0.0f;
    private float currentMotionBlurIntensity = 0.0f;
    private float currentMoveSpeed = 0.0f;
    private float currentMouseSens = 0.0f;
    private float chromaticAberrationIntensity = 0.0f;
    private float motionBlurIntensity = 0.0f;
    private float blinkOutTimer = 0.0f;
    private float blinkOutInterval = 14.0f;
    private float blinkInTimer = 0.0f;
    private float blinkInInterval = 5.0f;
    private float maxColorGrading = 0.0f;
    private float currentColorGrading = 0.0f;
    private float currentLensDistortion = 0.0f;
    private float maxLensDistortion = 0.0f;
    private float kaleidoscopeTimer = 0.0f;
    private bool hasKaleidoscopeStarted = false;
    [SerializeField]
    private Kaleidoscope kaleidoscopeEffect;

    private void Start()
    {
        normalMoveSpeed = firstPersonController.walkSpeed;
        normalMouseSens = firstPersonController.mouseSensitivity;

        postProcessVol.profile.TryGetSettings(out colorGrading);
        postProcessVol.profile.TryGetSettings(out chromaticAberration);
        postProcessVol.profile.TryGetSettings(out motionBlur);
        postProcessVol.profile.TryGetSettings(out depthOfField);
        postProcessVol.profile.TryGetSettings(out autoExposure);
        postProcessVol.profile.TryGetSettings(out lensDistortion);

        mainCamera = Camera.main;

        renderers = FindObjectsOfType<Renderer>();
    }

    void Update()
    {
        tripTime += Time.deltaTime;
        if (tripTime >= 0.0 && tripTime < 20.0)
        {
            currentTripLevel = tripLevel.Low;
            currentChromaticAberrationIntensity = 0.0f;
            currentMotionBlurIntensity = 120.0f;
            currentMoveSpeed = normalMoveSpeed;
            currentMouseSens = normalMouseSens;

            chromaticAberrationIntensity = 0.4f;
            motionBlurIntensity = 200f;
            trippedMoveSpeed = normalMoveSpeed * 3 / 4;
            trippedMouseSens = normalMouseSens * 3 / 4;
            firstPersonController.bobSpeed = 10 * 3 / 4;
            maxColorGrading = 12f;
            maxLensDistortion = 12;
        }
        else if (tripTime >= 20.0 && tripTime < 35.0)
        {
            currentTripLevel = tripLevel.Medium;
            currentChromaticAberrationIntensity = chromaticAberrationIntensity;
            currentMotionBlurIntensity = motionBlurIntensity;
            currentMoveSpeed = trippedMoveSpeed;
            currentMouseSens = trippedMouseSens;

            chromaticAberrationIntensity = 0.66f;
            motionBlurIntensity = 280f;
            trippedMoveSpeed = normalMoveSpeed * 2 / 3;
            trippedMouseSens = normalMouseSens * 2 / 3;
            firstPersonController.bobSpeed = 10 * 2/3;
            maxColorGrading = 20f;
            maxLensDistortion = 50f;
        }
        else if (tripTime >= 35.0 && tripTime < 60)
        {
            currentTripLevel = tripLevel.High;
            currentChromaticAberrationIntensity = chromaticAberrationIntensity;
            currentMotionBlurIntensity = motionBlurIntensity;
            currentMoveSpeed = trippedMoveSpeed;
            currentMouseSens = trippedMouseSens;

            chromaticAberrationIntensity = 1.00f;
            motionBlurIntensity = 360f;
            trippedMoveSpeed = normalMoveSpeed * 1 / 3;
            trippedMouseSens = normalMouseSens * 1 / 3;
            firstPersonController.bobSpeed = 10 * 1 / 3;
            maxColorGrading = 40f;
            maxLensDistortion = 70f;
        }
        else if (tripTime >= 60.0)
        {
            currentTripLevel = tripLevel.Highest;

            // effects are better (reduced), but now triggers kaleidoscopic effects
            maxColorGrading = 20f;
            maxLensDistortion = 0f;
            trippedMoveSpeed = normalMoveSpeed * 2 / 3;
            trippedMouseSens = normalMouseSens * 2 / 3;
            firstPersonController.bobSpeed = 10 * 2 / 3;
        }

        if (tripTime >= 5.0 && !hasTripped)
        {
            hasTripped = true;
            StartFadeIn();
        }

        if (hasTripped && !isTripping)
        {
            blinkInTimer += Time.deltaTime;
            if (blinkInTimer >= blinkInInterval && !isFadingIn)
            {
                currentColorGrading = UnityEngine.Random.Range(-maxColorGrading, maxColorGrading);
                StartFadeIn();
            }
        }

        if (isFadingIn)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            if (!isBlinking)
            {
                isBlinking = true;
                eyesAnim.Play("Blink");
            }
            chromaticAberration.intensity.value = Mathf.Lerp(currentChromaticAberrationIntensity, chromaticAberrationIntensity, t);
            motionBlur.shutterAngle.value = Mathf.Lerp(currentMotionBlurIntensity, motionBlurIntensity, t);
            firstPersonController.walkSpeed = Mathf.Lerp(currentMoveSpeed, trippedMoveSpeed, t);
            firstPersonController.mouseSensitivity = Mathf.Lerp(currentMouseSens, trippedMouseSens, t);
            colorGrading.hueShift.value = Mathf.Lerp(0f, currentColorGrading, t);
            lensDistortion.intensity.value = Mathf.Lerp(0f, maxLensDistortion, t);

            if (t >= 1.0f && isFadingIn)
            {
                firstPersonController.subtleSwayEnabled = true;
                isFadingIn = false;
                isBlinking = false;
                isTripping = true;
            }
        }

        if (isTripping && !isFadingIn)
        {
            MoveTextures();

            float pulse = Mathf.Sin(Time.time * 1.0f);
            float targetHueShift = Mathf.Lerp(-maxColorGrading, maxColorGrading, (pulse + 1f) / 2f);
            colorGrading.hueShift.value = Mathf.Lerp(colorGrading.hueShift.value, targetHueShift, Time.deltaTime);

            if (currentTripLevel != tripLevel.Highest)
            {
                float targetLensDistortion = Mathf.Lerp(-maxLensDistortion, maxLensDistortion, (pulse + 1f) / 2f);
                lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, targetLensDistortion, Time.deltaTime);

                blinkOutTimer += Time.deltaTime;
                // Check if Double Vision should be activated
                if (currentTripLevel != tripLevel.Low && doubleVisionTimer <= 0.0f && doubleVisionInterval <= 0.0f)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0) ShowDoubleVision();
                }

                // Only if Double Vision Activated
                if (doubleVisionTimer > 0.0f) doubleVisionTimer -= Time.deltaTime;
                else
                {
                    doubleVisionTimer = 0.0f;
                    doubleVisionEffect.ToggleDoubleVisionEffect(false);
                }

                if (doubleVisionTimer <= 0.0f && doubleVisionInterval > 0.0f) doubleVisionInterval -= Time.deltaTime;

                // Blink interval while tripping
                if (blinkOutTimer >= blinkOutInterval && !isFadingOut)
                {
                    currentColorGrading = colorGrading.hueShift.value;
                    currentLensDistortion = lensDistortion.intensity.value;
                    StartFadeOut();
                }
            }
            else
            {
                if (!hasKaleidoscopeStarted)
                {
                    hasKaleidoscopeStarted = true;
                    kaleidoscopeEffect.ToggleKaleidoscopeEffect(true);
                }
            }
        }

        if (hasKaleidoscopeStarted)
        {
            kaleidoscopeTimer += Time.deltaTime;
            float pulse = Mathf.Sin(Time.time * 1.0f);
            float targetHueShift = Mathf.Lerp(-100, 100, (pulse + 1f) / 2f);
            colorGrading.hueShift.value = Mathf.Lerp(colorGrading.hueShift.value, targetHueShift, Time.deltaTime);

            if (kaleidoscopeTimer >= 20f)
            {
                kaleidoscopeTimer = 0f;
                hasKaleidoscopeStarted = false;
                isTripping = false;
                tripTime = 0.0f;
                hasTripped = false;
                kaleidoscopeEffect.ToggleKaleidoscopeEffect(false);
                StartFadeOut();
            }
        }

        if (isFadingOut)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            if (!isBlinking)
            {
                isBlinking = true;
                eyesAnim.Play("Blink");
            }
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberrationIntensity, 0.0f, t);
            motionBlur.shutterAngle.value = Mathf.Lerp(motionBlurIntensity, 120f, t);
            firstPersonController.walkSpeed = Mathf.Lerp(trippedMoveSpeed, normalMoveSpeed, t);
            firstPersonController.bobSpeed = 10;
            firstPersonController.mouseSensitivity = Mathf.Lerp(trippedMouseSens, normalMouseSens, t);
            colorGrading.hueShift.value = Mathf.Lerp(currentColorGrading, 0f, t);
            lensDistortion.intensity.value = Mathf.Lerp(currentLensDistortion, 0f, t);

            if (t >= 1.0f)
            {
                firstPersonController.subtleSwayEnabled = false;
                isFadingOut = false;
                isBlinking = false;
                isTripping = false;
                StopTripping();
            }
        }
    }

    public void StartFadeIn()
    {
        isFadingIn = true;
        timer = 0.0f;
        blinkInTimer = 0.0f;
        blinkOutTimer = 0.0f;
    }

    public void StartFadeOut()
    {
        isFadingOut = true;
        timer = 0.0f;
        blinkOutTimer = 0.0f;
        blinkInTimer = 0.0f;
        blinkOutInterval = UnityEngine.Random.Range(12f, 18f);
        blinkInInterval = UnityEngine.Random.Range(3f, 10f);

        if (currentTripLevel == tripLevel.High)
        {
            blinkOutInterval = UnityEngine.Random.Range(25f, 40f);
            blinkInInterval = UnityEngine.Random.Range(0f, 3f);
        }
    }

    public void StopTripping()
    {
        chromaticAberration.intensity.value = 0;
        motionBlur.shutterAngle.value = 120f;
        firstPersonController.walkSpeed = normalMoveSpeed;
        firstPersonController.mouseSensitivity = normalMouseSens;
        doubleVisionEffect.ToggleDoubleVisionEffect(false);
        doubleVisionTimer = 0.0f;

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;
            if (material.shader.name == "Custom/ScrollingSwirlingTextureShader")
            {
                material.SetFloat("_ScrollSpeed", 0);
                material.SetFloat("_SwirlStrength", 0);
                material.SetFloat("_EnableColorPulsing", 0.0f);
            }
        }
    }

    public void MoveTextures()
    {
        foreach (Renderer renderer in renderers)
        {
            if (IsRendererInView(renderer))
            {
                Material material = renderer.material;
                if (material.shader.name == "Custom/ScrollingSwirlingTextureShader")
                {
                    //If already moving
                    if (material.GetFloat("_ScrollSpeed") != 0.0f) continue;

                    material.SetFloat("_ScrollSpeed", UnityEngine.Random.Range(-newScrollSpeed, newScrollSpeed));

                    if (UnityEngine.Random.Range(0, 2) == 0) material.SetFloat("_SwirlStrength", UnityEngine.Random.Range(-0.01f, 0.01f));

                    Color materialColor = material.GetColor("_Color");
                    if (materialColor != new Color32(255, 255, 255, 0))
                    {
                        material.SetFloat("_EnableColorPulsing", 1.0f);
                    }
                }
            }
        }
    }

    public void ShowDoubleVision()
    {
        doubleVisionInterval = 14.0f;
        doubleVisionTimer = 5.0f;
        if (currentTripLevel == tripLevel.High) doubleVisionTimer = 15.0f;
        doubleVisionEffect.ToggleDoubleVisionEffect(true);
    }

    bool IsRendererInView(Renderer renderer)
    {
        // Check if the renderer is within the camera's view frustum
        if (!renderer.isVisible)
            return false;

        // Check if the renderer is within the camera's field of view
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(renderer.bounds.center);
        return screenPoint.z > 0 && screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

}
