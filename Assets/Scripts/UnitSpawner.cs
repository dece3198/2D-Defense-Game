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
    public List<GameObject> unitList = new List<GameObject>();
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
        if(GameManager.instance.Gold >= 1)
        {
            GameManager.instance.Gold -= 1;
            foreach (var cellPos in tilePosList)
            {
                if (!usedTiles.Contains(cellPos))
                {
                    int rand = Random.Range(0, units.Length);
                    GameObject tower = Instantiate(units[rand], transform);
                    tower.GetComponentInChildren<Unit>().currentTilePos = cellPos;
                    unitList.Add(tower);
                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                    tower.transform.position = worldPos;
                    usedTiles.Add(cellPos);
                    return;
                }
            }

            Debug.Log("더 이상 배치할 타일이 없을 때");
        }

        
    }

    public void CombineA()
    {
        List<GameObject> matchedTowers = new List<GameObject>();
        UnitRecipe[] required = curTower.unitRecipe.recipeA;
        List<GameObject> tempList = new List<GameObject>(unitList);

        foreach(var req in required)
        {
            var match = tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>().unitRecipe == req);
            if(match != null)
            {
                matchedTowers.Add(match);
                tempList.Remove(match);
            }
        }

        if (matchedTowers.Count == curTower.unitRecipe.recipeA.Length)
        {
            Vector3Int spawnCell = curTower.currentTilePos;

            foreach (var t in matchedTowers)
            {
                usedTiles.Remove(t.GetComponentInChildren<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(curTower.unitRecipe.nextUnitA, transform);
            Unit towerComp = tower.GetComponentInChildren<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(tower);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            tower.transform.position = worldPos;
            usedTiles.Add(spawnCell);
        }
        else
        {
            List<UnitRecipe> existing = unitList.Where(t => t != curTower.gameObject).Select(t => t.GetComponentInChildren<Unit>().unitRecipe).ToList();
            List<UnitRecipe> missings = new List<UnitRecipe>();
            foreach (var req in required.Distinct())
            {
                int requiredCount = required.Count(r => r == req);
                int existingCount = existing.Count(e => e == req);

                if (existingCount < requiredCount)
                {
                    int missingAmount = requiredCount - existingCount;

                    for (int i = 0; i < missingAmount; i++)
                    {
                        missings.Add(req);
                    }
                }
            }

            if (missings.Count != 0)
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
        List<GameObject> matchedTowers = new List<GameObject>();
        UnitRecipe[] required = curTower.unitRecipe.recipeB;
        List<GameObject> tempList = new List<GameObject>(unitList);

        foreach (var req in required)
        {
            var match = tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>().unitRecipe == req);
            if (match != null)
            {
                matchedTowers.Add(match);
                tempList.Remove(match);
            }
        }

        if (matchedTowers.Count == curTower.unitRecipe.recipeB.Length)
        {
            Vector3Int spawnCell = curTower.currentTilePos;

            foreach (var t in matchedTowers)
            {
                usedTiles.Remove(t.GetComponentInChildren<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(curTower.unitRecipe.nextUnitB, transform);
            Unit towerComp = tower.GetComponentInChildren<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(tower);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            tower.transform.position = worldPos;
            usedTiles.Add(spawnCell);
        }
        else
        {
            List<UnitRecipe> existing = unitList.Where(t => t != curTower.gameObject).Select(t => t.GetComponentInChildren<Unit>().unitRecipe).ToList();
            List<UnitRecipe> missings = new List<UnitRecipe>();

            foreach (var req in required.Distinct())
            {
                int requiredCount = required.Count(r => r == req);
                int existingCount = existing.Count(e => e == req);

                if (existingCount < requiredCount)
                {
                    int missingAmount = requiredCount - existingCount;

                    for (int i = 0; i < missingAmount; i++)
                    {
                        missings.Add(req);
                    }
                }
            }
            if (missings.Count != 0)
            {
                var grouped = missings.GroupBy(r => r.unitName);
                string missingText = string.Join(", ", grouped.Select(g => g.Count() > 1 ? $"{g.Key} X{g.Count()}" : g.Key));
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


