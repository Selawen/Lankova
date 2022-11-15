using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDefaultValues", menuName = "ScriptableObjects/PlayerDefaultValues")]
public class PlayerDefaultValues : ScriptableObject
{
    [Header("Movement")]
    public float baseMoveSpeed = 1.5f;
    public float baseStepTime = 0.5f, baseNormalRotationSpeed = 20;
    public float[] baseLegMoveSpeeds = {2.0f,2.0f}, baseLegStepTimes = {0.25f, 0.25f};
    public float baseBopHeight = 0.1f, baseSprintMultiplier = 3.0f;

    [Header("Shooting")]
    public float baseFireRate = 3.0f;
    public int baseBulletAmount = 12;
    public float baseBulletMaxSpread = 0.05f, baseSprintSpreadMultiplier = 4.0f, baseRecoilAmoint = 0.15f, baseRecoilTimeBack = 0.02f, baseRecoilTimeForward = 0.13f;
    public int baseMaxClipSize = 60;
    public float baseReloadTime = 3.0f;

    [Header("Launcher")]
    public float baseProjectileHeight = 0.1f;
    public float baseBarrelRotateTime = 0.25f, baseUnloadTime = 0.5f, baseLoadTime = 0.5f, baseRegenerationTime = 30.0f;
    public int[] baseGrenadeMaxAmmo = { 2, 4, 3, 2 };
    public float baseMaxScatter = 0.0f;
    public bool baseRegenActive = true;

    [Header("DamageHandler")]
    public float baseShieldMax = 100;
    public float baseMinimumShieldRechargeAmount = 5, baseShieldRechargePenalty = 5, baseTorsoArmor = 3, 
        baseMaxTorsoHealth = 200, baseMaxLegHealth= 100, baseMaxArmHealth = 100, baseMaxLauncherHealth = 100, baseEMPTime = 9;
}
