using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamagable
{
    protected float hp;
    public float maxHP;
    protected Rigidbody rb;
    protected Flasher flasher;
    public float armor = 0.0f;

    protected virtual void Start()
    {
        hp = maxHP;
        rb = GetComponent<Rigidbody>();
        flasher = GetComponent<Flasher>();
    }

    protected virtual void Die()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage, Collider collider)
    {
        hp -= Mathf.Max(1.0f, damage-armor);
        flasher.Flash(Color.red, 0.2f, 1);
        if (hp <= 0) { Die(); }
    }

    public abstract void TakeFlash(float intensity, Collider collider);

    public abstract void TakeEMP(float intensity, Collider collider);

    public abstract void TakeStun(float intensity, Collider collider);
}
