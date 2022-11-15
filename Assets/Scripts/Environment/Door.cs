using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject[] doors;
    public Transform[] openPositions, closedPositions;
    public float openTime = 0.5f, closeTime = 0.5f;
    bool moving;
    public bool isOpen;
    public int entitiesInRangeOfDoor = 0;
    Coroutine openRoutine, closeRoutine;
    public AudioSource audioSource;
    public AudioClip openClip, closeClip;

    private void Start()
    {
        moving = false;
        if (isOpen)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].transform.position = openPositions[i].position;
            }
        }
        else
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].transform.position = closedPositions[i].position;
            }
        }
    }

    public void EntityEntersRange()
    {
        entitiesInRangeOfDoor++;
        if(!isOpen) { OpenDoor(true); }
    }

    public void EntityExitsRange()
    {
        entitiesInRangeOfDoor--;
        if(entitiesInRangeOfDoor <= 0)
        {
            CloseDoor(true);
        }
    }

    public void OpenDoor(bool forced)
    {
        if (!gameObject.activeInHierarchy) { return; }
        if (openRoutine == null) { openRoutine = StartCoroutine(OpenC(forced)); audioSource.PlayOneShot(openClip); }        
    }

    IEnumerator OpenC(bool forced)
    {
        if ((moving && !forced)|| isOpen) { yield break; }
        if (closeRoutine != null) { StopCoroutine(closeRoutine); }

        moving = true;
        float t = 0.0f;

        Vector3[] startingPositions = new Vector3[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            startingPositions[i] = doors[i].transform.position;
        }
        
        while(t<=1.0f)
        {
            t += Time.deltaTime / openTime;
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].transform.position = Vector3.Lerp(startingPositions[i], openPositions[i].position, t);
            }

            yield return null;
        }
        isOpen = true;
        moving = false;
        openRoutine = null;
    }

    public void CloseDoor(bool forced)
    {
        if (!gameObject.activeInHierarchy) { return; }
        if (closeRoutine == null) { closeRoutine = StartCoroutine(CloseC(forced)); audioSource.PlayOneShot(closeClip); }

    }

    IEnumerator CloseC(bool forced)
    {
        if ((moving && !forced) || !isOpen) { yield break; }
        if (openRoutine != null) { StopCoroutine(openRoutine); }

        moving = true;
        float t = 0.0f;

        Vector3[] startingPositions = new Vector3[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            startingPositions[i] = doors[i].transform.position;
        }

        while (t <= 1.0f)
        {
            t += Time.deltaTime / closeTime;
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].transform.position = Vector3.Lerp(startingPositions[i], closedPositions[i].position, t);
            }

            yield return null;
        }
        isOpen = false;
        moving = false;
        closeRoutine = null;
    }
}
