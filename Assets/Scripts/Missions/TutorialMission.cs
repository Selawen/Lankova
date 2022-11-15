using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMission : Mission
{
    public GameObject[] initialBootUp, firingRangeBootUp, grenadeRangeBootUp, finalBootUp;
    public GameObject[] triggers;
    public Door[] doors;
    public AudioSource voiceSource;
    public AudioClip[] voiceClips;
    public AudioClip grenadesEmpty;
    
    PlayerMovement player; GrenadeLauncher launcher; DamageHandler damageHandler;
    MissionManager misMan;
    bool arrivedAtFiringRange = false, arrivedAtGrenadeRange = false;

    public int gunTargetsDestroyed = 0, fragTargetsDestroyed = 0, flashTargetsFlashed = 0, smokeTargetsSmoked = 0, apTargetsDestroyed = 0;
    public int gunTargetsToDestroy = 0, fragTargetsToDestroy = 0, flashTargetsToFlash = 0, smokeTargetsToSmoke = 0, apTargetsToDestroy = 0;
    public GameObject[] gunTargetDummies, fragTargetDummies, flashTargetDummies, smokeTargetDummies, apTargetDummies;

    IEnumerator BootItems(GameObject[] obis)
    {
        for (int i = 0; i < obis.Length; i++)
        {
            yield return StartCoroutine(BootItem(obis[i]));
        }
    }

    IEnumerator BootItem(GameObject obi)
    {
        yield return new WaitForSeconds(Random.Range(0.15f, 0.55f));
        obi.SetActive(true);
        if (Random.Range(0, 100) < 20) { StartCoroutine(FlickerItem(obi)); }
    }

    IEnumerator FlickerItem(GameObject obi)
    {
        for (int i = 0; i < Random.Range(1, 6); i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            obi.SetActive(false);

            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            obi.SetActive(true);
        }
    }

    private void Start()
    {
        player = PlayerMovement.Instance;
        launcher = player.GetComponent<GrenadeLauncher>();
        damageHandler = player.GetComponent<DamageHandler>();
        misMan = MissionManager.Instance;
        ResetTriggers();
    }
    
    void ResetTriggers()
    {
        foreach (GameObject obi in triggers) { obi.SetActive(false); }
    }

    public override void StartMission()
    {
        StartCoroutine(StartMissionC());
    }

    IEnumerator StartMissionC()
    {
        ResetTriggers();
        triggers[0].SetActive(true);
        misMan.ResetObjectives();

        yield return StartCoroutine(BootItems(initialBootUp));
        damageHandler.BootUp();
       
        misMan.PrimaryObjective = "Go to Firing Range";
        voiceSource.PlayOneShot(voiceClips[0]);
        Subtitles.PlaySubtitles("Cadet Lankova, it is time for your training. Report to the firing range immediately.");

        yield return new WaitForSeconds(1.0f);
        misMan.SecondaryObjective0 = "Use mouse to look around";
        player.StartLooking();

        while(!(Input.GetAxis("Mouse X") > 0.1f) && !(Input.GetAxis("Mouse Y") > 0.1f))
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        misMan.CompleteSecondaryObjective(0);

        yield return new WaitForSeconds(2.0f);

        misMan.SecondaryObjective1 = "Use WASD to walk";
        player.StartMovement();
        doors[0].OpenDoor(false);

        while (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        misMan.CompleteSecondaryObjective(1);

        while (!arrivedAtFiringRange) { yield return null; }
        misMan.CompletePrimaryObjective();

        StartCoroutine(BootItems(firingRangeBootUp));
        foreach (GameObject obi in gunTargetDummies)
        {
            obi.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2.0f);

        voiceSource.PlayOneShot(voiceClips[1]);
        Subtitles.PlaySubtitles("Use your shotgun to destroy the targets");
        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Gun training";
        misMan.SecondaryObjective0 = "Use left mouse to fire";
        misMan.SecondaryObjective1 = "Destroy all target dummies";

        player.StartShooting();
        while (!Input.GetMouseButton(0)) { yield return null; }
        misMan.CompleteSecondaryObjective(0);

        while (gunTargetsDestroyed < gunTargetsToDestroy) { yield return new WaitForSeconds(1.0f); }
        misMan.CompleteSecondaryObjective(1);
        misMan.CompletePrimaryObjective();

        voiceSource.PlayOneShot(voiceClips[2]);
        Subtitles.PlaySubtitles("Good job. Proceed to the grenade range for grenade training. On the double, cadet.");
        while (voiceSource.isPlaying) { yield return null; }

        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Go to grenade range";
        misMan.SecondaryObjective0 = "Use shift to sprint";

        player.StartSprinting();
        doors[1].OpenDoor(false);

        ResetTriggers();
        triggers[1].SetActive(true);

        while (!Input.GetKey(KeyCode.LeftShift)) { yield return null; }
        misMan.CompleteSecondaryObjective(0);

        while (!arrivedAtGrenadeRange) { yield return null; }
        misMan.CompletePrimaryObjective();
        
        yield return StartCoroutine(BootItems(grenadeRangeBootUp));
        launcher.BootUp();

        foreach (GameObject obi in fragTargetDummies)
        {
            obi.SetActive(true);
        }

        voiceSource.PlayOneShot(voiceClips[3]);
        Subtitles.PlaySubtitles("Frag grenades are useful against groups of enemies. Use them to destroy the targets.");
        while (voiceSource.isPlaying) { yield return null; }

        launcher.FillGrenades(0);

        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Frag training";
        misMan.SecondaryObjective0 = "Destroy all target dummies";
        misMan.SecondaryObjective1 = "Press 1 to load a frag grenade";

        while (!Input.GetKey(KeyCode.Alpha1)) { yield return null;}
        misMan.CompleteSecondaryObjective(1);
        misMan.SecondaryObjective2 = "Use right mouse to launch grenade";

        while (!Input.GetMouseButton(1)) { yield return null; }
        misMan.CompleteSecondaryObjective(2);

        while (fragTargetsDestroyed < fragTargetsToDestroy)
        {
            if (launcher.grenadeCurrentAmmo[0] < 1)
            {
                voiceSource.PlayOneShot(grenadesEmpty);
                Subtitles.PlaySubtitles("Ran out? Don't worry, here's some extras");
                launcher.FillGrenades(0);
            }

            while (fragTargetsDestroyed < fragTargetsToDestroy && launcher.grenadeCurrentAmmo[0] >= 1) { yield return new WaitForSeconds(1.0f); }
        }
        misMan.CompletePrimaryObjective();
        misMan.CompleteSecondaryObjective(0);
        foreach (GameObject obi in flashTargetDummies)
        {
            obi.SetActive(true);
        }

        voiceSource.PlayOneShot(voiceClips[4]);
        Subtitles.PlaySubtitles("Flashbang grenades can disorient and stun targets. They also have an EMP component for mechanical enemies. Use one to stun the targets.");
        while (voiceSource.isPlaying) { yield return null; }

        launcher.FillGrenades(1);

        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Flashbang training";
        misMan.SecondaryObjective0 = "Stun all target dummies";
        misMan.SecondaryObjective1 = "Press 2 to load a flash grenade";

        while (!Input.GetKey(KeyCode.Alpha2)) { yield return null; }
        misMan.CompleteSecondaryObjective(1);
        misMan.SecondaryObjective2 = "Use right mouse to launch grenade";

        while (!Input.GetMouseButton(1)) { yield return null; }
        misMan.CompleteSecondaryObjective(2);

        while (flashTargetsFlashed < flashTargetsToFlash)
        {

            if (launcher.grenadeCurrentAmmo[1] < 1)
            {
                voiceSource.PlayOneShot(grenadesEmpty);
                Subtitles.PlaySubtitles("Ran out? Don't worry, here's some extras");
                launcher.FillGrenades(1);
            }

            while (flashTargetsFlashed < flashTargetsToFlash && launcher.grenadeCurrentAmmo[1] >= 1) { yield return new WaitForSeconds(1.0f); }
        }

        misMan.CompleteSecondaryObjective(0);
        misMan.CompletePrimaryObjective();
        
        foreach (GameObject obi in flashTargetDummies)
        {
            obi.SetActive(false);
        }

        foreach (GameObject obi in smokeTargetDummies)
        {
            obi.SetActive(true);
        }

        voiceSource.PlayOneShot(voiceClips[5]);
        Subtitles.PlaySubtitles("Smoke grenades impair enemy vision and accuracy. Use them on the targets.");
        while (voiceSource.isPlaying) { yield return null; }

        launcher.FillGrenades(2);

        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Smoke training";
        misMan.SecondaryObjective0 = "Cover all target dummies in smoke";
        misMan.SecondaryObjective1 = "Press 3 to load a smoke grenade";

        while (!Input.GetKey(KeyCode.Alpha3)) { yield return null; }
        misMan.CompleteSecondaryObjective(1);
        misMan.SecondaryObjective2 = "Use right mouse to launch grenade";

        while (!Input.GetMouseButton(1)) { yield return null; }
        misMan.CompleteSecondaryObjective(2);

        while (smokeTargetsSmoked < smokeTargetsToSmoke)
        {

            if (launcher.grenadeCurrentAmmo[2] < 1)
            {
                voiceSource.PlayOneShot(grenadesEmpty);
                Subtitles.PlaySubtitles("Ran out? Don't worry, here's some extras");
                launcher.FillGrenades(2);
            }

            while (smokeTargetsSmoked < smokeTargetsToSmoke && launcher.grenadeCurrentAmmo[2] >= 1) { yield return new WaitForSeconds(1.0f); }
        }
        misMan.CompleteSecondaryObjective(0);
        misMan.CompletePrimaryObjective();

        foreach (GameObject obi in smokeTargetDummies)
        {
            obi.SetActive(false);
        }

        foreach (GameObject obi in apTargetDummies)
        {
            obi.SetActive(true);
        }

        voiceSource.PlayOneShot(voiceClips[6]);
        Subtitles.PlaySubtitles("Armor Penetrating grenades are useful against armored enemies and strong objects. Use an AP grenade to destroy the target's cover, then kill the target with your shotgun.");
        while (voiceSource.isPlaying) { yield return null; }

        launcher.FillGrenades(3);

        misMan.ResetObjectives();
        misMan.PrimaryObjective = "AP training";
        misMan.SecondaryObjective0 = "Destroy the target dummy";
        misMan.SecondaryObjective1 = "Press 4 to load an AP grenade";

        while (!Input.GetKey(KeyCode.Alpha4)) { yield return null; }
        misMan.CompleteSecondaryObjective(1);
        misMan.SecondaryObjective2 = "Use right mouse to launch grenade";

        while (!Input.GetMouseButton(1)) { yield return null; }
        misMan.CompleteSecondaryObjective(2);

        while (apTargetsDestroyed < apTargetsToDestroy)
        {

            if (launcher.grenadeCurrentAmmo[3] < 1)
            {
                voiceSource.PlayOneShot(grenadesEmpty);
                Subtitles.PlaySubtitles("Ran out? Don't worry, here's some extras");
                launcher.FillGrenades(2);
            }

            while (apTargetsDestroyed < apTargetsToDestroy && launcher.grenadeCurrentAmmo[3] >= 1) { yield return new WaitForSeconds(1.0f); }
        }
        misMan.CompleteSecondaryObjective(0);
        misMan.CompletePrimaryObjective();

        voiceSource.PlayOneShot(voiceClips[7]);
        Subtitles.PlaySubtitles("Congratulations, pilot. You have finished your training. Go down the hallway, and report to the staging area to start your mission. Good luck.");

        ResetTriggers();
        triggers[2].SetActive(true);
        while (voiceSource.isPlaying) { yield return null; }
        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Report to the staging area";
        doors[2].OpenDoor(false);

        launcher.FillGrenades();       

    }


    public override void EndMission(bool success)
    {
        if(success)
        {
            PlayerPrefs.SetInt("TutorialPlayed", 1);
            misMan.CompletePrimaryObjective();
        }
    }

    public void ArriveAtFiringRange()
    {
        arrivedAtFiringRange = true;
    }

    public void ArriveAtGrenadeRange()
    {
        arrivedAtGrenadeRange = true;
    }

    public void HitTarget(int index)
    {
        switch(index)
        {
            case 0: gunTargetsDestroyed++; break;
            case 1: fragTargetsDestroyed++; break;
            case 2: flashTargetsFlashed++; break;
            case 3: smokeTargetsSmoked++; break;
            case 4: apTargetsDestroyed++; break;            
        }
        
    }
}
