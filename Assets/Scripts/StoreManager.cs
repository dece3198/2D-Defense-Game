using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class StoreManager : Singleton<StoreManager>
{
    [SerializeField] private UnitRecipe[] normalUnits;
    [SerializeField] private UnitRecipe[] rareUnits;
    [SerializeField] private UnitRecipe[] epicUnits;
    [SerializeField] private UnitRecipe[] uniqueUnits;
    [SerializeField] private UnitRecipe[] legendaryUnits;
    [SerializeField] private UnitStroeSlot[] normalSlots;
    [SerializeField] private UnitStroeSlot[] rareSlots;
    [SerializeField] private UnitStroeSlot[] epicSlots;
    [SerializeField] private UnitStroeSlot[] uniqueSlots;
    [SerializeField] private UnitStroeSlot[] legendarySlots;
    [SerializeField] private TextMeshProUGUI missing;
    private Dictionary<UnitRating, (int, int)> priceDic = new Dictionary<UnitRating, (int, int)>();
    

    private new void Awake()
    {
        base.Awake();
        priceDic.Add(UnitRating.Normal, (1, 5));
        priceDic.Add(UnitRating.Rare, (5, 10));
        priceDic.Add(UnitRating.Epic, (10, 20));
        priceDic.Add(UnitRating.Unique, (25, 50));
        priceDic.Add(UnitRating.Legendary, (50, 100));
    }

    public void NewDay()
    {
        DataManager.instance.curData.isTimeCompensation = false;
        Shuffle(normalUnits, normalSlots);
        Shuffle(rareUnits, rareSlots);
        Shuffle(epicUnits, epicSlots);
        Shuffle(uniqueUnits, uniqueSlots);
        Shuffle(legendaryUnits, legendarySlots);
    }

    private void Shuffle(UnitRecipe[] units, UnitStroeSlot[] unitSlots)
    {
        List<UnitRecipe> unitPool = new List<UnitRecipe>(units);

        for (int i = 0; i < unitPool.Count; i++)
        {
            int randomIndex = Random.Range(i, unitPool.Count);
            (unitPool[i], unitPool[randomIndex]) = (unitPool[randomIndex], unitPool[i]);
        }

        for (int i = 0; i < unitSlots.Length; i++)
        {
            int randomValue = Random.Range((priceDic[unitPool[i].unitRating]).Item1, (priceDic[unitPool[i].unitRating]).Item2);
            unitSlots[i].AddUnit(unitPool[i], randomValue);
            unitSlots[i].buyImage.SetActive(false);
            unitSlots[i].isBuy = true;
        }
    }

    public void ShowLackText()
    {
        StartCoroutine(shakeCo());
    }

    private IEnumerator shakeCo()
    {
        missing.gameObject.SetActive(true);
        missing.text = "루비가 부족합니다.";
        missing.transform.DOShakePosition(1f, new Vector3(1.5f, 1.5f, 0), 30, 90f, false, true);
        yield return new WaitForSeconds(1f);
        missing.gameObject.SetActive(false);
    }
}
