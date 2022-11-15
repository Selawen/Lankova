using UnityEngine;

interface IDamagable
{
    public void TakeDamage(float damage, Collider collider);
    public void TakeFlash(float intensity, Collider collider);
    public void TakeEMP(float intensity, Collider collider);

    public void TakeStun(float intensity, Collider collider);
}
