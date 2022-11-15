using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEnemy : Enemy
{
    [Header("Stats")]
    public float moveSpeed;
    public float flashedTime, smokeSpreadMultiplier = 3.0f, flashSpreadMultiplier = 6.0f;
    public LayerMask seePlayerMask, seePlayerMaskNoSmoke;

    [Header("Primary Weapon")]
    public float fireRate;
    public float reloadTime, bulletSpread, range, minRange, stunnedTime;
    public int clipSize;
    protected int currentBullets;
    public GameObject bulletPrefab, bulletSpawn, armPivot;
    public AudioSource firingSound;

    [Header("Secondary Weapon")]
    public GameObject secondaryProjectile;
    public int secondaryAmmo;
    public float secondaryRange, secondaryCooldown, secondaryScatter;
    public Transform secondarySpawn;

    protected bool playerInSight, inMinimumRange, stunned, flashed, inSmoke, canUseSecondary;



    protected override void Start()
    {
        base.Start();
        currentBullets = clipSize;
        StartCoroutine(Shoot());
        stunned = false; flashed = false; canUseSecondary = true;
    }

    protected virtual void Update()
    {
        if (playerInSight && !stunned)
        {
            transform.LookAt(Vector3.Scale(PlayerMovement.Instance.transform.position, new Vector3(1, 0, 1)) + transform.position.y * Vector3.up);
            float curDis = Vector3.Distance(Vector3.Scale(new Vector3(1, 0, 1), transform.position), Vector3.Scale(new Vector3(1, 0, 1), PlayerMovement.Instance.transform.position));
            inMinimumRange = curDis < minRange;

            if (!inMinimumRange)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }

            if(curDis <= secondaryRange && secondaryAmmo > 0 && secondaryProjectile != null && canUseSecondary)
            {
                canUseSecondary = false; StartCoroutine(ReloadSecondary());
                secondaryAmmo--;
                Vector3 target = PlayerMovement.Instance.transform.position +
                    new Vector3(Random.Range(-secondaryScatter, secondaryScatter), 
                    Random.Range(-secondaryScatter, secondaryScatter), 
                    Random.Range(-secondaryScatter, secondaryScatter));

                secondarySpawn.LookAt(target);
                GameObject sec = Instantiate(secondaryProjectile, secondarySpawn.position, secondarySpawn.rotation);
                sec.GetComponent<ArcingProjectile>()?.Init(secondarySpawn.position, target);
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Smoke"))
        {
            inSmoke = true;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Smoke"))
        {
            inSmoke = false;
        }
    }

    protected IEnumerator ReloadSecondary()
    {
        canUseSecondary = false;
        yield return new WaitForSeconds(secondaryCooldown);
        canUseSecondary = true;
    }

    protected IEnumerator Shoot()
    {
        while(true)
        {
            while (stunned) { yield return new WaitForSeconds(0.5f); }
            RaycastHit hit;
            if (Physics.Raycast(transform.position, PlayerMovement.Instance.transform.position - transform.position, out hit, range, seePlayerMaskNoSmoke))
            { playerInSight = hit.collider.CompareTag("Player"); }
            else { playerInSight = false; }            

            while (!playerInSight)
            {
                yield return new WaitForSeconds(1.0f);
                if (Physics.Raycast(transform.position, PlayerMovement.Instance.transform.position - transform.position, out hit, range, seePlayerMaskNoSmoke))
                { playerInSight = hit.collider.CompareTag("Player");  } 
                else { playerInSight = false; }
            }

            //Spread calculation
            float actualSpread = bulletSpread;

            Physics.Raycast(transform.position, PlayerMovement.Instance.transform.position - transform.position, out hit, range, seePlayerMask);
            if (hit.collider.CompareTag("Smoke")) { actualSpread *= smokeSpreadMultiplier; }
            if (inSmoke) { actualSpread *= smokeSpreadMultiplier * 0.5f; }
            if (flashed) { actualSpread *= flashSpreadMultiplier; }


            armPivot.transform.LookAt(PlayerMovement.Instance.transform.position + 
                new Vector3(Random.Range(-actualSpread, actualSpread), Random.Range(-actualSpread, actualSpread), Random.Range(-actualSpread, actualSpread)));

            firingSound.Play();
            Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            currentBullets--;
            yield return new WaitForSeconds(1 / fireRate);

            if(currentBullets<=0)
            {
                armPivot.transform.localRotation = Quaternion.Euler(90, 0, 0);
                yield return new WaitForSeconds(reloadTime);
            }
            currentBullets = clipSize;
        }
    }

    public override void TakeFlash(float intensity, Collider collider)
    {
        StartCoroutine(TakeFlashC(intensity));
    }

    protected IEnumerator TakeFlashC(float intensity)
    {
        if (flashed) { yield break; }
        flashed = true;
        yield return new WaitForSeconds(intensity * flashedTime);
        flashed = false;
    }

    public override void TakeEMP(float intensity, Collider collider)
    {
        return;
    }

    public override void TakeStun(float intensity, Collider collider)
    {
        StartCoroutine(TakeStunC(intensity));
    }

    protected IEnumerator TakeStunC(float intensity)
    {
        if (stunned) { yield break; }
        stunned = true;
        yield return new WaitForSeconds(intensity * stunnedTime);
        stunned = false;
    }
}
