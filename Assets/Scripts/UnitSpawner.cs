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
    public GameObject curUnit;
    [SerializeField] private TextMeshProUGUI missing;
    [SerializeField] private GameObject unitStore;
    private bool isStore = false;

    private Dictionary<UnitRating, int> unitPriceDic = new Dictionary<UnitRating, int>();

    private new void Awake()
    {
        base.Awake();
        unitPriceDic.Add(UnitRating.Normal, 1);
        unitPriceDic.Add(UnitRating.Rare, 2);
        unitPriceDic.Add(UnitRating.Epic, 3);
        unitPriceDic.Add(UnitRating.Unique, 9);
    }

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
        Unit unit = curUnit.GetComponentInChildren<Unit>();

        List<GameObject> matchedUnits = new List<GameObject>() { curUnit };
        UnitRecipe[] required = unit.unitRecipe.recipeA;
        List<UnitRecipe> requiredList = new List<UnitRecipe>(required);
        requiredList.Remove(unit.unitRecipe);
        List<GameObject> tempList = new List<GameObject>(unitList);
        tempList.Remove(tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>() == unit));

        foreach (var req in requiredList)
        {
            GameObject match = tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>().unitRecipe == req);
            if (match != null)
            {
                matchedUnits.Add(match);
                tempList.Remove(match);
            }
        }

        if (matchedUnits.Count == required.Length)
        {
            Vector3Int spawnCell = unit.currentTilePos;

            foreach (var t in matchedUnits)
            {
                usedTiles.Remove(t.GetComponentInChildren<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(unit.unitRecipe.nextUnitA, transform);
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
                missing.text = "부족한 유닛 : " + missingText;
                StartCoroutine(shakeCo());
            }
        }
    }

    public void CombineB()
    {
        Unit unit = curUnit.GetComponentInChildren<Unit>();

        List<GameObject> matchedUnits = new List<GameObject>() { curUnit};
        UnitRecipe[] required = unit.unitRecipe.recipeB;
        List<UnitRecipe> requiredList = new List<UnitRecipe>(required);
        requiredList.Remove(unit.unitRecipe);
        List<GameObject> tempList = new List<GameObject>(unitList);
        tempList.Remove(tempList.FirstOrDefault(t => t.GetComponentInChildren<Unit>() == unit));

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
            Vector3Int spawnCell = unit.currentTilePos;

            foreach (var t in matchedUnits)
            {
                usedTiles.Remove(t.GetComponentInChildren<Unit>().currentTilePos);
                unitList.Remove(t);
                Destroy(t);
            }
            GameObject tower = Instantiate(unit.unitRecipe.nextUnitB, transform);
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
                missing.text = "부족한 유닛 : " + missingText;
                StartCoroutine(shakeCo());
            }
        }
    }
    
    public void BuyUnit(int value)
    {
        if (GameManager.instance.Jam > 0)
        {
            GameManager.instance.Jam -= 1;
            foreach (var cellPos in tilePosList)
            {
                if (!usedTiles.Contains(cellPos))
                {
                    GameObject tower = Instantiate(units[value], transform);
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

    public void StoreUi()
    {
        isStore = !isStore;

        if(isStore)
        {
            unitStore.SetActive(true);
        }
        else
        {
            unitStore.SetActive(false);
        }
    }

    public void SellUnit()
    {
        if(Random.value < 0.33)
        {
            GameManager.instance.Gold += unitPriceDic[curUnit.GetComponentInChildren<Unit>().unitRecipe.unitRating];
        }
        usedTiles.Remove(curUnit.GetComponentInChildren<Unit>().currentTilePos);
        unitList.Remove(curUnit);
        Destroy(curUnit);
        UiManager.instance.CloseUi();
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


