using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurnActor : MonoBehaviour
{
    public int initiative;

    /// <summary>
    /// Act should modify the actors initiative value in some way. 
    /// To make a static rotating turn, you can set it to the initiative of the last actor in the turnmanager +1. 
    /// If you have turns with some sort of time cost, you can set it to the initiative of that time cost
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator Act();

    protected virtual void Start()
    {
        TurnManager.Instance.AddActor(this);
    }

    protected virtual void OnDestroy()
    {
        TurnManager.Instance.RemoveActor(this);
    }
}
