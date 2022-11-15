using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector2 startPos;
    Vector2 gridPosition;
    public Vector2 GridPosition
    {
        get
        {
            return gridPosition;
        }
        set
        {
            if (GridManager.Instance.IsWithinBounds(value))
            {
                GridManager.Instance.grid[(int)gridPosition.x, (int)gridPosition.y].Remove(this);
                gridPosition = value;
                GridManager.Instance.grid[(int)gridPosition.x, (int)gridPosition.y].Add(this);
                transform.position = GridManager.Instance.transform.position +
                    new Vector3(gridPosition.x * GridManager.Instance.cellSize, 0, gridPosition.y * GridManager.Instance.cellSize);
            }
        }

    }
    public bool isSolid;

    protected void Start()
    {
        GridPosition = startPos;
    }

    protected void OnDestroy()
    {
        GridManager.Instance.grid[(int)gridPosition.x, (int)gridPosition.y].Remove(this);
    }
}
