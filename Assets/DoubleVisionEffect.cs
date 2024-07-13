using UnityEngine;
using System.Collections;

public class DoubleVisionEffect : MonoBehaviour
{
    public Material blendMaterial;
    public float lerpDuration = 2f; // Duration of the lerping effect
    private bool isEffectActive = false; // Flag to track if the effect is currently active

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Apply the blend material to the full screen
        Graphics.Blit(source, destination, blendMaterial);
    }

    private void Awake()
    {
        blendMaterial.SetFloat("_DistortionFrequency", 0f);
    }

    // Function to toggle the Double Vision effect on/off
    public void ToggleDoubleVisionEffect(bool activate)
    {
        if (activate && !isEffectActive)
        {
            // Turn on the effect
            StartCoroutine(LerpDistortionFrequency(0f, 0.05f));
            isEffectActive = true;
        }
        else if (!activate && isEffectActive)
        {
            // Turn off the effect
            StartCoroutine(LerpDistortionFrequency(0.05f, 0f));
            isEffectActive = false;
        }
    }

    // Coroutine to lerp the distortionFrequency property
    private IEnumerator LerpDistortionFrequency(float startValue, float endValue)
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / lerpDuration);
            blendMaterial.SetFloat("_DistortionFrequency", currentValue);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Ensure the distortionFrequency is exactly the endValue
        blendMaterial.SetFloat("_DistortionFrequency", endValue);
    }

}
