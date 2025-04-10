using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerSpawner : Singleton<TowerSpawner>
{
    public Tilemap tilemap;
    public GameObject[] towers;
    public HashSet<Vector3Int> usedTiles = new HashSet<Vector3Int>();
    public List<Vector3Int> tilePosList = new List<Vector3Int>();

    private void Start()
    {
        GetAllTilePositions();

        tilePosList.Sort((a, b) =>
        {
            int ycompare = -a.y.CompareTo(b.y);
            return ycompare != 0 ? ycompare : a.x.CompareTo(b.x);
        });
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

    public void RandomSpawn()
    { 

        foreach(var cellPos in tilePosList)
        {
            if(!usedTiles.Contains(cellPos))
            {
                int rand = Random.Range(0, towers.Length);
                GameObject tower = Instantiate(towers[rand], transform);
                tower.GetComponentInChildren<Tower>().currentTilePos = cellPos;
                Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                tower.transform.position = worldPos;
                usedTiles.Add(cellPos);
                return;
            }
        }

        Debug.Log("더 이상 배치할 타일이 없을 때");
    }
}
