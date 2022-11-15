using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : ProjectileMovement
{
    public GameObject explosionPrefab;
    public bool explodeOnTimeout = true;

    protected override void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    protected override void OnTimeOut()
    {
        if (explodeOnTimeout) { Instantiate(explosionPrefab, transform.position, transform.rotation); }
        base.OnTimeOut();
    }


}
