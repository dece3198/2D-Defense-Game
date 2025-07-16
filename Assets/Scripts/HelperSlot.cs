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
        // 1. �ʿ��� ���� �� �������� ����
        Dictionary<UnitRecipe, int> neededUnits = new Dictionary<UnitRecipe, int>();
        foreach (var recipeUnit in unitRecipe.recipes)
        {
            if (neededUnits.ContainsKey(recipeUnit))
                neededUnits[recipeUnit]++;
            else
                neededUnits[recipeUnit] = 1;
        }

        // 2. ���� ���� ���� ���� ����
        Dictionary<UnitRecipe, int> myUnits = new Dictionary<UnitRecipe, int>();
        foreach (var obj in UnitSpawner.instance.unitList)
        {
            UnitRecipe recipe = obj.GetComponentInChildren<Unit>().unitRecipe;

            if (myUnits.ContainsKey(recipe))
                myUnits[recipe]++;
            else
                myUnits[recipe] = 1;
        }

        // 3. ���� �������� üũ
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

        // 4. �̹� ���յ� ������ �ִ��� Ȯ�� (�̰͵� 1���� ������ ��)
        bool alreadyHave = myUnits.ContainsKey(unitRecipe) && myUnits[unitRecipe] > 0;

        // 5. ���� ����
        if (alreadyHave)
        {
            outline.effectColor = Color.green; //  �̹� ������ ���
            isCombine = false;
        }
        else if (isCombinable)
        {
            outline.effectColor = Color.yellow; //  ���� ����
            isCombine = true;
        }
        else
        {
            outline.effectColor = Color.red; //  ���� �Ұ���
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
