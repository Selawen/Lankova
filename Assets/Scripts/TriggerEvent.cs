using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent enterEvent, exitEvent, stayEvent;

    private void OnTriggerEnter(Collider other)
    {
        enterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        exitEvent.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        stayEvent.Invoke();
    }
}
