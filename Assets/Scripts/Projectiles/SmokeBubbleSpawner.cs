using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBubbleSpawner : MonoBehaviour
{
    public GameObject bubble;
    public int amount;

    private void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject b = Instantiate(bubble, transform);
            b.transform.localScale = Vector3.one * Random.Range(0.1f, 0.75f);
            b.transform.localPosition += new Vector3(
                Random.Range(-0.2f, 0.2f),
                Random.Range(-0.2f, 0.2f),
                Random.Range(-0.2f, 0.2f));
        }
    }
}
