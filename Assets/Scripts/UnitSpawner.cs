using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitSpawner : Singleton<UnitSpawner>
{
    public Tilemap tilemap;
    public GameObject[] units;
    public HashSet<Vector3Int> usedTiles = new HashSet<Vector3Int>();
    public List<Vector3Int> tilePosList = new List<Vector3Int>();
    public List<GameObject> unitList = new List<GameObject>();
    public Unit curUnit;
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

            Debug.Log("�� �̻� ��ġ�� Ÿ���� ���� ��");
        }

        
    }

    public void CombineA()
    {
        List<GameObject> matchedUnits = new List<GameObject>() { curUnit.gameObject };
        UnitRecipe[] required = curUnit.unitRecipe.recipeA;
        List<UnitRecipe> requiredList = new List<UnitRecipe>(required);
        requiredList.Remove(curUnit.unitRecipe);
        List<GameObject> tempList = new List<GameObject>(unitList);
        tempList.Remove(tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>() == curUnit));

        foreach (var req in requiredList)
        {
            var match = tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>().unitRecipe == req);
            if (match != null)
            {
                matchedUnits.Add(match);
                tempList.Remove(match);
            }
        }

        if (matchedUnits.Count == required.Length)
        {
            Vector3Int spawnCell = curUnit.currentTilePos;

            foreach (var t in matchedUnits)
            {
                usedTiles.Remove(t.GetComponentInChildren<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(curUnit.unitRecipe.nextUnitA, transform);
            Unit towerComp = tower.GetComponentInChildren<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(tower);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            tower.transform.position = worldPos;
            usedTiles.Add(spawnCell);
            UiManager.instance.CloseUi();
        }
        else
        {
            List<UnitRecipe> existing = unitList.Where(t => t != curUnit.gameObject).Select(t => t.GetComponentInChildren<Unit>().unitRecipe).ToList();
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
                missing.text = "������ ���� : " + missingText;
                StartCoroutine(shakeCo());
            }
        }
    }

    public void CombineB()
    {
        List<GameObject> matchedUnits = new List<GameObject>() { curUnit.gameObject };
        UnitRecipe[] required = curUnit.unitRecipe.recipeB;
        List<UnitRecipe> requiredList = new List<UnitRecipe>(required);
        requiredList.Remove(curUnit.unitRecipe);
        List<GameObject> tempList = new List<GameObject>(unitList);
        tempList.Remove(tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>() == curUnit));

        foreach (var req in requiredList)
        {
            var match = tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>().unitRecipe == req);
            if (match != null)
            {
                matchedUnits.Add(match);
                tempList.Remove(match);
            }
        }

        if (matchedUnits.Count == required.Length)
        {
            Vector3Int spawnCell = curUnit.currentTilePos;

            foreach (var t in matchedUnits)
            {
                usedTiles.Remove(t.GetComponentInChildren<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(curUnit.unitRecipe.nextUnitB, transform);
            Unit towerComp = tower.GetComponentInChildren<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(tower);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            tower.transform.position = worldPos;
            usedTiles.Add(spawnCell);
            UiManager.instance.CloseUi();
        }
        else
        {
            List<UnitRecipe> existing = unitList.Where(t => t != curUnit.gameObject).Select(t => t.GetComponentInChildren<Unit>().unitRecipe).ToList();
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
                missing.text = "������ ���� : " + missingText;
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


