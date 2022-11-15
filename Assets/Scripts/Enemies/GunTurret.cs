using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTurret : GunEnemy
{
    public void PowerDown()
    {
        stunned = true;
    }

    protected override void Update()
    {
        return;
    }

    public override void TakeEMP(float intensity, Collider collider)
    {
        StartCoroutine(TakeStunC(intensity));
    }

    public override void TakeStun(float intensity, Collider collider)
    {
        return;
    }

    public override void TakeFlash(float intensity, Collider collider)
    {
        return;
    }
}
