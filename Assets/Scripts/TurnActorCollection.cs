using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorCollection
{
    public List<TurnActor> actors { get; private set; }

    public TurnActorCollection()
    {
        actors = new List<TurnActor>();
    }

    public int Count
    {
        get
        {
            return actors.Count;
        }
    }

    public void AddActor(TurnActor a)
    {
        actors.Add(a);
        for (int i = actors.Count-1; i >= 1 && actors[i].initiative < actors[i-1].initiative; i--)
        {
            Swap(i, i - 1);
        }
    }

    public void RemoveActor(TurnActor a)
    {
        actors.Remove(a);
    }

    public void CycleActor()
    {
        if(actors.Count < 2) { return; }
        TurnActor a = actors[0];
        RemoveActor(a);
        AddActor(a);
    }

    /// <summary>
    /// Lowers the initiative of all actors with the initiative of the actor on index 0.
    /// </summary>
    public void LowerInitiatives()
    {
        if (actors.Count < 1) { return; }
        int amount = actors[0].initiative;
        foreach(TurnActor a in actors)
        {
            a.initiative -= amount;
        }
    }

    public void Swap(int one, int two)
    {
        TurnActor[] backup = new TurnActor[actors.Count];
        actors.CopyTo(backup);

        try
        {
            TurnActor temp = actors[one];
            actors[one] = actors[two];
            actors[two] = temp;
        }
        catch(System.IndexOutOfRangeException)
        {
            actors = new List<TurnActor>(backup);
            Debug.LogWarning("Attempted to Swap two turn actors when there was less than two");
        }
    }
}
