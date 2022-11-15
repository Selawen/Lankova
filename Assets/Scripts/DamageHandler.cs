using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageHandler : MonoBehaviour, IDamagable
{
    public PlayerDefaultValues defaultValues;

    PlayerMovement player;
    GrenadeLauncher launcher;
    public GameObject deathPanel;

    #region STATS
    [Header("Stats")]
    [HideInInspector] public float shieldMax = 100;
    [HideInInspector] public float minimumShieldRechargeAmount = 5, shieldRechargePenalty = 10;
    [HideInInspector] public float torsoArmor = 3;
    float currentShieldMax;
    float currentShield;
    public float CurrentShield
    {
        get
        {
            return currentShield;
        }
        set
        {
            if (value < currentShield) { StartCoroutine(FlashShieldOutline(damagedColor)); }
            currentShield = Mathf.Clamp(value, 0, currentShieldMax);
            float shieldIncrement = shieldMax / shieldBars.Length;
            int currentBar = Mathf.Min((int)(currentShield / shieldIncrement), 4);

            if (!bootedUp) { return; }

            for (int i = 0; i < 5; i++)
            {
                shieldBars[i].gameObject.SetActive(i * shieldIncrement < currentShield);
                if (i == currentBar)
                {
                    shieldBars[i].color = Color.Lerp(shieldHealthyColor, damagedColor, 1.0f - ((int)(currentShield - 1) % (int)shieldIncrement) / shieldIncrement);
                }
                else { shieldBars[i].color = shieldHealthyColor; }
            }
            if (currentShield <= shieldMax / 4 && !shieldWasLow) { shieldLowSound.Play(); }
            if (currentShield <= 0 && !shieldWasDown) { shieldDownSound.Play(); }

            shieldExclamationPoint.gameObject.SetActive(currentShield <= 0);
            shieldsDown.gameObject.SetActive(currentShield <= 0);
            shieldsLow.gameObject.SetActive(currentShield > 0 && currentShield <= shieldMax / 4);
            shieldWasLow = currentShield <= shieldMax / 4;
            shieldWasDown = currentShield <= 0;
        }
    }

    [HideInInspector] public float maxTorsoHealth = 200, maxLegHealth = 100, maxArmHealth = 100, maxLauncherHealth = 100;
    [HideInInspector] public float empTime;

    float currentTorsoHealth;
    float currentArmHealth;
    float currentLeftLegHealth;
    float currentRightLegHealth;
    float currentLauncherHealth;
    #endregion

    #region GAMEOBJECTSETC
    [Header("UI Elements")]
    public Image shieldExclamationPoint;
    public Image shieldOutline;
    public Image[] shieldBars;
    public GameObject shieldRechargeIcon;
    public Image torsoIndicator, armIndicator, leftLegIndicator, rightLegIndicator, launcherIndicator;
    public GameObject leftCanvasBackground, middleCanvasBackground, rightCanvasBackground;
    public Text statusEffectsText;
    public GameObject shieldsLow, shieldsDown, reactorMeltdown;
    public FlashbangUI flashUI;
    public GameObject[] systemDamageText;

    [Header("Colors")]
    public Color damagedColor;
    public Color componentHealthyColor, shieldHealthyColor;
    public Color shieldOutlineColor;

    Color currentTorsoColor, currentArmColor, currentLeftLegColor, currentRightLegColor, currentLauncherColor;

    [Header("Colliders")]
    public Collider torsoCollider;
    public Collider leftLegCollider, rightLegCollider, armCollider, launcherCollider;

    [Header("MechComponents")]
    public Light leftLight;
    public Light rightLight;
    public GameObject leftLightObject, rightLightObject;
    public GameObject[] EMPable;

    [Header("AudioSources")]
    public AudioSource systemDamage;
    public AudioSource warningSiren, shieldLowSound, shieldDownSound, shieldRechargeSound;
    #endregion

    [Header("Other")]
    public float flickerMinTime = 1.5f;
    public float flickerMaxTime = 3.5f;

    //Weird status effect bool arrays here
    int[] torsoStatusEffects, armStatusEffects, launcherStatusEffects;
    int[][] legStatusEffects;

    List<string> statusEffects;
    bool shieldFlashing = false, torsoFlashing = false, armFlashing = false, leftLegFlashing = false,
        rightLegFlashing = false, launcherFlashing = false, EMPd = false, bootedUp = false;
    bool shieldWasDown, shieldWasLow;
    float timeSinceLastDamaged;

    Coroutine lightLeftFlickerRoutine, lightRightFlickerRoutine, uiLeftFlickerRoutine, uiRightFlickerRoutine, uiMiddleFlickerRoutine;

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();

        bootedUp = false;
        player = GetComponent<PlayerMovement>();
        launcher = GetComponent<GrenadeLauncher>();
        currentShieldMax = shieldMax;
        CurrentShield = shieldMax;
        currentTorsoHealth = maxTorsoHealth;
        currentLeftLegHealth = maxLegHealth;
        currentRightLegHealth = maxLegHealth;
        currentArmHealth = maxArmHealth;
        currentLauncherHealth = maxLauncherHealth;
        statusEffects = new List<string>();
        UpdateStatusEffects();

        torsoStatusEffects = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        armStatusEffects = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        launcherStatusEffects = new int[] { 0, 0, 0, 0, 0, 0 };
        legStatusEffects = new int[][] { new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 } };

        StartCoroutine(ShieldRecharge());

    }

    public void BootUp()
    {
        bootedUp = true;
        CurrentShield = CurrentShield;
    }

    private void Update()
    {
        timeSinceLastDamaged += Time.deltaTime;
        //DebugUpdate();
    }

    void DebugUpdate()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            HalveMoveSpeed(0); UpdateDamageEffects();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RepairMoveSpeed(0); UpdateDamageEffects();
        }
    }

    void LoadSettings()
    {
        shieldMax = defaultValues.baseShieldMax;
        minimumShieldRechargeAmount = defaultValues.baseMinimumShieldRechargeAmount;
        shieldRechargePenalty = defaultValues.baseShieldRechargePenalty;
        torsoArmor = defaultValues.baseTorsoArmor;
        maxTorsoHealth = defaultValues.baseMaxTorsoHealth;
        maxLegHealth = defaultValues.baseMaxLegHealth;
        maxArmHealth = defaultValues.baseMaxArmHealth;
        maxLauncherHealth = defaultValues.baseMaxLauncherHealth;
        empTime = defaultValues.baseEMPTime;
    }

    IEnumerator ShieldRecharge()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (CurrentShield == currentShieldMax || currentShieldMax <= 0 || timeSinceLastDamaged < shieldRechargePenalty)
            { shieldRechargeIcon.SetActive(false); continue; }
            shieldRechargeIcon.SetActive(true);
            StartCoroutine(FlashShieldOutline(componentHealthyColor));
            if (!shieldRechargeSound.isPlaying) { shieldRechargeSound.Play(); }
            CurrentShield += Mathf.Max(timeSinceLastDamaged - shieldRechargePenalty, minimumShieldRechargeAmount);
        }
    }

    void UpdateDamageEffects()
    {
        #region TORSO
        if(torsoStatusEffects[0] > 0)
        {
            if(uiLeftFlickerRoutine == null) { uiLeftFlickerRoutine = StartCoroutine(Flicker(leftCanvasBackground)); }
        }
        else
        {
            if(uiLeftFlickerRoutine != null) { StopCoroutine(uiLeftFlickerRoutine); uiLeftFlickerRoutine = null; StopFlicker(leftCanvasBackground); }
        }
        
        if(torsoStatusEffects[1] > 0)
        {
            if(uiMiddleFlickerRoutine == null) { uiMiddleFlickerRoutine = StartCoroutine(Flicker(middleCanvasBackground)); }
        }
        else
        {
            if(uiMiddleFlickerRoutine != null) { StopCoroutine(uiMiddleFlickerRoutine); uiMiddleFlickerRoutine = null; StopFlicker(middleCanvasBackground); }
        }        
        
        if(torsoStatusEffects[2] > 0)
        {
            if(uiRightFlickerRoutine == null) { uiRightFlickerRoutine = StartCoroutine(Flicker(rightCanvasBackground)); }
        }
        else
        {
            if(uiRightFlickerRoutine != null) { StopCoroutine(uiRightFlickerRoutine); uiRightFlickerRoutine = null; StopFlicker(rightCanvasBackground); }
        }
        
        if (torsoStatusEffects[3] > 0)
        {
            if (lightLeftFlickerRoutine == null) { lightLeftFlickerRoutine = StartCoroutine(Flicker(leftLight)); }
        }
        else
        {
            if (lightLeftFlickerRoutine != null) { StopCoroutine(lightLeftFlickerRoutine); lightLeftFlickerRoutine = null; StopFlicker(leftLight); }
        }

        if (torsoStatusEffects[4] > 0)
        {
            if (lightRightFlickerRoutine == null) { lightRightFlickerRoutine = StartCoroutine(Flicker(rightLight)); }
        }
        else
        {
            if (lightRightFlickerRoutine != null) { StopCoroutine(lightRightFlickerRoutine); lightRightFlickerRoutine = null; StopFlicker(rightLight); }
        }

        leftLightObject.SetActive(torsoStatusEffects[5] == 0);
        rightLightObject.SetActive(torsoStatusEffects[6] == 0);

        currentShieldMax = Mathf.Max(defaultValues.baseShieldMax * (1.0f - (torsoStatusEffects[7] * 0.25f)), 0);

        torsoArmor = defaultValues.baseTorsoArmor - torsoStatusEffects[9];
        #endregion

        #region ARM
        for (int i = 0; i < 3; i++)
        {
            switch (armStatusEffects[i])
            {
                case 0: player.barrelFireChances[i] = 1.0f; break;
                case 1: player.barrelFireChances[i] = 0.75f; break;
                case 2: player.barrelFireChances[i] = 0.25f; break;
                case 3: player.barrelFireChances[i] = 0.0f; break;
                default: Debug.LogError("WTF"); break;
            }
        }

        //player.fireRate = defaultValues.baseFireRate / (2 * armStatusEffects[3]);
        player.fireRate = defaultValues.baseFireRate / Mathf.Pow(2, armStatusEffects[3]);

        switch (armStatusEffects[4])
        {
            case 0: player.reloadTime = defaultValues.baseReloadTime; break;
            case 1: player.reloadTime = defaultValues.baseReloadTime + 0.25f; break;
            case 2: player.reloadTime = defaultValues.baseReloadTime + 0.75f; break;
            case 3: player.reloadTime = defaultValues.baseReloadTime + 1.5f; break;
            default: Debug.LogError("WTF"); break;
        }

        player.bulletMaxSpread = defaultValues.baseBulletMaxSpread + (defaultValues.baseBulletMaxSpread * armStatusEffects[4]);

        switch (armStatusEffects[6])
        {
            case 0: player.maxClipSize = defaultValues.baseMaxClipSize; break;
            case 1: player.maxClipSize = defaultValues.baseMaxClipSize / 2; break;
            case 2: player.maxClipSize = defaultValues.baseMaxClipSize / 4; break;
            default: Debug.LogError("WTF"); break;
        }
        #endregion

        #region LEGS

        for (int leg = 0; leg < 2; leg++)
        {
            player.legMoveSpeeds[leg] = defaultValues.baseLegMoveSpeeds[leg] / Mathf.Pow(1.5f, legStatusEffects[leg][0]);
            if (legStatusEffects[leg][0] >= 3) { player.legMoveSpeeds[leg] = 0; }

            player.legStepTimes[leg] = defaultValues.baseLegStepTimes[leg] * Mathf.Pow(1.5f, legStatusEffects[leg][1]);
        }

        float newRotSpd = defaultValues.baseNormalRotationSpeed;
        if (legStatusEffects[0][2] > 0) { newRotSpd /= 2; }
        if (legStatusEffects[1][2] > 0) { newRotSpd /= 2; }
        player.PlayerRotationSpeed = newRotSpd;

        #endregion

        #region LAUNCHER

        launcher.barrelRotateTime = defaultValues.baseBarrelRotateTime * Mathf.Pow(2, launcherStatusEffects[0]);
        launcher.unloadTime = defaultValues.baseUnloadTime * Mathf.Pow(2, launcherStatusEffects[1]);
        launcher.loadTime = defaultValues.baseLoadTime * Mathf.Pow(2, launcherStatusEffects[2]);
        launcher.maxScatter = defaultValues.baseMaxScatter + (0.5f * launcherStatusEffects[3]);
        launcher.projectileHeight = defaultValues.baseProjectileHeight * Mathf.Pow(1.5f, launcherStatusEffects[4]);
        launcher.regenerationTime = defaultValues.baseRegenerationTime * Mathf.Max(1.0f, 1.5f * launcherStatusEffects[5]);

        launcher.regenActive = launcherStatusEffects[5] >= 3;
        #endregion
    }

    #region limbDamageHandlers

    #region torso
    void HandleTorsoDamage(float damage)
    {
        damage = Mathf.Max(1.0f, damage - 2.0f);
        currentTorsoHealth = Mathf.Clamp(currentTorsoHealth - damage, 0, maxTorsoHealth);
        //torsoIndicator.color = Color.Lerp(damagedColor, componentHealthyColor, currentTorsoHealth / maxTorsoHealth);
        currentTorsoColor = Color.Lerp(damagedColor, componentHealthyColor, currentTorsoHealth / maxTorsoHealth);
        StartCoroutine(FlashTorsoIndicator(damagedColor));
        if (Random.Range(0, maxTorsoHealth - 1) > currentTorsoHealth)
        {
            float randomRoll = Random.Range(0, 100) + Mathf.Min(currentTorsoHealth, 100);
            //Fancy Damage Effects go Here;
            if (randomRoll > 145) { LowerArmor(); UpdateDamageEffects(); return; }
            if (randomRoll > 130) { FlickerLeftLight(); UpdateDamageEffects(); return; }
            if (randomRoll > 115) { FlickerRightLight(); UpdateDamageEffects(); return; }
            if (randomRoll > 95) { LowerShieldMax(); UpdateDamageEffects(); return; }
            if (randomRoll > 85) { FlickerRight(); UpdateDamageEffects(); return; }
            if (randomRoll > 75) { LowerShieldMax(); UpdateDamageEffects(); return; }
            if (randomRoll > 65) { FlickerMiddle(); UpdateDamageEffects(); return; }
            if (randomRoll > 60) { DisableLeftLight(); UpdateDamageEffects(); return; }
            if (randomRoll > 55) { DisableRightLight(); UpdateDamageEffects(); return; }
            if (randomRoll > 40) { LowerArmor(); UpdateDamageEffects(); return; }
            if (randomRoll > 30) { FlickerLeft(); UpdateDamageEffects(); return; }
            if (randomRoll > 20) { LowerShieldMax(); UpdateDamageEffects(); return; }
            ReactorMeltdown();
        }
    }

    void LowerArmor()
    {
        if (torsoStatusEffects[9] > defaultValues.baseTorsoArmor) { return; }

        //if(torsoArmor <= 0) { torsoStatusEffects[9] = 1; }
        torsoStatusEffects[9]++;
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        switch (torsoArmor)
        {
            case 2: AddStatusEffect("Torso armor punctured"); break;
            case 1: AddStatusEffect("Torso armor severely damaged"); break;
            case 0: AddStatusEffect("Torso armor broken"); break;
            default: AddStatusEffect("Torso armor damaged"); break;
        }
    }

    void FlickerLeft()
    {
        if (torsoStatusEffects[0] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[0]++;
        AddStatusEffect("Status HUD Damaged");
    }

    void FlickerMiddle()
    {
        if (torsoStatusEffects[1] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[1]++;
        AddStatusEffect("Center HUD Damaged");
    }

    void FlickerRight()
    {
        if (torsoStatusEffects[2] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[2]++;
        AddStatusEffect("Weapon HUD Damaged");
    }

    void FlickerLeftLight()
    {
        if (torsoStatusEffects[3] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[3]++;

        AddStatusEffect("Left Spotlight Damaged");
    }

    void FlickerRightLight()
    {
        if (torsoStatusEffects[4] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[4]++;

        AddStatusEffect("Right Spotlight Damaged");
    }

    void DisableLeftLight()
    {
        if (torsoStatusEffects[5] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[5]++;
        torsoStatusEffects[3]++;

        AddStatusEffect("Left Spotlight Destroyed");
    }

    void DisableRightLight()
    {
        if (torsoStatusEffects[6] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[6]++;
        torsoStatusEffects[4]++;

        AddStatusEffect("Right Spotlight Destroyed");
    }

    void LowerShieldMax()
    {
        if (torsoStatusEffects[7] > 3) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[7]++;

        switch (torsoStatusEffects[7])
        {
            case 0: Debug.LogError("WTF"); break;
            case 1: AddStatusEffect("Shield capacitor leaking"); break;
            case 2: AddStatusEffect("Shield matrix misaligned"); break;
            case 3: AddStatusEffect("Shields cascading"); break;
            case 4: AddStatusEffect("Shields destroyed"); break;
            default: Debug.LogError("WTF"); break;
        }
        CurrentShield = CurrentShield; //LMAO (this actually makes sense, it makes sure the shield is below the new maximum

    }

    void ReactorMeltdown()
    {
        if (torsoStatusEffects[8] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        torsoStatusEffects[8]++;
        AddStatusEffect("Reactor Meltdown");
        Debug.Log("Reactor Meltdown. Game Over.");
        StartCoroutine(Die());
    }

    #endregion

    #region arm
    void HandleArmDamage(float damage)
    {
        currentArmHealth = Mathf.Clamp(currentArmHealth - damage, 0, maxArmHealth);
        currentArmColor = Color.Lerp(damagedColor, componentHealthyColor, currentArmHealth / maxArmHealth);
        StartCoroutine(FlashArmIndicator(damagedColor));
        if (Random.Range(0, maxArmHealth - 1) > currentArmHealth)
        {
            if (!systemDamage.isPlaying) { systemDamage.Play(); }
            float randomRoll = Random.Range(0, 100) + (currentArmHealth / maxArmHealth * 100);

            if (randomRoll > 90) { IncreaseReloadTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 85) { DamageBarrel(); UpdateDamageEffects(); return; }
            if (randomRoll > 75) { HalveClipSize(); UpdateDamageEffects(); return; }
            if (randomRoll > 65) { HalveFireRate(); UpdateDamageEffects(); return; }
            if (randomRoll > 55) { DamageBarrel(); UpdateDamageEffects(); return; }
            if (randomRoll > 45) { IncreaseBulletSpread(); UpdateDamageEffects(); return; }
            if (randomRoll > 20) { HalveFireRate(); UpdateDamageEffects(); return; }
            DamageBarrel(); UpdateDamageEffects();
        }
    }

    void DamageBarrel()
    {
        int barrel = Random.Range(0, 3);
        if (armStatusEffects[barrel] > 2) { return; }
        armStatusEffects[barrel]++;

        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        switch (barrel)
        {
            case 0:
                if (armStatusEffects[barrel] == 1) { AddStatusEffect("Barrel 1 warped"); };
                if (armStatusEffects[barrel] == 2) { AddStatusEffect("Barrel 1 severely damaged"); };
                if (armStatusEffects[barrel] == 3) { AddStatusEffect("Barrel 1 destroyed"); };
                break;
            case 1:
                if (armStatusEffects[barrel] == 1) { AddStatusEffect("Barrel 2 warped"); };
                if (armStatusEffects[barrel] == 2) { AddStatusEffect("Barrel 2 severely damaged"); };
                if (armStatusEffects[barrel] == 3) { AddStatusEffect("Barrel 2 destroyed"); };
                break;
            case 2:
                if (armStatusEffects[barrel] == 1) { AddStatusEffect("Barrel 3 warped"); };
                if (armStatusEffects[barrel] == 2) { AddStatusEffect("Barrel 3 severely damaged"); };
                if (armStatusEffects[barrel] == 3) { AddStatusEffect("Barrel 3 destroyed"); };
                break;
            default: Debug.LogError("There is no barrel 4. Fuck you."); break;
        }
    }

    void HalveFireRate()
    {
        if (armStatusEffects[3] > 1) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        armStatusEffects[3]++;

        switch (armStatusEffects[3])
        {
            case 1: AddStatusEffect("Gun motor snagging"); break;
            case 2: AddStatusEffect("Firing mechanism damaged"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void IncreaseReloadTime()
    {
        if (armStatusEffects[4] > 2) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        armStatusEffects[4]++;

        switch (armStatusEffects[4])
        {
            case 1: AddStatusEffect("Magazine holder misaligned"); break;
            case 2: AddStatusEffect("Reload coroutine lagging"); break;
            case 3: AddStatusEffect("Clip ejector jammed"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void IncreaseBulletSpread()
    {
        if (armStatusEffects[5] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        armStatusEffects[5]++;

        AddStatusEffect("Targeting mechanisms failing");
    }

    void HalveClipSize()
    {
        if (armStatusEffects[6] > 1) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        armStatusEffects[6]++;

        switch (armStatusEffects[6])
        {
            case 1: AddStatusEffect("Secondary clip feed broken"); break;
            case 2: AddStatusEffect("Clip feed jammed"); break;
            default: Debug.LogError("WTF"); break;
        }
    }



    #endregion

    #region legs

    void HandleLeftLegDamage(float damage)
    {
        currentLeftLegHealth = Mathf.Clamp(currentLeftLegHealth - damage, 0, maxLegHealth);
        currentLeftLegColor = Color.Lerp(damagedColor, componentHealthyColor, currentLeftLegHealth / maxLegHealth);
        StartCoroutine(FlashLeftLegIndicator(damagedColor));
        if (Random.Range(0, maxLegHealth - 1) > currentLeftLegHealth)
        {

            float randomRoll = Random.Range(0, 100) + (currentLeftLegHealth / maxLegHealth * 100);
            if (randomRoll > 100) { return; }
            if (randomRoll > 75) { HalveMoveSpeed(0); UpdateDamageEffects(); return; }
            if (randomRoll > 40) { DoubleStepTime(0); UpdateDamageEffects(); return; }
            if (randomRoll > 10) { ImpairRotation(0); UpdateDamageEffects(); return; }
            BreakLeg(0); UpdateDamageEffects();
        }
    }

    void HandleRightLegDamage(float damage)
    {
        currentRightLegHealth = Mathf.Clamp(currentRightLegHealth - damage, 0, maxLegHealth);
        currentRightLegColor = Color.Lerp(damagedColor, componentHealthyColor, currentRightLegHealth / maxLegHealth);
        StartCoroutine(FlashRightLegIndicator(damagedColor));
        if (Random.Range(0, maxLegHealth - 1) > currentRightLegHealth)
        {

            float randomRoll = Random.Range(0, 100) + (currentRightLegHealth / maxLegHealth * 100);
            if (randomRoll > 100) { return; }
            if (randomRoll > 75) { HalveMoveSpeed(1); UpdateDamageEffects(); return; }
            if (randomRoll > 40) { DoubleStepTime(1); UpdateDamageEffects(); return; }
            if (randomRoll > 10) { ImpairRotation(1); UpdateDamageEffects(); return; }
            BreakLeg(1); UpdateDamageEffects();
        }
    }

    void HalveMoveSpeed(int leg)
    {
        if (legStatusEffects[leg][0] > 1) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        legStatusEffects[leg][0]++;

        string legName = string.Empty;
        if (leg == 0) { legName = "Left leg"; } else { legName = "Right leg"; }
        switch (legStatusEffects[leg][0])
        {
            case 1: AddStatusEffect(legName + " upper servo jammed"); break;
            case 2: AddStatusEffect(legName + " hydrolic fluid leaking"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void DoubleStepTime(int leg)
    {
        if (legStatusEffects[leg][1] > 1) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        legStatusEffects[leg][1]++;

        string legName = string.Empty;
        if (leg == 0) { legName = "Left leg"; } else { legName = "Right leg"; }
        switch (legStatusEffects[leg][1])
        {
            case 1: AddStatusEffect(legName + " speed limited"); break;
            case 2: AddStatusEffect(legName + " brakes misfiring"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void ImpairRotation(int leg)
    {
        if (legStatusEffects[leg][2] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        legStatusEffects[leg][2]++;

        AddStatusEffect("Rotation servos damaged");
    }

    void BreakLeg(int leg)
    {
        if (legStatusEffects[leg][0] > 2) { return; }
        if (legStatusEffects[leg][0] < 2) { HalveMoveSpeed(leg); return; }

        legStatusEffects[leg][0] = 3;

        string legName = string.Empty;
        if (leg == 0) { legName = "Left leg"; } else { legName = "Right leg"; }
        AddStatusEffect(legName = " disabled");
    }

    #endregion

    #region launcher
    void HandleLauncherDamage(float damage)
    {
        currentLauncherHealth = Mathf.Clamp(currentLauncherHealth - damage, 0, maxLauncherHealth);
        currentLauncherColor = Color.Lerp(damagedColor, componentHealthyColor, currentLauncherHealth / maxLauncherHealth);
        StartCoroutine(FlashLauncherIndicator(damagedColor));
        if (Random.Range(0, maxLauncherHealth - 1) > currentLauncherHealth)
        {
            float randomRoll = Random.Range(0, 100) + (currentLauncherHealth / maxLauncherHealth * 100);
            if (randomRoll > 110) { DoubleUnloadTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 85) { IncreaseRegenerationTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 80) { DoubleDrumRotationTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 65) { IncreaseHeight(); UpdateDamageEffects(); return; }
            if (randomRoll > 60) { IncreaseScatter(); UpdateDamageEffects(); return; }
            if (randomRoll > 50) { DoubleDrumRotationTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 40) { DoubleLoadTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 25) { IncreaseRegenerationTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 15) { DoubleDrumRotationTime(); UpdateDamageEffects(); return; }
            if (randomRoll > 5) { IncreaseScatter(); UpdateDamageEffects(); return; }
            DestroyRegenerator();


        }
    }

    void DoubleDrumRotationTime()
    {
        if (launcherStatusEffects[0] > 2) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[0]++;
        switch (launcherStatusEffects[0])
        {
            case 1: AddStatusEffect("Launcher drum motor damaged"); break;
            case 2: AddStatusEffect("Launcher drum warped"); break;
            case 3: AddStatusEffect("Launcher drum severely damaged"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void DoubleUnloadTime()
    {
        if (launcherStatusEffects[1] > 1) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[1]++;
        switch (launcherStatusEffects[1])
        {
            case 1: AddStatusEffect("Launcher unload rail warped"); break;
            case 2: AddStatusEffect("Launcher leaking hydrolic fluid"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void DoubleLoadTime()
    {
        if (launcherStatusEffects[2] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[2]++;
        switch (launcherStatusEffects[2])
        {
            case 1: AddStatusEffect("Launcher loading mechanism damaged"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void IncreaseScatter()
    {
        if (launcherStatusEffects[3] > 2) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[3]++;
        switch (launcherStatusEffects[3])
        {
            case 1: AddStatusEffect("Launcher barrel misaligned"); break;
            case 2: AddStatusEffect("Launcher targeting sensor blocked"); break;
            case 3: AddStatusEffect("Launcher barrel bent"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void IncreaseHeight()
    {
        if (launcherStatusEffects[4] > 0) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[4]++;
        switch (launcherStatusEffects[4])
        {
            case 1: AddStatusEffect("Launcher vertical stabilizer bent"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void IncreaseRegenerationTime()
    {
        if (launcherStatusEffects[5] > 1) { return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[5]++;
        switch (launcherStatusEffects[5])
        {
            case 1: AddStatusEffect("Grenade generator leaking"); break;
            case 2: AddStatusEffect("Grenade generator feed clogged"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    void DestroyRegenerator()
    {
        if (launcherStatusEffects[5] > 2) { return; }
        if (launcherStatusEffects[5] < 2) { IncreaseRegenerationTime(); return; }
        if (!systemDamage.isPlaying) { systemDamage.Play(); }

        launcherStatusEffects[5] = 3;
        switch (launcherStatusEffects[5]) //LMAO WHAT
        {
            case 3: AddStatusEffect("Grenade generator broken"); break;
            default: Debug.LogError("WTF"); break;
        }
    }

    #endregion
    #endregion

    #region limbRepairHandlers

    #region TORSO

    public void RepairUILeft()
    {
        torsoStatusEffects[0] = 0;
        RemoveStatusEffect("Status HUD Damaged");
    }

    public void RepairUIMiddle()
    {
        torsoStatusEffects[1] = 0;
        RemoveStatusEffect("Center HUD Damaged");
    }

    public void RepairUIRight()
    {
        torsoStatusEffects[2] = 0;
        RemoveStatusEffect("Weapon HUD Damaged");
    }

    public void RepairLightLeft()
    {
        torsoStatusEffects[3] = 0;
        torsoStatusEffects[5] = 0;
        RemoveStatusEffect("Left Spotlight Damaged");
        RemoveStatusEffect("Left Spotlight Destroyed");
    }
    
    public void RepairLightRight()
    {
        torsoStatusEffects[4] = 0;
        torsoStatusEffects[6] = 0;
        RemoveStatusEffect("Right Spotlight Damaged");
        RemoveStatusEffect("Right Spotlight Destroyed");
    }

    public void RepairShields()
    {
        torsoStatusEffects[7] = 0;
        RemoveStatusEffect("Shield capacitor leaking");
        RemoveStatusEffect("Shield matrix misaligned");
        RemoveStatusEffect("Shields cascading");
        RemoveStatusEffect("Shields destroyed");
    }

    public void RepairArmor()
    {
        torsoStatusEffects[9] = 0;
        RemoveStatusEffect("Torso armor punctured");
        RemoveStatusEffect("Torso armor severely damaged");
        RemoveStatusEffect("Torso armor broken");
    }

    #endregion

    #region ARM

    public void RepairBarrelZero()
    {
        armStatusEffects[0] = 0;
        RemoveStatusEffect("Barrel 1 warped");
        RemoveStatusEffect("Barrel 1 severely damaged");
        RemoveStatusEffect("Barrel 1 destroyed");
    }
    
    public void RepairBarrelOne()
    {
        armStatusEffects[1] = 0;
        RemoveStatusEffect("Barrel 2 warped");
        RemoveStatusEffect("Barrel 2 severely damaged");
        RemoveStatusEffect("Barrel 2 destroyed");
    }
    
    public void RepairBarrelTwo()
    {
        armStatusEffects[2] = 0;
        RemoveStatusEffect("Barrel 3 warped");
        RemoveStatusEffect("Barrel 3 severely damaged");
        RemoveStatusEffect("Barrel 3 destroyed");
    }

    public void RepairFireRate()
    {
        armStatusEffects[3] = 0;
        RemoveStatusEffect("Gun motor snagging");
        RemoveStatusEffect("Firing mechanism damaged");
    }

    public void RepairReloadTime()
    {
        armStatusEffects[4] = 0;
        RemoveStatusEffect("Magazine holder misaligned");
        RemoveStatusEffect("Reload coroutine lagging");
        RemoveStatusEffect("Clip ejector jammed");
    }

    public void RepairBulletSpread()
    {
        armStatusEffects[5] = 0;
        RemoveStatusEffect("Targeting mechanisms failing");
    }

    public void RepairClipSize()
    {
        armStatusEffects[6] = 0;
        RemoveStatusEffect("Secondary clip feed broken");
        RemoveStatusEffect("Clip feed jammed");
    }

    #endregion

    #region LEGS

    public void RepairMoveSpeed(int leg)
    {
        legStatusEffects[leg][0] = 0;
        
        string legName = string.Empty;
        if (leg == 0) { legName = "Left leg"; } else { legName = "Right leg"; }

        RemoveStatusEffect(legName + " upper servo jammed");
        RemoveStatusEffect(legName + " hydrolic fluid leaking");
        RemoveStatusEffect(legName + " disabled");
    }

    public void RepairStepTime(int leg)
    {
        legStatusEffects[leg][1] = 0;
        
        string legName = string.Empty;
        if (leg == 0) { legName = "Left leg"; } else { legName = "Right leg"; }

        RemoveStatusEffect(legName + " speed limited");
        RemoveStatusEffect(legName + " brakes misfiring");        
    }

    public void RepairRotationSpeed()
    {
        legStatusEffects[0][2] = 0;
        legStatusEffects[1][2] = 0;

        RemoveStatusEffect("Rotation servos damaged");
    }

    #endregion

    #region LAUNCHER

    public void RepairDrumRotation()
    {
        launcherStatusEffects[0] = 0;
        RemoveStatusEffect("Launcher drum motor damaged");
        RemoveStatusEffect("Launcher drum warped");
        RemoveStatusEffect("Launcher drum severely damaged");
    }

    public void RepairUnloadTime()
    {
        launcherStatusEffects[1] = 0;
        RemoveStatusEffect("Launcher unload rail warped");
        RemoveStatusEffect("Launcher leaking hydrolic fluid");
    }

    public void RepairLoadTime()
    {
        launcherStatusEffects[2] = 0;
        RemoveStatusEffect("Launcher loading mechanism damaged");
    }

    public void RepairScatter()
    {
        launcherStatusEffects[3] = 0;
        RemoveStatusEffect("Launcher barrel misaligned");
        RemoveStatusEffect("Launcher targeting sensor blocked");
        RemoveStatusEffect("Launcher barrel bent");
    }

    public void RepairProjectileHeight()
    {
        launcherStatusEffects[4] = 0;
        RemoveStatusEffect("Launcher vertical stabilizer bent");
    }

    public void RepairGrenadeRegenerator()
    {
        launcherStatusEffects[5] = 0;
        RemoveStatusEffect("Grenade generator leaking");
        RemoveStatusEffect("Grenade generator feed clogged");
        RemoveStatusEffect("Grenade generator broken");
    }

    #endregion

    #endregion

    IEnumerator Flicker(GameObject targetObject)
    {
        Image[] targets = targetObject.GetComponentsInChildren<Image>();
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(flickerMinTime / Mathf.Min(targets.Length, 5), flickerMaxTime / Mathf.Min(targets.Length, 5)));

            //half of the time, flicker an image. Otherwise, flicker text;
            if (Random.Range(0, 2) == 0)
            {
                Image current = targets[Random.Range(0, targets.Length)];

                Image[] par = current.GetComponentsInParent<Image>();

                bool isParentActive = true;
                foreach (Image i in par)
                {
                    if (i == current) { continue; }
                    isParentActive = isParentActive && i.enabled;
                }

                if (!isParentActive) { continue; }

                current.enabled = !current.enabled;

                Image[] kids = current.GetComponentsInChildren<Image>();
                Text[] kidsText = current.GetComponentsInChildren<Text>();

                foreach (Image i in kids)
                { i.enabled = current.enabled; }

                foreach (Text t in kidsText)
                {
                    t.enabled = current.enabled;
                }
            }
            else
            {
                Text[] texts = targetObject.GetComponentsInChildren<Text>();
                for (int i = 0; i < Random.Range(1,4); i++)
                {
                    Text current = Utility.Pick(texts);

                    Image[] par = current.GetComponentsInParent<Image>();

                    bool isParentActive = true;
                    foreach (Image img in par)
                    {
                        if (img == current) { continue; }
                        isParentActive = isParentActive && img.enabled;
                    }

                    if (!isParentActive) { continue; }

                    current.enabled = !current.enabled;
                    yield return new WaitForSeconds(Random.Range(flickerMinTime, flickerMaxTime));
                }                
            }
        }
    }

    void StopFlicker(GameObject targetObject)
    {
        Image[] targets = targetObject.GetComponentsInChildren<Image>();
        Text[] targetTexts = targetObject.GetComponentsInChildren<Text>();

        foreach (Text t in targetTexts)
        {
            t.enabled = true;
        }

        foreach (Image i in targets)
        {
            i.enabled = true;
        }
    }

    IEnumerator Flicker(Light light)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(flickerMinTime, flickerMaxTime));
            light.enabled = !light.enabled;
        }
    }

    void StopFlicker(Light light)
    {
        light.enabled = true;
    }

    #region indicator flashin

    IEnumerator FlashShieldOutline(Color flashColor)
    {
        if (shieldFlashing) { yield break; }
        shieldFlashing = true;
        shieldOutline.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        shieldOutline.color = shieldOutlineColor;
        shieldFlashing = false;
    }

    IEnumerator FlashTorsoIndicator(Color flashColor)
    {
        if (torsoFlashing) { yield break; }
        torsoFlashing = true;
        torsoIndicator.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        torsoIndicator.color = currentTorsoColor;
        torsoFlashing = false;
    } 
    
    IEnumerator FlashArmIndicator(Color flashColor)
    {
        if (armFlashing) { yield break; }
        armFlashing = true;
        armIndicator.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        armIndicator.color = currentArmColor;
        armFlashing = false;
    }
    IEnumerator FlashLeftLegIndicator(Color flashColor)
    {
        if (leftLegFlashing) { yield break; }
        leftLegFlashing = true;
        leftLegIndicator.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        leftLegIndicator.color = currentLeftLegColor;
        leftLegFlashing = false;
    }

    IEnumerator FlashRightLegIndicator(Color flashColor)
    {
        if (rightLegFlashing) { yield break; }
        rightLegFlashing = true;
        rightLegIndicator.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        rightLegIndicator.color = currentRightLegColor;
        rightLegFlashing = false;
    } 
    
    IEnumerator FlashLauncherIndicator(Color flashColor)
    {
        if (launcherFlashing) { yield break; }
        launcherFlashing = true;
        launcherIndicator.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        launcherIndicator.color = currentLauncherColor;
        launcherFlashing = false;
    }

    #endregion

    void AddStatusEffect(string text)
    {
        statusEffects.Add(text);        
        UpdateStatusEffects();
    }

    void RemoveStatusEffect(string text)
    {
        //statusEffects.Remove(text);
        statusEffects.RemoveAll(x => x == text); //Not sure if this works but like, let's find out?
        UpdateStatusEffects();
    }

    void UpdateStatusEffects()
    {
        systemDamageText[0].SetActive(statusEffects.Count >= 5 && statusEffects.Count < 12);
        systemDamageText[1].SetActive(statusEffects.Count >= 12 && statusEffects.Count < 20);
        if (statusEffects.Count >= 20)
        {
            systemDamageText[2].SetActive(true);
            if (!warningSiren.isPlaying) { warningSiren.Play(); }
        }
        else
        {
            systemDamageText[2].SetActive(false);
            warningSiren.Stop();
        }

        string result = string.Empty;
        for (int i = 0; i < statusEffects.Count; i++)
        {
            result += statusEffects[i];
            result += "\n";
        }
        statusEffectsText.text = result;
    }

    #region IDamagable
    public void TakeDamage(float damage, Collider collider)
    {
        timeSinceLastDamaged = 0;
        if (CurrentShield > 0)
        {
            float excessDamage = damage - CurrentShield;
            CurrentShield -= damage;

            if (excessDamage > 0) { TakeDamage(excessDamage, collider); }
            return;
        }

        if (collider == torsoCollider) { HandleTorsoDamage(damage); return; }
        if (collider == armCollider) { HandleArmDamage(damage); return; }
        if (collider == leftLegCollider) { HandleLeftLegDamage(damage); return; }
        if (collider == rightLegCollider) { HandleRightLegDamage(damage); return; }
        if (collider == launcherCollider) { HandleLauncherDamage(damage); return; }
    }

    public void TakeFlash(float intensity, Collider collider)
    {
        if(intensity <= 0.0f) { return; }
        //throw new System.NotImplementedException();
        flashUI.Flash(intensity);
    }

    public void TakeEMP(float intensity, Collider collider)
    {
        StartCoroutine(TakeEMPC(intensity));
    }

    IEnumerator TakeEMPC(float intensity)
    {
        if (EMPd || intensity <= 0.0f) { yield break; }
        EMPd = true;

        bool[] oldStatuses = new bool[EMPable.Length];

        for (int i = 0; i < EMPable.Length; i++)
        {
            oldStatuses[i] = EMPable[i].activeSelf;
            for (int j = 0; j < Random.Range(1, 4); j++)
            {
                yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
                EMPable[i].SetActive(oldStatuses[i]);

                yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
                EMPable[i].SetActive(false);
            }
        }

        yield return new WaitForSeconds(empTime * intensity);

        Utility.FisherYates(ref EMPable);

        for (int i = 0; i < EMPable.Length; i++)
        {
            for (int j = 0; j < Random.Range(2, 6); j++)
            {
                yield return new WaitForSeconds(Random.Range(0.025f, 0.2f));
                EMPable[i].SetActive(false);

                yield return new WaitForSeconds(Random.Range(0.025f, 0.2f));
                EMPable[i].SetActive(oldStatuses[i]);
            }
        }       

        EMPd = false;
    }

    public void TakeStun(float intensity, Collider collider)
    {
        return; //LMAO I'm in a mech what did you think would happen
    }

    #endregion



    IEnumerator Die()
    {
        reactorMeltdown.SetActive(true);
        middleCanvasBackground.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0.0f;
        deathPanel.SetActive(true);
        deathPanel.GetComponent<Image>().color = Color.clear;
        float t = 0;
        while(t <= 1.0f)
        {
            t += Time.unscaledDeltaTime / 3.0f;
            deathPanel.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, t);
        }
    }

}
