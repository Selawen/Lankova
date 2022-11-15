using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetDummy : DamagableObject
{
    public UnityEvent onStun, onFlash, onEMP, onSmoke;
    bool wasFlashed = false, wasEMPd = false, wasStunned = false, wasSmoked = false;

    public override void TakeEMP(float intensity, Collider collider)
    {
        if (wasEMPd) { return; }
        wasEMPd = true;
        onEMP.Invoke();
    }
    
    public override void TakeFlash(float intensity, Collider collider)
    {
        if (wasFlashed) { return; }
        wasFlashed = true;
        onFlash.Invoke();
    }
    
    public override void TakeStun(float intensity, Collider collider)
    {
        if (wasStunned) { return; }
        wasStunned = true;
        onStun.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Smoke") && !wasSmoked) { onSmoke.Invoke(); wasSmoked = true; }

    }
}
