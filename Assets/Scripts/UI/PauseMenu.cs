using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    public KeyCode[] pauseKey = { KeyCode.Escape };
    public UnityEvent pauseEvents;

    private void Update()
    {
        foreach(KeyCode key in pauseKey)
        {
            if(Input.GetKeyDown(key))
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseEvents.Invoke();
    }
}
