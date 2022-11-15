using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcingProjectile : MonoBehaviour
{
    public float moveSpeed, flightTime = 3.0f, damage = 5.0f, height = 1.0f;
    protected Rigidbody rb;
    public GameObject[] spawnOnImpact;
    public bool spawnOnTimeout = true, damageOnImpact = true, heightScalesWithDistance = true, speedScalesWithDistance = true;

    public void Init(Vector3 origin, Vector3 target)
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(TimeOut());
        StartCoroutine(Move(origin, target));
    }

    public void Init(Vector3 _origin, Vector3 _target, float _height)
    {
        height = _height;
        Init(_origin, _target);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (damageOnImpact)
        {
            //collision.rigidbody.GetComponent<DamageHandler>()?.TakeDamage(damage, collision.collider);
            //collision.rigidbody.GetComponent<Enemy>()?.TakeDamage(damage);
            collision.collider.GetComponentInParent<IDamagable>()?.TakeDamage(damage, collision.collider);
        }

        if (spawnOnImpact.Length > 0)
        {
            Spawn();
        }
        Destroy(gameObject);
    }

    void Spawn()
    {
        foreach(GameObject spawnable in spawnOnImpact)
        {
            Instantiate(spawnable, transform.position, Quaternion.identity);
        }
    }

    IEnumerator Move(Vector3 origin, Vector3 target)
    {
        float distance = Vector3.Distance(origin, target);
        if (heightScalesWithDistance) { height *= distance; }
        if (speedScalesWithDistance) { moveSpeed /= distance; }
        Vector3 control = origin + (target - origin) / 2 + Vector3.up * height;

        float t = 0.0f;
        while(t <= 1.1f)
        {
            t += Time.deltaTime * (moveSpeed);
            Vector3 m1 = Vector3.LerpUnclamped(origin, control, t);
            Vector3 m2 = Vector3.LerpUnclamped(control, target, t);

            rb.MovePosition(Vector3.LerpUnclamped(m1, m2, t));
            yield return null;
        }

        OnTimeOut();

    }

    protected virtual IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(flightTime);
        OnTimeOut();
    }

    protected virtual void OnTimeOut()
    {
        if (spawnOnTimeout) { Spawn(); }
        Destroy(gameObject);
    }
}
