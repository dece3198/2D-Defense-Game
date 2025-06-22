using DG.Tweening;
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
    private Dictionary<UnitRecipe, Stack<GameObject>> unitPool = new();

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
    //랜덤으로 유닛소환
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
                    GameObject unit = ExitPool(units[rand]);
                    unit.SetActive(true);
                    unit.GetComponentInChildren<Unit>().currentTilePos = cellPos;
                    unitList.Add(unit);
                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                    unit.transform.position = worldPos;
                    usedTiles.Add(cellPos);
                    return;
                }
            }

            Debug.Log("더 이상 배치할 타일이 없을 때");
        }

        
    }
    //조합A버튼을 누를시 알맞는 유닛 조합 없을시 missingText로 없는 유닛표시
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
                EnterPool(t.GetComponentInChildren<Unit>());
            }

            GameObject _unit = ExitPool(unit.unitRecipe.nextUnitA);
            _unit.SetActive(true);
            Unit towerComp = _unit.GetComponentInChildren<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(_unit);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            _unit.transform.position = worldPos;
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
    //조합B버튼을 누를시 알맞는 유닛 조합 없을시 missingText로 없는 유닛표시
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
                EnterPool(t.GetComponentInChildren<Unit>());
            }
            GameObject _unit = ExitPool(unit.unitRecipe.nextUnitB);
            _unit.SetActive(true);
            Unit towerComp = _unit.GetComponentInChildren<Unit>();
            towerComp.currentTilePos = spawnCell;
            unitList.Add(_unit);
            Vector3 worldPos = tilemap.GetCellCenterWorld(spawnCell);
            _unit.transform.position = worldPos;
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
    
    //유닛 구매
    public void BuyUnit(int value)
    {
        if (GameManager.instance.Jam > 0)
        {
            GameManager.instance.Jam -= 1;
            foreach (var cellPos in tilePosList)
            {
                if (!usedTiles.Contains(cellPos))
                {
                    GameObject unit = ExitPool(units[value]);
                    unit.SetActive(true);
                    unit.GetComponentInChildren<Unit>().currentTilePos = cellPos;
                    unitList.Add(unit);
                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                    unit.transform.position = worldPos;
                    usedTiles.Add(cellPos);
                    return;
                }
            }

            Debug.Log("더 이상 배치할 타일이 없을 때");
        }
    }

    //33%확률로 유닛판매
    public void SellUnit()
    {
        if(Random.value < 0.33)
        {
            GameManager.instance.Gold += unitPriceDic[curUnit.GetComponentInChildren<Unit>().unitRecipe.unitRating];
        }
        usedTiles.Remove(curUnit.GetComponentInChildren<Unit>().currentTilePos);
        unitList.Remove(curUnit);
        EnterPool(curUnit.GetComponentInChildren<Unit>());
        UiManager.instance.CloseUi();
    }
    //게임종료시 유닛제거
    public void EndGame()
    {
        for (int i = unitList.Count - 1; i >= 0; i--)
        {
            Unit _unit = unitList[i].GetComponentInChildren<Unit>();

            usedTiles.Remove(_unit.currentTilePos);
            EnterPool(_unit);
            unitList.RemoveAt(i);
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



    private GameObject ExitPool(GameObject _unit)
    {
        UnitRecipe recipe = _unit.GetComponentInChildren<Unit>().unitRecipe;
        if (unitPool.ContainsKey(recipe) && unitPool[recipe].Count > 0)
        {
            return unitPool[recipe].Pop();
        }

        return Instantiate(_unit);
    }

    private void EnterPool(Unit _unit)
    {
        GameObject parent = _unit.transform.parent.GetComponent<SPUM_Prefabs>().gameObject;

        if (!unitPool.ContainsKey(_unit.unitRecipe))
            unitPool[_unit.unitRecipe] = new Stack<GameObject>();

        unitPool[_unit.unitRecipe].Push(parent);
        parent.SetActive(false);
    }

    private IEnumerator shakeCo()
    {
        missing.gameObject.SetActive(true);
        missing.transform.DOShakePosition(1f, new Vector3(1f, 1f, 0), 30, 90f, false, true);
        yield return new WaitForSeconds(1f);
        missing.gameObject.SetActive(false);
    }
}


