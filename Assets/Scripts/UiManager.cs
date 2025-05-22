using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private GameObject towerUi;
    [SerializeField] private GameObject closeUi;
    [SerializeField] private Image unitImage;
    [SerializeField] private Image[] recipeImageA;
    [SerializeField] private Image[] recipeImageB;
    [SerializeField] private Image[] starImageA;
    [SerializeField] private Image[] starImageB;
    [SerializeField] private TextMeshProUGUI[] unitANames;
    [SerializeField] private TextMeshProUGUI[] unitBNames;
    [SerializeField] private Image resultImageA;
    [SerializeField] private Image resultImageB;
    [SerializeField] private GameObject buttonA;
    [SerializeField] private GameObject buttonB;
    [SerializeField] private Image starA;
    [SerializeField] private Image starB;
    [SerializeField] private TextMeshProUGUI unitAName;
    [SerializeField] private TextMeshProUGUI unitBName;
    [SerializeField] private GameObject RecipeUi;
    private bool isRecipeUi = false;
    private Dictionary<UnitRating, Color> unitDic = new Dictionary<UnitRating, Color>();

    private new void Awake()
    {
        base.Awake();
        unitDic.Add(UnitRating.Normal, Color.gray);
        unitDic.Add(UnitRating.Rare, Color.white);
        unitDic.Add(UnitRating.Epic, Color.yellow);
        unitDic.Add(UnitRating.Unique, Color.blue);
        unitDic.Add(UnitRating.Legendary, Color.red);
    }

    public void AddUnit(UnitRecipe unitRecipe)
    {
        towerUi.SetActive(true);
        closeUi.SetActive(true);
        unitImage.sprite = unitRecipe.unitImage;
        for(int i = 0; i < recipeImageA.Length; i++)
        {
            if (i < unitRecipe.recipeA.Length && unitRecipe.recipeA[i] != null)
            {
                recipeImageA[i].sprite = unitRecipe.recipeA[i].unitImage;
                starImageA[i].color = unitDic[unitRecipe.recipeA[i].unitRating];
                recipeImageA[i].gameObject.SetActive(true);
                unitANames[i].text = unitRecipe.recipeA[i].unitName;
            }
            else
            {
                recipeImageA[i].gameObject.SetActive(false);
            }

            if (i < unitRecipe.recipeB.Length && unitRecipe.recipeB[i] != null)
            {
                recipeImageB[i].sprite = unitRecipe.recipeB[i].unitImage;
                starImageB[i].color = unitDic[unitRecipe.recipeB[i].unitRating];
                recipeImageB[i].gameObject.SetActive(true);
                unitBNames[i].text = unitRecipe.recipeB[i].unitName;
            }
            else
            {
                recipeImageB[i].gameObject.SetActive(false);
            }
        }

        if(unitRecipe.nextUnitA != null)
        {
            resultImageA.gameObject.SetActive(true);
            resultImageA.sprite = unitRecipe.nextUnitA.GetComponentInChildren<Unit>().unitRecipe.unitImage;
            starA.color = unitDic[unitRecipe.nextUnitA.GetComponentInChildren<Unit>().unitRecipe.unitRating];
            unitAName.text = unitRecipe.nextUnitA.GetComponentInChildren<Unit>().unitRecipe.unitName;
        }
        else
        {
            resultImageA.gameObject.SetActive(false);
        }
        if(unitRecipe.nextUnitB != null)
        {
            resultImageB.gameObject.SetActive(true);
            resultImageB.sprite = unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitImage;
            starB.color = unitDic[unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitRating];
            unitBName.text = unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitName;
        }
        else
        {
            resultImageB.gameObject.SetActive(false);
        }

        buttonA.SetActive(recipeImageA.Any(img => img.gameObject.activeSelf) || resultImageA.gameObject.activeSelf);
        buttonB.SetActive(recipeImageB.Any(img => img.gameObject.activeSelf) || resultImageB.gameObject.activeSelf);
    }

    public void RecipeUiOnOff()
    {
        isRecipeUi = !isRecipeUi;

        if(isRecipeUi)
        {
            RecipeUi.SetActive(true);
        }
        else
        {
            RecipeUi.SetActive(false);
        }
    }

    public void CloseUi()
    {
        towerUi.SetActive(false);
        closeUi.SetActive(false);
    }
}
