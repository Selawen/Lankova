using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Text scoreValueText;

    private int scoreValue;
    public int ScoreValue
    {
        get { return scoreValue; }
        set 
        {
            scoreValue = value;
            scoreValueText.text = scoreValue.ToString();
        }
    }
}
