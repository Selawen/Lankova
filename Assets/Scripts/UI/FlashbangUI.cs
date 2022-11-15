using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashbangUI : MonoBehaviour
{
    Image img;
    public float bufferTime, loseTime;
    bool flashed = false;

    public void Flash(float intensity)
    {
        gameObject.SetActive(true);

        StartCoroutine(FlashC(intensity));
    }

    IEnumerator FlashC(float intensity)
    {
        img = GetComponent<Image>();
        if (flashed) { yield break; }
        flashed = true;

        img.color = Color.Lerp(Color.clear, Color.white, intensity * 2);
        yield return new WaitForSeconds(bufferTime * intensity);

        while (intensity >= 0.0f)
        {
            intensity -= Time.deltaTime / loseTime;
            img.color = Color.Lerp(Color.clear, Color.white, intensity * 2);
            yield return null;
        }
        gameObject.SetActive(false);
        flashed = false;
    }
}
