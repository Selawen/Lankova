using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlayerActor : TurnActor
{
    GridMovement mover;
    protected override void Start()
    {
        mover = GetComponent<GridMovement>();
        base.Start();
    }
    public override IEnumerator Act()
    {
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.W)) { if (mover.Move(Vector2.up)) { break; } }
            if (Input.GetKeyDown(KeyCode.S)) { if (mover.Move(Vector2.down)) { break; } }
            if (Input.GetKeyDown(KeyCode.A)) { if (mover.Move(Vector2.left)) { break; } }
            if (Input.GetKeyDown(KeyCode.D)) { if (mover.Move(Vector2.right)) { break; } }
            yield return null;
        }
        initiative += TurnManager.Instance.GetLastActorInitiative();
    }
}
