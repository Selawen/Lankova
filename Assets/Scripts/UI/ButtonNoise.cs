using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonNoise : MonoBehaviour
{
    AudioSource buttonNoise;
    // Start is called before the first frame update
    void OnEnable()
    {
        SceneManager.sceneLoaded += AttachNoiseToAllButtons;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AttachNoiseToAllButtons;
    }

    void AttachNoiseToAllButtons(Scene scene, LoadSceneMode mode)
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        buttonNoise = GetComponent<AudioSource>();

        foreach (Button b in buttons)
        {
            b.onClick.AddListener(buttonNoise.Play);
        }
    }
}
