using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeC(Camera.main, intensity, duration));
    }

    public void Shake(Camera camera, float intensity, float duration)
    {
        StartCoroutine(ShakeC(camera, intensity, duration));

    }

    IEnumerator ShakeC(Camera camera, float intensity, float duration)
    {
        float t = 0;
        
        while(t < duration)
        {
            t += Time.deltaTime;

            float xOffset = Random.Range(-intensity, intensity);
            float yOffset = Random.Range(-intensity, intensity);

            camera.transform.position += camera.transform.right * xOffset + camera.transform.up * yOffset;
            yield return null;

            t += Time.deltaTime;
            camera.transform.position -= camera.transform.right * xOffset + camera.transform.up * yOffset;
            yield return null;

        }
    }
}
