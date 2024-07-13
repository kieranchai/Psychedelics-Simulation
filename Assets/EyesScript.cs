using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesScript : MonoBehaviour
{
    public Psychedelia psycheScript;

    public void FadeIn()
    {
        StartCoroutine(fadeIn());
    }

    public void FadeOut()
    {
        StartCoroutine(fadeOut());
    }

    IEnumerator fadeIn()
    {
        float t = 0f;

        while (t <= 0.5f)
        {
            t += Time.deltaTime;
            psycheScript.depthOfField.focusDistance.value = Mathf.Lerp(10f, 0f, t / 0.5f);
            psycheScript.autoExposure.minLuminance.value = Mathf.Lerp(0f, 2f, t / 0.5f);
            yield return null;
        }
        psycheScript.depthOfField.focusDistance.value = 0f;
        psycheScript.autoExposure.minLuminance.value = 2f;
    }

    IEnumerator fadeOut()
    {
        float t = 0f;

        while (t <= 8f)
        {
            t += Time.deltaTime;
            psycheScript.depthOfField.focusDistance.value = Mathf.Lerp(0f, 10f, t / 8f);

            if (t <= 1f)
            {
                psycheScript.autoExposure.minLuminance.value = Mathf.Lerp(2f, -3f, t / 1f);
            } else
            {
                psycheScript.autoExposure.minLuminance.value = Mathf.Lerp(-3f, -0f, t / 1f);
            }
            yield return null;
        }
        psycheScript.depthOfField.focusDistance.value = 10f;
        psycheScript.autoExposure.minLuminance.value = 0f;
    }
}
