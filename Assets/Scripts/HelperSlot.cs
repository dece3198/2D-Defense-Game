using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelperSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnitRecipe unitRecipe;
    public Image unitImage;
    public Image unitRating;
    [SerializeField] private Outline outline;
    private Vector3 originalScales;
    [SerializeField] private bool isCombine = false;
    [SerializeField] private TextMeshProUGUI countText;
    public int recipeIndex;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        originalScales = transform.localScale;
    }

    public void SetRecipe(UnitRecipe recipe, int count)
    {
        unitRecipe = recipe;
        recipeIndex = count;

        if (unitRecipe == null)
        {
            gameObject.SetActive(false);
            return;
        }
        unitRating.color = UiManager.instance.unitDic[unitRecipe.unitRating];
        gameObject.SetActive(true);
        unitImage.sprite = recipe.unitImage;
        FindUnit();
    }

    public void FindUnit()
    {

        if(unitRecipe == null)
        {
            gameObject.SetActive(false);
            return;
        }
        // 1. 필요한 유닛 개수를 세기
        Dictionary<UnitRecipe, int> neededCount = new Dictionary<UnitRecipe, int>();
        foreach (var recipe in unitRecipe.recipes)
        {
            if (!neededCount.ContainsKey(recipe))
                neededCount[recipe] = 1;
            else
                neededCount[recipe]++;
        }

        // 2. 내가 가진 유닛 개수를 세기
        Dictionary<UnitRecipe, int> haveCount = new Dictionary<UnitRecipe, int>();
        foreach (var obj in UnitSpawner.instance.unitList)
        {
            var unit = obj.GetComponentInChildren<Unit>().unitRecipe;
            if (!haveCount.ContainsKey(unit))
                haveCount[unit] = 1;
            else
                haveCount[unit]++;
        }

        // 3. 비교해서 조합 가능한지 판단
        bool isCombinable = true;

        foreach (var pair in neededCount)
        {
            UnitRecipe needUnit = pair.Key;
            int required = pair.Value;
            int owned = haveCount.ContainsKey(needUnit) ? haveCount[needUnit] : 0;

            if (owned < required)
            {
                isCombinable = false;
                break;
            }
        }

        if(unitRecipe != null && unitRecipe.unitRating > Rating.Normal)
        {
            if (isCombinable)
            {
                outline.effectColor = Color.yellow;
                isCombine = true;
            }
            else
            {
                outline.effectColor = Color.red;
                isCombine = false;
            }
        }

        int haveCountz = 0;
        foreach (var obj in UnitSpawner.instance.unitList)
        {
            var unit = obj.GetComponentInChildren<Unit>().unitRecipe;
            if (unit == unitRecipe)
                haveCountz++;
        }

        countText.text = $"{Mathf.Min(haveCountz, recipeIndex)} / {recipeIndex}";

        if(haveCountz == recipeIndex)
        {
            outline.effectColor = Color.green;
            isCombine = false;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales * 0.9f, 0.1f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad);
            if(isCombine)
            {
                UnitSpawner.instance.Combine(unitRecipe);
            }
        }
    }
}
