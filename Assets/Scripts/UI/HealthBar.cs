using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    float hpValue = 1.0f;

    [SerializeField]
    public float HPValue
    {
        get { return hpValue; }
        set 
        {
            hpValue = Mathf.Clamp(value, 0.0f, 1.0f);
            bar.transform.localScale = new Vector3(hpValue, 1);
        }
    }

    [SerializeField]
    public float ConcreteHPValue
    {
        get { return HPValue * maxConcreteHPValue; }
        set { HPValue = value / maxConcreteHPValue; }
    }
    public float maxConcreteHPValue;
}
