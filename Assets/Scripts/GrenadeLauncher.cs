using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeLauncher : MonoBehaviour
{
    enum GrenadeType { None = -1, Frag = 0, Flash = 1, Smoke = 2, AP = 3}

    public PlayerDefaultValues defaultValues;

    [Header("Game Objects")]
    public GameObject launcherPivot;
    public Transform grenadeSpawn, inBarrel;
    public Transform[] magBarrels;
    public GameObject drumMag;
    public GameObject[] grenadePrefabs;
    public GameObject[] dummies;

    [Header("Stats")]
    [HideInInspector] public float projectileHeight = 0.1f;
    [HideInInspector] public float barrelRotateTime = 0.25f, unloadTime = 0.5f, loadTime = 0.5f;
    [HideInInspector] public float regenerationTime = 30.0f;
    [HideInInspector] public int[] grenadeMaxAmmo, grenadeCurrentAmmo;
    [HideInInspector] public float maxScatter;
    [HideInInspector] public bool regenActive = true;

    [Header("UI Elements")]
    public GameObject[] currentGrenadeIcons;
    public Text[] ammoCounterTexts;
    public Image[] regenBars;

    [Header("Audio")]
    public AudioSource fireSound;
    public AudioSource rotateSound, loadSound, regenSound;

    Queue<GrenadeType> regeneratorQueue;
    bool isDrumRotating, isLoading, bootedUp;
    int currentBarrel;
    GrenadeType currentGrenade;
    GrenadeType CurrentGrenade
    {
        get { return currentGrenade; }
        set 
        { 
            currentGrenade = value;

            for (int i = 0; i < currentGrenadeIcons.Length; i++)
            {
                currentGrenadeIcons[i].SetActive((int)currentGrenade == i);
            }
        }
    }
    GameObject currentDummy;

    PlayerMovement player;
    DamageHandler dh;
    private void Start()
    {
        LoadSettings();

        player = GetComponent<PlayerMovement>();
        dh = GetComponent<DamageHandler>();
        currentBarrel = 0;
        isDrumRotating = false; isLoading = false;
        CurrentGrenade = GrenadeType.None;
        currentDummy = null;

        //StartCoroutine(Regenerate());
        UpdateAmmoCounters();
        UpdateDummyVisibility();
    }

    private void Update()
    {
        if (Time.timeScale <= 0.0f) { return; }

        if (!bootedUp) { return; }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { StartCoroutine(LoadCycle(GrenadeType.Frag)); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { StartCoroutine(LoadCycle(GrenadeType.Flash)); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { StartCoroutine(LoadCycle(GrenadeType.Smoke)); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { StartCoroutine(LoadCycle(GrenadeType.AP)); }
        
        if(Input.GetMouseButtonDown(1) && !isLoading)
        {
            FireGrenade();
        }
    }

    private void FixedUpdate()
    {
        //Aim barrel
        if (!bootedUp) { return; }
        if (Time.timeScale <= 0.0f) { return; }

        AimBarrel();
    }

    public void LoadSettings()
    {
        projectileHeight = defaultValues.baseProjectileHeight;
        barrelRotateTime = defaultValues.baseBarrelRotateTime;
        unloadTime = defaultValues.baseUnloadTime;
        loadTime = defaultValues.baseLoadTime;
        regenerationTime = defaultValues.baseRegenerationTime;
        System.Array.Copy(defaultValues.baseGrenadeMaxAmmo, grenadeMaxAmmo, grenadeMaxAmmo.Length);
        maxScatter = defaultValues.baseMaxScatter;
        regenActive = defaultValues.baseRegenActive;
    }

    public void BootUp()
    {
        StartCoroutine(Regenerate());
        bootedUp = true;
    }

    void AimBarrel()
    {
        Vector3 origin = launcherPivot.transform.position;
        Vector3 target = player.currentTargetPoint;

        float distance = Vector3.Distance(origin, target);
        float height = projectileHeight * distance;

        Vector3 control = origin + (target - origin) / 2 + Vector3.up * height;
        launcherPivot.transform.LookAt(control);
    }

    void FireGrenade()
    {
        if (Time.timeScale <= 0.0f) { return; }

        if (isLoading) { return; }
        if (CurrentGrenade == GrenadeType.None) { return; }
        //if(grenadeCurrentAmmo[(int)CurrentGrenade] <= 0) { return; } //Should not be necessary because it won't load a non-existent grenade

        currentDummy.transform.parent = magBarrels[(int)CurrentGrenade];
        currentDummy.transform.localPosition = Vector3.zero;

        Vector3 spread = new Vector3(Random.Range(-maxScatter, maxScatter), Random.Range(-maxScatter, maxScatter), Random.Range(-maxScatter, maxScatter));

        ArcingProjectile nade = 
            Instantiate(GrenadeTypeToPrefab(CurrentGrenade), grenadeSpawn.transform.position, grenadeSpawn.transform.rotation).GetComponent<ArcingProjectile>();
        nade.Init(grenadeSpawn.transform.position, player.currentTargetPoint + spread, projectileHeight);

        regeneratorQueue.Enqueue(CurrentGrenade);
        currentDummy = null;
        CurrentGrenade = GrenadeType.None;
        fireSound.Play();
        UpdateDummyVisibility();
        UpdateAmmoCounters();
    }

    #region loadCycle
    IEnumerator LoadCycle(GrenadeType newGrenade)
    {
        if (isLoading) { yield break; }
        if(newGrenade == GrenadeType.None) { Debug.LogError("Can't load an empty grenade you silly piece of shit"); }
        isLoading = true;
        bool loadNew = newGrenade != CurrentGrenade;

        if(CurrentGrenade != GrenadeType.None)
        {
            yield return StartCoroutine(UnloadGrenade());
        }

        if (!loadNew)
        { isLoading = false; yield break; }

        yield return StartCoroutine(LoadGrenade(newGrenade));

        isLoading = false;
    }

    IEnumerator UnloadGrenade()
    {
        if (!isLoading) { Debug.LogWarning("Unload Coroutine should never be called from anywhere except the loadcycle"); }
        if(CurrentGrenade == GrenadeType.None || currentDummy == null) { Debug.LogError("Can't unload an empty grenade"); }
        
        yield return StartCoroutine(RotateDrum(CurrentGrenade));
        currentDummy.transform.parent = magBarrels[(int)CurrentGrenade];
        Vector3 startPos = currentDummy.transform.localPosition;
        GrenadeType loadedGrenade = CurrentGrenade;
        CurrentGrenade = GrenadeType.None;

        loadSound.Play();

        float t = 0.0f;
        while(t<=1.0f)
        {
            t += Time.deltaTime / unloadTime;
            currentDummy.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, t);
            yield return null;
        }

        grenadeCurrentAmmo[(int)loadedGrenade]++;
        UpdateAmmoCounters();

        currentDummy.transform.localPosition = Vector3.zero;
        currentDummy = null;
        
    }

    IEnumerator LoadGrenade(GrenadeType newGrenade)
    {
        if (!isLoading) { Debug.LogWarning("Load Coroutine should never be called from anywhere except the loadcycle"); }
        

        yield return StartCoroutine(RotateDrum(newGrenade));
        if(grenadeCurrentAmmo[(int)newGrenade] <= 0) { yield break; }

        currentDummy = dummies[(int)newGrenade];
        currentDummy.transform.parent = inBarrel;
        grenadeCurrentAmmo[(int)newGrenade]--;
        UpdateAmmoCounters();

        Vector3 startPos = currentDummy.transform.localPosition;
        loadSound.Play();

        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / loadTime;
            currentDummy.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, t);
            yield return null;
        }

        currentDummy.transform.localPosition = Vector3.zero;
        CurrentGrenade = newGrenade;
    }

    IEnumerator RotateDrum(GrenadeType gt)
    {
        if (isDrumRotating || gt == GrenadeType.None) { yield break; }
        isDrumRotating = true;

        for (; currentBarrel != (int)gt; currentBarrel++, currentBarrel%=grenadePrefabs.Length)
        {
            Quaternion orig = drumMag.transform.localRotation;
            Quaternion target = Quaternion.Euler(orig.eulerAngles.x, orig.eulerAngles.y, orig.eulerAngles.z - (360/grenadePrefabs.Length));
            float t = 0.0f;
            rotateSound.Play();

            while (t<=1.0f)
            {
                t += Time.deltaTime / barrelRotateTime;
                drumMag.transform.localRotation = Quaternion.Lerp(orig, target, t);
                yield return null;
            }

            drumMag.transform.localRotation = target;
        }

        isDrumRotating = false;
    }

    #endregion

    IEnumerator Regenerate()
    {
        regeneratorQueue = new Queue<GrenadeType>();
        while(true)
        {
            while(regeneratorQueue.Count <= 0) { yield return new WaitForSeconds(1.0f); }

            GrenadeType currentlyGenerating = regeneratorQueue.Dequeue();
            if(currentlyGenerating == GrenadeType.None) { Debug.LogError("Although it sounds badass as all hell, cannot create null grenades"); }
            if(grenadeCurrentAmmo[(int)currentlyGenerating] >= grenadeMaxAmmo[(int)currentlyGenerating]) { continue; }

            RectTransform regBar = regenBars[(int)currentlyGenerating].rectTransform;
            Vector3 origScale = regBar.localScale;
                
            regenBars[(int)currentlyGenerating].gameObject.SetActive(true);
            float t = 0.0f;
            while(t<=1.0f)
            {
                if (regenActive)
                {
                    t += Time.deltaTime / regenerationTime;
                }
                regBar.localScale = Vector3.Scale(new Vector3(1.0f, t, 1.0f), origScale);
                yield return null;
            }

            grenadeCurrentAmmo[(int)currentlyGenerating] = 
                Mathf.Min(grenadeCurrentAmmo[(int)currentlyGenerating] + 1, grenadeMaxAmmo[(int)currentlyGenerating]);
            Debug.Log("Regenerated a " + currentlyGenerating.ToString());
            regenBars[(int)currentlyGenerating].gameObject.SetActive(false);
            regenSound.Play();
            UpdateDummyVisibility();
            UpdateAmmoCounters();
        }
    }

    void UpdateDummyVisibility()
    {
        for (int i = 0; i < dummies.Length; i++)
        {
            dummies[i].SetActive(grenadeCurrentAmmo[i] > 0);
        }
    }

    void UpdateAmmoCounters()
    {
        for (int i = 0; i < ammoCounterTexts.Length; i++)
        {
            ammoCounterTexts[i].text = grenadeCurrentAmmo[i].ToString();
        }
    }

    GameObject GrenadeTypeToPrefab(GrenadeType gt)
    {
        return GrenadeTypeToPrefab((int)gt);
    }

    public GameObject GrenadeTypeToPrefab(int gt)
    {
        if (gt < 0 || gt >= grenadePrefabs.Length) { return null; }
        return grenadePrefabs[gt];
    }

    public void FillGrenades()
    {
        for (int i = 0; i < grenadeMaxAmmo.Length; i++)
        {
            FillGrenades(i);
        }
        regeneratorQueue.Clear();
    }

    public void FillGrenades(int i)
    {
        grenadeCurrentAmmo[i] = grenadeMaxAmmo[i];

        UpdateAmmoCounters();
        UpdateDummyVisibility();
    }
}
