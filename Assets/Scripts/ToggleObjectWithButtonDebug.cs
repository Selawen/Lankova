using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectWithButtonDebug : MonoBehaviour
{
    public KeyCode button = KeyCode.R;
    public GameObject obbie;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(button))
        {
            obbie.SetActive(!obbie.activeSelf);
        }
    }
}
