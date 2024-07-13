using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;
    [HideInInspector]
    public DepthOfField depthOfField;
    [HideInInspector]
    public AutoExposure autoExposure;

    public float fadeDuration = 0.1f;
    private float timer = 0.0f;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private bool isBlinking = false;

    public float newScrollSpeed = 0.005f; 
    private Camera mainCamera;

    [SerializeField]
    private DoubleVisionEffect doubleVisionEffect;
    private float doubleVisionInterval = 10f;
    private float doubleVisionTimer = 0f;

    [SerializeField]
    private Animator eyesAnim;

    Renderer[] renderers;

    private void Start()
    {
        normalMoveSpeed = firstPersonController.walkSpeed;
        trippedMoveSpeed = normalMoveSpeed - 3f;
        normalMouseSens = firstPersonController.mouseSensitivity;
        trippedMouseSens = normalMouseSens / 3;

        postProcessVol.profile.TryGetSettings(out chromaticAberration);
        postProcessVol.profile.TryGetSettings(out motionBlur);
        postProcessVol.profile.TryGetSettings(out depthOfField);
        postProcessVol.profile.TryGetSettings(out autoExposure);

        mainCamera = Camera.main;

        renderers = FindObjectsOfType<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTripping = !isTripping;

            if (isTripping)
            {
                StartFadeIn();
            }
            else
            {
                StartFadeOut();
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
            chromaticAberration.intensity.value = Mathf.Lerp(0.0f, 1.0f, t);
            motionBlur.shutterAngle.value = Mathf.Lerp(120f, 360f, t);
            firstPersonController.walkSpeed = Mathf.Lerp(normalMoveSpeed, trippedMoveSpeed, t);
            firstPersonController.mouseSensitivity = Mathf.Lerp(normalMouseSens, trippedMouseSens, t);
            if (t >= 1.0f)
            {
                isFadingIn = false;
                isBlinking = false;
            }
        }

        if (isTripping && !isFadingIn)
        {
            MoveTextures();

            // Check if Double Vision should be activated
            if (doubleVisionTimer <= 0.0f && doubleVisionInterval <= 0.0f)
            {
                if (Random.Range(0, 2) == 0) ShowDoubleVision();
            }

            // Only if Double Vision Activated
            if (doubleVisionTimer > 0.0f) doubleVisionTimer -= Time.deltaTime;
            else
            {
                doubleVisionTimer = 0.0f;
                doubleVisionEffect.ToggleDoubleVisionEffect(false);
            }

            if (doubleVisionTimer <= 0.0f && doubleVisionInterval > 0.0f) doubleVisionInterval -= Time.deltaTime;
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
            chromaticAberration.intensity.value = Mathf.Lerp(1.0f, 0.0f, t);
            motionBlur.shutterAngle.value = Mathf.Lerp(360f, 120f, t);
            firstPersonController.walkSpeed = Mathf.Lerp(trippedMoveSpeed, normalMoveSpeed, t);
            firstPersonController.mouseSensitivity = Mathf.Lerp(trippedMouseSens, normalMouseSens, t);
            if (t >= 1.0f)
            {
                isFadingOut = false;
                isBlinking = false;
                StopTripping();
            }
        }
    }

    public void StartFadeIn()
    {
        isFadingIn = true;
        timer = 0.0f;
    }

    public void StartFadeOut()
    {
        isFadingOut = true;
        timer = 0.0f;
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
                material.SetFloat("_ScrollSpeed", 0); // Should lerp towards intended value instead of directly changing
                material.SetFloat("_SwirlStrength", 0); // Should lerp towards intended value instead of directly changing
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

                    material.SetFloat("_ScrollSpeed", Random.Range(-newScrollSpeed, newScrollSpeed));

                    if (Random.Range(0, 2) == 0) material.SetFloat("_SwirlStrength", Random.Range(-0.01f, 0.01f));

                    Color materialColor = material.GetColor("_Color");
                    if (materialColor != new Color32(255,255,255,0))
                    {
                        material.SetFloat("_EnableColorPulsing", 1.0f);
                    }
                }
            }
        }
    }

    public void ShowDoubleVision()
    {
        doubleVisionInterval = 30.0f;
        doubleVisionTimer = 5.0f;
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
