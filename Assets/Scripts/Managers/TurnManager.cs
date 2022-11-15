using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public TurnActorCollection actors { get; private set; }

    private void Awake()
    {
        if (Instance) { Destroy(Instance.gameObject); } //If any previous turn managers somehow exist, destroy them.
        Instance = this;
        actors = new TurnActorCollection();
    }

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return null;
        Debug.Log(actors.Count);
        while(actors.Count > 0)
        {
            yield return actors.actors[0].StartCoroutine(actors.actors[0].Act());
            actors.CycleActor();
            actors.LowerInitiatives();
        }
    }

    public void AddActor(TurnActor a)
    {
        actors.AddActor(a);
    }

    public void RemoveActor(TurnActor a)
    {
        actors.RemoveActor(a);
    }

    /// <summary>
    /// Get the initiative of the last actor in the actors list. Useful for games with a static turn rotation
    /// </summary>
    /// <returns></returns>
    public int GetLastActorInitiative()
    {
        return actors.actors[actors.Count-1].initiative; 
    }
}
