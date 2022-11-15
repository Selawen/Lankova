using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlantMission : Mission
{
    public AudioClip[] voiceClips;
    public AudioSource audioSource;
    public AudioSource backgroundMusic;

    [TextArea]
    public string[] subtitles;
    public GameObject[] triggers;    

    public GameObject doors;
    public GameObject lights;
    MissionManager misMan;
    public GunTurret[] gunTurrets;
    int powerGeneratorsDestroyed = 0;
    public int powerGeneratorsToDestroy;
    public Door doorToPoseidon;

    //MISSION OUTLINE

    //Primary Objective:        Reach Inner Ship
    //First Objective:      Enter Enemy Ship
    //Second Objective:     Breach Outer Perimeter
    //                      Find Power Plant
    //                      Destroy Power Plant
    //Fourth Objective:     Breach Inner Perimeter
    //Mission Success

    //TRIGGERS

    //trigger[0] = start mission
    //trigger[1] = enter ship
    //trigger[2] = through outer perimeter
    //trigger[3] = enter power plant
    //trigger[4] = reach inner perimeter


    private void Start()
    {
        misMan = MissionManager.Instance;
    }

    public override void EndMission(bool success)
    {
        throw new System.NotImplementedException();
    }

    public override void StartMission()
    {
        StartCoroutine(StartMissionC());
    }

    IEnumerator StartMissionC()
    {
        triggers[0].SetActive(false);
        misMan.CompletePrimaryObjective();

        for (int i = 0; i < 3; i++)
        {
            audioSource.PlayOneShot(voiceClips[i]);
            Subtitles.PlaySubtitles(subtitles[i]);
            yield return new WaitForSeconds(voiceClips[i].length);
        }
        backgroundMusic.Play();
        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Break into inner ship";
        misMan.SecondaryObjective0 = "Board the Poseidon";
        triggers[1].SetActive(true);
        doorToPoseidon.OpenDoor(false);
    }

    public void EnterShip()
    {
        triggers[1].SetActive(false);

        misMan.ResetSecondaryObjectives();
        misMan.SecondaryObjective0 = "Breach outer perimeter";

        triggers[2].SetActive(true);
    }

    public void BreachOuterPerimeter()
    {
        triggers[2].SetActive(false);

        misMan.CompleteSecondaryObjective(0);
        misMan.SecondaryObjective1 = "Find the power generator";

        triggers[3].SetActive(true);
    }

    public void FindPowerGenerator()
    {
        triggers[3].SetActive(false);

        misMan.CompleteSecondaryObjective(1);

        misMan.SecondaryObjective2 = "Destroy the power generators";
    }

    public void DestroyPowerGenerator()
    {
        powerGeneratorsDestroyed++;
        if(powerGeneratorsDestroyed >= powerGeneratorsToDestroy)
        {
            triggers[4].SetActive(true);
            misMan.CompleteSecondaryObjective(2);
            foreach(Door d in doors.GetComponentsInChildren<Door>())
            {
                d.OpenDoor(true);                
            }

            lights.SetActive(false);
            foreach(GunTurret turret in gunTurrets)
            {
                turret.PowerDown();
            }
            StartCoroutine(PowerGeneratorDestroyed());
        }
    }

    IEnumerator PowerGeneratorDestroyed()
    {
        yield return new WaitForSeconds(3.0f);
        foreach (Door d in doors.GetComponentsInChildren<Door>())
        {
            d.enabled = false;
            foreach(Collider c in d.GetComponentsInChildren<Collider>())
            {
                if (c.isTrigger) { c.gameObject.SetActive(false); }
            }
        }
        misMan.ResetSecondaryObjectives();
        misMan.SecondaryObjective0 = "Breach inner perimeter";
    }

    public void ReachInnerShip()
    {
        triggers[4].SetActive(false);
        misMan.CompletePrimaryObjective();
        misMan.CompleteSecondaryObjective(0);
        audioSource.PlayOneShot(voiceClips[3]);
        Subtitles.PlaySubtitles(subtitles[3]);
    }

    public void SetStartText()
    {
        misMan.ResetObjectives();
        misMan.PrimaryObjective = "Go to Staging Area";
    }
}
