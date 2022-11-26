using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public Text primaryObjectiveText;
    public Text[] secondaryObjectiveTexts;
    public AudioSource objectiveNotificationSound;

    string primaryObjective;
    public string PrimaryObjective
    {
        get
        {
            return primaryObjective;
        }
        set
        {
            primaryObjective = value;
            primaryObjectiveText.text = primaryObjective;
            primaryObjectiveText.color = objectiveTextColor;
            if (primaryObjective != "" && primaryObjective != string.Empty)
            { objectiveNotificationSound.Play(); }
        }
    }

    string secondaryObjective0;
    public string SecondaryObjective0
    {
        get
        {
            return secondaryObjective0;
        }
        set
        {
            secondaryObjective0 = value;
            secondaryObjectiveTexts[0].text = secondaryObjective0;
            secondaryObjectiveTexts[0].color = objectiveTextColor;

            if (secondaryObjective0 != "" && secondaryObjective0 != string.Empty)
            { objectiveNotificationSound.Play(); }
        }
    }

    string secondaryObjective1;
    public string SecondaryObjective1
    {
        get
        {
            return secondaryObjective1;
        }
        set
        {
            secondaryObjective1 = value;
            secondaryObjectiveTexts[1].text = secondaryObjective1;
            secondaryObjectiveTexts[1].color = objectiveTextColor;

            if (secondaryObjective1 != "" && secondaryObjective1 != string.Empty)
            { objectiveNotificationSound.Play(); }
        }
    }

    string secondaryObjective2;
    public string SecondaryObjective2
    {
        get
        {
            return secondaryObjective2;
        }
        set
        {
            secondaryObjective2 = value;
            secondaryObjectiveTexts[2].text = secondaryObjective2; 
            secondaryObjectiveTexts[2].color = objectiveTextColor;

            if (secondaryObjective2 != "" && secondaryObjective2 != string.Empty)
            { objectiveNotificationSound.Play(); }
        }
    }

    public Color objectiveTextColor, objectiveCompletedColor, objectiveFailedColor;

    private void Awake()
    {
        if(Instance != null) { Debug.LogWarning("Several mission managers existed. Destroyed the old one. Oops."); Destroy(Instance.gameObject); return; }
        Instance = this;
    }

    public void CompletePrimaryObjective()
    {
        primaryObjectiveText.color = objectiveCompletedColor;
        objectiveNotificationSound.Play();
    }

    public void CompleteSecondaryObjective(int i)
    {
        if(secondaryObjectiveTexts[i] == null) { return; } //Haha I love me some editor errors
        secondaryObjectiveTexts[i].color = objectiveCompletedColor;
        objectiveNotificationSound.Play();
    }
    
    public void FailPrimaryObjective()
    {
        primaryObjectiveText.color = objectiveFailedColor;
        objectiveNotificationSound.Play();
    }

    public void FailSecondaryObjective(int i)
    {
        secondaryObjectiveTexts[i].color = objectiveFailedColor;
        objectiveNotificationSound.Play();
    }

    public void ResetObjectives()
    {
        PrimaryObjective = "";
        ResetSecondaryObjectives();
    }

    public void ResetSecondaryObjectives()
    {
        SecondaryObjective0 = "";
        SecondaryObjective1 = "";
        SecondaryObjective2 = "";
    }
}
