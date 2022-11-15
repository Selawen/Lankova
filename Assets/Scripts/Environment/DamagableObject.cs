using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagableObject : MonoBehaviour, IDamagable
{
    Flasher flasher;
    public int HP, armor;
    public UnityEvent onDestroyEvent;
    public bool destroyParentIfAllChildrenDestroyed = true;

    private void Start()
    {
        flasher = GetComponent<Flasher>();   
        if(flasher == null) { Debug.LogError("Destructable Object does not have flasher component"); }
    }

    public virtual void TakeDamage(float damage, Collider collider)
    {
        if(damage <= 0) { return; }
        HP -= Mathf.Max(1, (int)damage - armor);
        flasher.Flash(Color.red, 0.25f, 1);
        if (HP <= 0) { Destroy(gameObject); }
    }

    public virtual void TakeEMP(float intensity, Collider collider)
    {
        return;
    }

    public virtual void TakeFlash(float intensity, Collider collider)
    {
        return;
    }

    public virtual void TakeStun(float intensity, Collider collider)
    {
        return;
    }

    protected virtual void OnDestroy()
    {
        onDestroyEvent.Invoke();
        if(transform.parent.childCount <= 1 && destroyParentIfAllChildrenDestroyed) { Destroy(transform.parent.gameObject, 0.1f); }
    }
}
