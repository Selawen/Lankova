using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpairEffect { Flash, Stun, EMP }
public class ImpairOnTrigger : MonoBehaviour
{    
    public ImpairEffect eff;
    public float timeOutTime = 0.15f;
    float scalefactor;

    private void Start()
    {
        scalefactor = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3;
        StartCoroutine(TimeOut());
        GetComponent<AudioSource>()?.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        float intensity = 1.0f - Vector3.Distance(transform.position, other.transform.position) / scalefactor;
        switch (eff)
        {
            case ImpairEffect.Flash:
                other.GetComponentInParent<IDamagable>()?.TakeFlash(intensity, other);
                break;
            case ImpairEffect.EMP:
                other.GetComponentInParent<IDamagable>()?.TakeEMP(intensity, other);
                break;
            case ImpairEffect.Stun:
                other.GetComponentInParent<IDamagable>()?.TakeStun(intensity, other);
                break;
            default: Debug.LogError("Tried to activate non-handled impair effect");
                break;
        }
    }

    IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(timeOutTime);
        Destroy(gameObject);
    }
}
