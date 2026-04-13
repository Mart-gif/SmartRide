using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSettings : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Vector2 cellSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 gridOrigin = Vector2.zero;

    public Vector2 CellSize => cellSize;
    public Vector2 GridOrigin => gridOrigin;

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = gridOrigin.x + gridPosition.x * cellSize.x;
        float y = gridOrigin.y + gridPosition.y * cellSize.y;
        return new Vector3(x, y, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / cellSize.x);
        int y = Mathf.RoundToInt((worldPosition.y - gridOrigin.y) / cellSize.y);
        return new Vector2Int(x, y);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
