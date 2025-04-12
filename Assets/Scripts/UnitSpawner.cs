using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitSpawner : Singleton<UnitSpawner>
{
    public Tilemap tilemap;
    public GameObject[] units;
    public HashSet<Vector3Int> usedTiles = new HashSet<Vector3Int>();
    public List<Vector3Int> tilePosList = new List<Vector3Int>();
    public List<UnitRecipe> unitList = new List<UnitRecipe>();
    public Unit curTower;
    [SerializeField] private TextMeshProUGUI missing;

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
                int rand = Random.Range(0, units.Length);
                GameObject tower = Instantiate(units[rand], transform);
                tower.GetComponentInChildren<Unit>().currentTilePos = cellPos;
                unitList.Add(tower.GetComponentInChildren<Unit>().unitRecipe);
                Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                tower.transform.position = worldPos;
                usedTiles.Add(cellPos);
                return;
            }
        }

        Debug.Log("더 이상 배치할 타일이 없을 때");
    }

    public void CombineA()
    {
        var matchedTowers = unitList.Where(t => curTower.unitRecipe.recipeA.Contains(t.GetComponent<Unit>().unitRecipe)).ToList();
        if (matchedTowers.Count == curTower.unitRecipe.recipeA.Length)
        {
            Vector3Int spawnCell = curTower.currentTilePos;

            foreach (var t in matchedTowers)
            {
                usedTiles.Remove(t.GetComponent<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(curTower.unitRecipe.nextTowerA, transform);
            Unit towerComp = tower.GetComponent<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(towerComp.unitRecipe);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            tower.transform.position = worldPos;
            usedTiles.Add(spawnCell);
        }
        else
        {
            UnitRecipe[] required = curTower.unitRecipe.recipeA;
            List<UnitRecipe> existing = unitList.Select(t => t.GetComponent<Unit>().unitRecipe).ToList();
            List<UnitRecipe> missings = required.Where(req => !existing.Contains(req)).ToList();
            if(missings.Count != 0)
            {
                var grouped = missings.GroupBy(r => r.unitName);
                string missingText = string.Join(", ", grouped.Select(g => g.Count() > 1 ? $"{g.Key} X{g.Count()}" : g.Key));
                missing.text = "부족한 유닛 : " + missingText;
                StartCoroutine(shakeCo());
            }
        }
    }

    public void CombineB()
    {
        var matchedTowers = unitList.Where(t => curTower.unitRecipe.recipeB.Contains(t.GetComponent<Unit>().unitRecipe)).ToList();
        if (matchedTowers.Count == curTower.unitRecipe.recipeB.Length)
        {
            Vector3Int spawnCell = curTower.currentTilePos;

            foreach (var t in matchedTowers)
            {
                usedTiles.Remove(t.GetComponent<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(curTower.unitRecipe.nextTowerB, transform);
            Unit towerComp = tower.GetComponent<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(towerComp.unitRecipe);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            tower.transform.position = worldPos;
            usedTiles.Add(spawnCell);
        }
        else
        {
            UnitRecipe[] required = curTower.unitRecipe.recipeB;
            List<UnitRecipe> existing = unitList.Select(t => t.GetComponent<Unit>().unitRecipe).ToList();
            List<UnitRecipe> missings = required.Where(req => !existing.Contains(req)).ToList();
            if (missings.Count != 0)
            {
                string missingText = string.Join(",", missings.Select(r => r.unitName));
                missing.text = "부족한 유닛 : " + missingText;
                StartCoroutine(shakeCo());
            }
        }
    }

    private IEnumerator shakeCo()
    {
        missing.gameObject.SetActive(true);
        float time = 1f;
        Vector3 origingPos = missing.transform.position;
        while(time > 0)
        {
            time -= Time.deltaTime;
            missing.transform.position = Random.insideUnitSphere * 1f + origingPos;
            yield return null;
        }
        missing.transform.position = origingPos;
        missing.gameObject.SetActive(false);
    }
}


