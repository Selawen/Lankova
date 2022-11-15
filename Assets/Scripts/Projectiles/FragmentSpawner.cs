using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSpawner : MonoBehaviour
{
    public int amountOfFragments;
    public GameObject fragment;

    private void Start()
    {
        for (int i = 0; i < amountOfFragments; i++)
        {
            Instantiate(fragment, transform.position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }

        Destroy(gameObject);
    }
}
