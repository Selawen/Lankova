using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    DamageHandler damageHandler;
    Rigidbody rb;

    #region field hell
    public PlayerDefaultValues defaultValues;

    [Header("Movement")]
    [HideInInspector]public bool grounded;
    [HideInInspector] public float moveSpeed, stepTime, normalPlayerRotationSpeed;
    [HideInInspector] public float[] legMoveSpeeds, legStepTimes;
    float playerRotationSpeed;
    public float PlayerRotationSpeed
    {
        get { return playerRotationSpeed; }
        set { playerRotationSpeed = value; GetComponentInChildren<MechTurning>().rotationSpeed = playerRotationSpeed; }
    }
    [HideInInspector] public float bopHeight, sprintMovementMultiplier;    
    bool moving;

    [Header("Shooting")]
    [HideInInspector] public float fireRate;
    [HideInInspector] public float bulletAmount, bulletMaxSpread, sprintSpreadMultiplier;
    [HideInInspector] public float recoilAmount, recoilTimeBack, recoilTimeForward;
    [HideInInspector] public int maxClipSize = 30;
    int currentClip;
    int CurrentClip { 
        get
        {
            return currentClip;
        }
        set 
        {
            currentClip = value;
            ammoCounter.text = currentClip.ToString();
        }
    }
    [HideInInspector] public float reloadTime;
    bool shooting;
    Vector3 gunBarrelOrig;
    public LayerMask gunPointerLayers;


    [Header("Objects")]
    public Transform bulletSpawn;
    public GameObject cameraMount, torso, gun, arm, muzzleFlash;
    public GameObject bulletPrefab;

    [Header("Audio")]
    public AudioSource[] legAudio;
    public AudioSource[] gunAudio;
    public AudioSource gunReload, gunCharge;
    public AudioClip misfire;
    int currentLeg = 0;

    [Header("UI Elements")]
    public Text ammoCounter;

    [Header("Debug")]
    public bool skipBootupAndTutorial = false;
    public GameObject testGrenade;

    [HideInInspector]
    public float[] barrelFireChances;
    public Vector3 currentTargetPoint;
    bool sprinting, canSprint;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadDefaults();

        Cursor.lockState = CursorLockMode.Confined;
        rb = GetComponent<Rigidbody>();
        barrelFireChances = new float[] { 1.0f, 1.0f, 1.0f };
        moving = false; shooting = false; sprinting = false; canSprint = false;
        PlayerRotationSpeed = normalPlayerRotationSpeed;

        GetComponentInChildren<MechTurning>(true).enabled = false;
        foreach (MouseLook ml in GetComponentsInChildren<MouseLook>(true))
        {
            ml.enabled = false;
        }

        //StartCoroutine(Movement());
        //StartCoroutine(Shoot());
        gunBarrelOrig = gun.transform.localPosition;
        CurrentClip = maxClipSize;       
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if(Time.timeScale <= 0.0f) { return; }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50, gunPointerLayers))
        {
            currentTargetPoint = hit.point;
        }
        else
        {
            currentTargetPoint = ray.GetPoint(50);
        }
    }

    void HandleInput()
    {
    }

    public void LoadDefaults()
    {
        //Movement
        moveSpeed = defaultValues.baseMoveSpeed;
        stepTime = defaultValues.baseStepTime;
        normalPlayerRotationSpeed = defaultValues.baseNormalRotationSpeed;
        System.Array.Copy(defaultValues.legMoveSpeeds, legMoveSpeeds, legMoveSpeeds.Length);
        System.Array.Copy(defaultValues.baseLegStepTimes, legStepTimes, legStepTimes.Length);
        bopHeight = defaultValues.baseBopHeight;
        sprintMovementMultiplier = defaultValues.baseSprintMultiplier;

        //Shooting
        fireRate = defaultValues.baseFireRate;
        bulletAmount = defaultValues.baseBulletAmount;
        bulletMaxSpread = defaultValues.baseBulletMaxSpread;
        sprintSpreadMultiplier = defaultValues.baseSprintSpreadMultiplier;
        recoilAmount = defaultValues.baseRecoilAmoint;
        recoilTimeBack = defaultValues.baseRecoilTimeBack;
        recoilTimeForward = defaultValues.baseRecoilTimeForward;
        reloadTime = defaultValues.baseReloadTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground")) { grounded = true; }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground")) { grounded = false; }
    }

    public void BootUp()
    {
        StartLooking(); StartMovement(); StartShooting(); StartSprinting();
    }

    public void StartLooking()
    {
        foreach (MouseLook ml in GetComponentsInChildren<MouseLook>(true))
        {
            ml.enabled = true;
        }
        GetComponentInChildren<MechTurning>().enabled = true;
    }    

    public void StartMovement()
    {
        StartCoroutine(Movement());
    }

    public void StartShooting()
    {
        StartCoroutine(Shoot());
    }

    public void StartSprinting()
    {
        canSprint = true;
    }

    IEnumerator Movement()
    {
        if (moving) { Debug.LogWarning("Tried to start Movement Coroutine when already active"); yield break; }

        moving = true;

        while(true)
        {
            sprinting = Input.GetKey(KeyCode.LeftShift) && canSprint;
            while (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            {
                rb.velocity = Vector3.zero;
                yield return null;
            }

            Vector3 direction = Vector3.zero;


            //Input keys in reversed order of priority. I'm lazy okay.
            if(Input.GetKey(KeyCode.A))
            {
                direction = -transform.right;
            }
            if(Input.GetKey(KeyCode.D))
            {
                direction = transform.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction = -transform.forward;
            }
            if (Input.GetKey(KeyCode.W))
            {
                direction = transform.forward;
            }

            Vector3 torsoOrig = torso.transform.localPosition;            
            Vector3 cameraOrig = cameraMount.transform.localPosition;
            Vector3 torsoTarget = torso.transform.localPosition - Vector3.up * bopHeight;
            Vector3 cameraTarget = cameraMount.transform.localPosition - Vector3.up * bopHeight;

            if (sprinting)
            {
                torsoTarget = torso.transform.localPosition - Vector3.up * bopHeight*2;
                cameraTarget = cameraMount.transform.localPosition - Vector3.up * bopHeight*2;
            }


            currentLeg++;
            currentLeg %= 2;

            float t = 0.0f;
            while(t <= 1.0f)
            {
                cameraMount.transform.localPosition = Vector3.Lerp(cameraOrig, cameraTarget, t);
                torso.transform.localPosition = Vector3.Lerp(torsoOrig, torsoTarget, t);

                if (sprinting) 
                { 
                    rb.velocity = direction * Mathf.Lerp(0.4f * legMoveSpeeds[currentLeg], legMoveSpeeds[currentLeg] * sprintMovementMultiplier, t);
                    t += Time.deltaTime / (legStepTimes[currentLeg] * 2) * sprintMovementMultiplier;
                }
                else 
                { 
                    rb.velocity = direction * Mathf.Lerp(0.4f * legMoveSpeeds[currentLeg], legMoveSpeeds[currentLeg], t);
                    t += Time.deltaTime / (legStepTimes[currentLeg] * 2);
                }        
                yield return null;
            }

            legAudio[currentLeg].Play();
            t = 0.0f;
            while (t <= 1.0f)
            {
                cameraMount.transform.localPosition = Vector3.Lerp(cameraTarget, cameraOrig, t);
                torso.transform.localPosition = Vector3.Lerp(torsoTarget, torsoOrig, t);

                if (sprinting)
                { 
                    rb.velocity = direction * Mathf.Lerp(legMoveSpeeds[currentLeg] * sprintMovementMultiplier, 0.4f * legMoveSpeeds[currentLeg], t);
                    t += Time.deltaTime / (legStepTimes[currentLeg] * 2) * sprintMovementMultiplier;
                }
                else
                { 
                    rb.velocity = direction * Mathf.Lerp(legMoveSpeeds[currentLeg], 0.4f * legMoveSpeeds[currentLeg], t);
                    t += Time.deltaTime / (legStepTimes[currentLeg] * 2);
                }     

                yield return null;
            }
            torso.transform.localPosition = torsoOrig;
            cameraMount.transform.localPosition = cameraOrig;

        }
    }

    IEnumerator Shoot()
    {
        if (shooting) { Debug.LogWarning("Attempted to start Shoot coroutine while already running"); yield break; }
        shooting = true;

        int currentBarrel = 0;

        while(true)
        {
            while(!Input.GetMouseButton(0) || Time.timeScale <= 0.0f)
            {
                yield return null;
            }

            if (Random.Range(0.0f, 0.99f) < barrelFireChances[currentBarrel])
            {
                //RaycastHit hit;
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(ray, out hit, 50, gunPointerLayers))
                //{
                //    bulletSpawn.transform.LookAt(hit.point);
                //}
                //else
                //{
                //    bulletSpawn.transform.LookAt(ray.GetPoint(50));
                //}
                bulletSpawn.transform.LookAt(currentTargetPoint);

                for (int i = 0; i < bulletAmount; i++)
                {
                    
                    float x = Random.Range(-bulletMaxSpread, bulletMaxSpread);
                    float y = Random.Range(-bulletMaxSpread, bulletMaxSpread);
                    if (sprinting) { x *= Random.Range(1, sprintSpreadMultiplier); y *= Random.Range(1, sprintSpreadMultiplier); }

                    Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation((bulletSpawn.forward + bulletSpawn.right * x + bulletSpawn.up * y).normalized));
                }
                gunAudio[currentBarrel].Play();
                StartCoroutine(GunRecoil());
            }
            else { gunAudio[currentBarrel].PlayOneShot(misfire); }

            float t = 0.0f;
            int nextBarrel = (currentBarrel + 1) % 3;

            Quaternion targetRotation = Quaternion.Euler(gun.transform.localRotation.eulerAngles.x,
                    gun.transform.localRotation.eulerAngles.y, (nextBarrel * 120));

            Quaternion currentRotation = gun.transform.localRotation;

            CurrentClip--;

            while (t <= 1.0f)
            {
                gun.transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, t);
                t += Time.deltaTime * fireRate;
                yield return null;
            }
            gun.transform.localRotation = targetRotation;
            currentBarrel = nextBarrel;

            if(CurrentClip <= 0)
            {
                gunReload.Play();
                gunCharge.PlayDelayed(reloadTime - gunCharge.clip.length);

                float rt = 0.0f;
                while(rt <= 1.0f)
                {
                    CurrentClip = (int)Mathf.Lerp(0, maxClipSize, rt);
                    rt += Time.deltaTime / reloadTime;
                    yield return null;
                }
                CurrentClip = maxClipSize;
            }
        }    
    }

    IEnumerator GunRecoil()
    {
        float t = 0;
        muzzleFlash.SetActive(true);
        while(t<=1.0f)
        {
            t += Time.deltaTime / (recoilTimeBack);
            gun.transform.localPosition = gunBarrelOrig - Vector3.forward * Mathf.Lerp(0, recoilAmount, t);
            yield return null;
        }
        muzzleFlash.SetActive(false);
        t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / (recoilTimeForward);
            gun.transform.localPosition = gunBarrelOrig - Vector3.forward * Mathf.Lerp(recoilAmount, 0, t);
            yield return null;
        }
        gun.transform.localPosition = gunBarrelOrig;
    }

    
}
