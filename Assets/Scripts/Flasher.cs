using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Flash(Color color, float FlashTime)
    {
        StartCoroutine(FlashC(color, FlashTime, FlashTime, FlashTime * 2));
    }

    public void Flash(Color color, float FlashTime, int amount)
    {
        StartCoroutine(FlashC(color, FlashTime, FlashTime, amount * (FlashTime * 2)));
    }

    public void Flash(Color color, float offTime, float onTime, int amount)
    {
        StartCoroutine(FlashC(color, offTime, onTime, amount * (offTime + onTime)));
    }

    public void Flash(Color color, float FlashTime, float totalDuration)
    {
        StartCoroutine(FlashC(color, FlashTime, FlashTime, totalDuration));
    }

    public void Flash(Color color, float offTime, float onTime, float totalDuration)
    {
        StartCoroutine(FlashC(color, offTime, onTime, totalDuration));
    }

    IEnumerator FlashC(Color color, float offTime, float onTime, float totalDuration)
    {
        foreach(Renderer r in renderers)
        {
            r.material.EnableKeyword("_EMISSION");
        }

        float t = 0;
        while (t < totalDuration)
        {
            t += Time.deltaTime;

            foreach (Renderer r in renderers)
            {
                r.material.SetColor("_EmissionColor", color);
            }
            yield return new WaitForSeconds(onTime);
            t += onTime;

            foreach (Renderer r in renderers)
            {
                r.material.SetColor("_EmissionColor", Color.black);
            }
            yield return new WaitForSeconds(offTime);
            t += offTime;
        }

        foreach (Renderer r in renderers)
        {
            r.material.SetColor("_EmissionColor", Color.black);
            r.material.DisableKeyword("_EMISSION");

        }
    }
}
