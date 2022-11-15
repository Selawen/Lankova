using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mission : MonoBehaviour
{
    public abstract void StartMission();

    public abstract void EndMission(bool success);
}
