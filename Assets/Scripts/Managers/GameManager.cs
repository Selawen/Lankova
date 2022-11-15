using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UnityEvent deathEvents, winEvents;
    public Texture2D cursor;
    public UnityEvent pauseEvents;
    public bool canPause;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware); //WHY
        canPause = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    #region ManaBasic Functionality
    public void Die()
    {
        deathEvents.Invoke();
    }

    public void Win()
    {
        winEvents.Invoke();
    }

    public void SetPauseable(bool _canPause)
    {
        canPause = _canPause;
    }

    public void TogglePause()
    {
        if (!canPause) { return; }
        pauseEvents.Invoke();
    }
    #endregion

    #region debug

    #endregion
}
