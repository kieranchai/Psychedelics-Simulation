using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Kaleidoscope : MonoBehaviour
{
    public Psychedelia psycheScript;

    public Material postProcessMaterial;
    public float lerpDuration = 15f;
    private bool isEffectActive = false;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, postProcessMaterial);
    }

    private void Awake()
    {
        postProcessMaterial.SetFloat("_NumSegments", 1);
        postProcessMaterial.SetFloat("_RotationSpeed", 0f);
    }

    public void ToggleKaleidoscopeEffect(bool activate)
    {
        if (activate && !isEffectActive)
        {
            // Turn on the effect
            StartCoroutine(LerpKaleidoscopeStrength(0f, 12f, 0f, 1f, 0f, -100f));
            isEffectActive = true;
        }
        else if (!activate && isEffectActive)
        {
            // Turn off the effect
            StartCoroutine(LerpKaleidoscopeStrength(12f, 1f, 1f, 0f, -100f, 0f));
            isEffectActive = false;
        }
    }

    private IEnumerator LerpKaleidoscopeStrength(float startValue, float endValue, float startValue2, float endValue2, float startValue3, float endValue3)
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / lerpDuration);
            postProcessMaterial.SetFloat("_NumSegments", currentValue);

            float currentValue2 = Mathf.Lerp(startValue2, endValue2, elapsedTime / lerpDuration);
            postProcessMaterial.SetFloat("_RotationSpeed", currentValue2);

            psycheScript.lensDistortion.intensity.value = Mathf.Lerp(startValue3, endValue3, elapsedTime / lerpDuration);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        postProcessMaterial.SetFloat("_NumSegments", endValue);
        postProcessMaterial.SetFloat("_RotationSpeed", endValue2);
    }
}
