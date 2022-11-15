using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayedPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("TutorialPlayed"))
        {
            if(PlayerPrefs.GetInt("TutorialPlayed") > 0) { gameObject.SetActive(false); }
        }
        else
        {
            PlayerPrefs.SetInt("TutorialPlayed", 0);
        }
    }

    public void FinishTutorial()
    {
        PlayerPrefs.SetInt("TutorialPlayed", 1);
    }
}
