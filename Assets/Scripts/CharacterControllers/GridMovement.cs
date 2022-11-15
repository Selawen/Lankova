using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : GridObject
{
    public bool Move(Vector2 movement)
    {
        movement = new Vector2((int)movement.x, (int)movement.y);

        if (!GridManager.Instance.IsWithinBounds(movement + GridPosition))
        {
            return false;
        }
        
        if(GridManager.Instance.IsSolid(GridPosition + movement))
        {
            return false;
        }

        GridPosition += movement;
        return true;
    }

    public bool Move(int x, int y)
    {
        return Move(new Vector2(x, y));
    }
}
