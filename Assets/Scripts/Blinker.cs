using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Blink(float blinkTime, int amount)
    {
        StartCoroutine(BlinkC(blinkTime, blinkTime, amount*(blinkTime*2)));
    }

    public void Blink(float offTime, float onTime, int amount)
    {
        StartCoroutine(BlinkC(offTime, onTime, amount*(offTime+onTime)));
    }

    public void Blink(float blinkTime, float totalDuration)
    {
        StartCoroutine(BlinkC(blinkTime, blinkTime, totalDuration));
    }

    public void Blink(float offTime, float onTime, float totalDuration)
    {
        StartCoroutine(BlinkC(offTime, onTime, totalDuration));
    }

    IEnumerator BlinkC(float offTime, float onTime, float totalDuration)
    {
        float t = 0;
        while(t<totalDuration)
        {
            t += Time.deltaTime;
            
            foreach(Renderer r in renderers)
            {
                r.enabled = false;
            }
            yield return new WaitForSeconds(offTime);
            t += offTime;

            foreach (Renderer r in renderers)
            {
                r.enabled = true;
            }
            yield return new WaitForSeconds(onTime);
            t += onTime;
        }

        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
    }
}
