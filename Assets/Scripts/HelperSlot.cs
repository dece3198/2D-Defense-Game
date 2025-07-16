using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelperSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnitRecipe unitRecipe;
    public Image unitImage;
    public Image unitrating;
    [SerializeField] private Outline outline;
    private Vector3 originalScales;
    [SerializeField] private bool isCombine = false;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        originalScales = transform.localScale;
    }

    public void FindUnit()
    {
        // 1. 필요한 유닛 몇 마리인지 세기
        Dictionary<UnitRecipe, int> neededUnits = new Dictionary<UnitRecipe, int>();
        foreach (var recipeUnit in unitRecipe.recipes)
        {
            if (neededUnits.ContainsKey(recipeUnit))
                neededUnits[recipeUnit]++;
            else
                neededUnits[recipeUnit] = 1;
        }

        // 2. 현재 내가 가진 유닛 세기
        Dictionary<UnitRecipe, int> myUnits = new Dictionary<UnitRecipe, int>();
        foreach (var obj in UnitSpawner.instance.unitList)
        {
            UnitRecipe recipe = obj.GetComponentInChildren<Unit>().unitRecipe;

            if (myUnits.ContainsKey(recipe))
                myUnits[recipe]++;
            else
                myUnits[recipe] = 1;
        }

        // 3. 조합 가능한지 체크
        bool isCombinable = true;
        foreach (var pair in neededUnits)
        {
            UnitRecipe needed = pair.Key;
            int requiredCount = pair.Value;

            int haveCount = myUnits.ContainsKey(needed) ? myUnits[needed] : 0;

            if (haveCount < requiredCount)
            {
                isCombinable = false;
                break;
            }
        }

        // 4. 이미 조합된 유닛이 있는지 확인 (이것도 1개만 있으면 됨)
        bool alreadyHave = myUnits.ContainsKey(unitRecipe) && myUnits[unitRecipe] > 0;

        // 5. 색상 설정
        if (alreadyHave)
        {
            outline.effectColor = Color.green; //  이미 조합한 경우
            isCombine = false;
        }
        else if (isCombinable)
        {
            outline.effectColor = Color.yellow; //  조합 가능
            isCombine = true;
        }
        else
        {
            outline.effectColor = Color.red; //  조합 불가능
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
