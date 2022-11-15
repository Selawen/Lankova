using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityCountTriggerEvent : MonoBehaviour
{
    int entitiesInTrigger = 0;

    public UnityEvent onFirstEntityTriggerEnter, onLastEntityTriggerLeave;


    private void OnTriggerEnter(Collider other)
    {
        entitiesInTrigger++;
        if(entitiesInTrigger == 1) { onFirstEntityTriggerEnter.Invoke(); }
    }

    private void OnTriggerExit(Collider other)
    {
        entitiesInTrigger--;
        if(entitiesInTrigger <= 0) { onLastEntityTriggerLeave.Invoke(); }
    }
}
