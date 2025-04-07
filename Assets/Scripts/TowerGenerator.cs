using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public List<Vector3Int> tilePosList = new List<Vector3Int>();

    private void Start()
    {
        GetAllTilePositions();

        tilePosList.Sort((a, b) =>
        {
            int ycompare = -a.y.CompareTo(b.y);
            return ycompare != 0 ? ycompare : a.x.CompareTo(b.x);
        });

        Vector3Int cellPos = tilePosList[18];
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
    }


    public void GetAllTilePositions()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilePosList.Add(pos);
            }
        }
    }
}
