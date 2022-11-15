using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<GridObject>[,] grid;
    public int gridWidth, gridHeight, cellSize;
    public static GridManager Instance;

    private void Awake()
    {
        Instance = this;

        grid = new List<GridObject>[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = new List<GridObject>();
            }
        }
    }
    public bool HasObject(int x, int y)
    {
        return grid[x, y].Count > 0;
    }
        public bool HasObject(Vector2 position)
    {
        return HasObject((int)position.x, (int)position.y);
    }

    public bool IsSolid(int x, int y)
    {
        return !grid[x, y].TrueForAll(obj => !obj.isSolid);
    }

    public bool IsSolid(Vector2 position)
    {
        return IsSolid((int)position.x, (int)position.y);
    }

    public bool IsWithinBounds(Vector2 position)
    {
        return IsWithinBounds((int)position.x, (int)position.y);
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x < gridWidth && x >= 0 && y < gridHeight && y >= 0;
    }
}

