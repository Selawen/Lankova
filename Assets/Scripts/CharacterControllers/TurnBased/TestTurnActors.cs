using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTurnActors : TurnActor
{
    GridMovement mover;

    protected override void Start()
    {
        mover = GetComponent<GridMovement>();
        base.Start();

    }
    public override IEnumerator Act()
    {
        mover.Move(0, 1);
        initiative = TurnManager.Instance.GetLastActorInitiative() + 1;
        yield return new WaitForSeconds(0.2f);
    }
}
