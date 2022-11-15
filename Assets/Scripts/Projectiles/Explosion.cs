using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float maxScale, explodeTime, damage, stayMaxTime = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explode());
        GetComponent<AudioSource>()?.Play();
    }

    IEnumerator Explode()
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * maxScale;

        float t = 0;
        while(t <= 1.0f)
        {
            t += Time.deltaTime / explodeTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);            
            yield return null;
        }

        yield return new WaitForSeconds(stayMaxTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.GetComponentInParent<DamageHandler>()?.TakeDamage(damage, other);
        //other.GetComponentInParent<Enemy>()?.TakeDamage(damage);   
        if (damage > 0) { other.GetComponentInParent<IDamagable>()?.TakeDamage(damage, other); }
    }
}
