using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private GameObject CombineUi;
    [SerializeField] private GameObject unitUi;
    [SerializeField] private GameObject unitStateUi;
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
    [SerializeField] private TextMeshProUGUI curUnitName;
    [SerializeField] private TextMeshProUGUI curUnitAtk;
    [SerializeField] private TextMeshProUGUI curUnitBuffAtk;
    [SerializeField] private TextMeshProUGUI curUnitAtkSpeed;
    [SerializeField] private TextMeshProUGUI curUnitDefDebuff;
    [SerializeField] private TextMeshProUGUI curUnitSpeedDebuff;
    [SerializeField] private TextMeshProUGUI curUnitBuff;
    [SerializeField] private TextMeshProUGUI curUnitStun;
    [SerializeField] private TextMeshProUGUI curUnitSkill;
    [SerializeField] private TextMeshProUGUI characteristicText;
    [SerializeField] private GameObject sellButton;
    [SerializeField] private GameObject recipeUi;
    [SerializeField] private GameObject menuUi;
    private bool isRecipeUi = false;
    private bool isMenu = false;
    public Dictionary<Rating, Color> unitDic = new Dictionary<Rating, Color>();
    private Unit curUnit;

    private new void Awake()
    {
        base.Awake();
        recipeUi.SetActive(false);
        unitDic.Add(Rating.Normal, Color.gray);
        unitDic.Add(Rating.Rare, Color.white);
        unitDic.Add(Rating.Epic, Color.yellow);
        unitDic.Add(Rating.Unique, Color.blue);
        unitDic.Add(Rating.Legendary, Color.red);
    }

    private void Update()
    {
        if(curUnit != null)
        {
            curUnitBuffAtk.text = "(+" + (curUnit.minAtk - curUnit.unitRecipe.minAtk).ToString() + " ~ +" + (curUnit.maxAtk - curUnit.unitRecipe.maxAtk).ToString() + ")";
        }
    }

    public void AddUnit(Unit unit)
    {
        curUnit = unit;
        unitUi.SetActive(true);
        unitStateUi.SetActive(true);
        CombineUi.SetActive(true);
        closeUi.SetActive(true);
        unitImage.sprite = unit.unitRecipe.unitImage;
        curUnitName.text = unit.unitRecipe.unitName;
        float min = unit.unitRecipe.minAtk * ((unit.unitRecipe.level + 1) * 0.2f);
        float max = unit.unitRecipe.maxAtk * ((unit.unitRecipe.level + 1) * 0.2f);
        curUnitAtk.text = (unit.unitRecipe.minAtk + min).ToString() + " ~ " + (unit.unitRecipe.maxAtk + max).ToString();
        curUnitAtkSpeed.text = (1 / unit.unitRecipe.atkCoolTime).ToString("N1");
        curUnitDefDebuff.text = unit.unitRecipe.debuff.ToString("N1");
        curUnitSpeedDebuff.text = (unit.unitRecipe.speedDebuff * 100).ToString("N0") + "%";
        curUnitBuff.text = unit.unitRecipe.buff.ToString("N1");
        curUnitStun.text = unit.unitRecipe.stun.ToString("N1");
        curUnitSkill.text = (unit.unitRecipe.skillDamage * 100).ToString("N0") + "%";
        characteristicText.text = unit.unitRecipe.unitCharacteristic;

        if(unit.unitRecipe.unitRating < Rating.Legendary)
        {
            sellButton.SetActive(true);
        }
        else
        {
            sellButton.SetActive(false);
        }

        for (int i = 0; i < recipeImageA.Length; i++)
        {
            if (i < unit.unitRecipe.recipeA.Length && unit.unitRecipe.recipeA[i] != null)
            {
                recipeImageA[i].sprite = unit.unitRecipe.recipeA[i].unitImage;
                starImageA[i].color = unitDic[unit.unitRecipe.recipeA[i].unitRating];
                recipeImageA[i].gameObject.SetActive(true);
                unitANames[i].text = unit.unitRecipe.recipeA[i].unitName;
            }
            else
            {
                recipeImageA[i].gameObject.SetActive(false);
            }

            if (i < unit.unitRecipe.recipeB.Length && unit.unitRecipe.recipeB[i] != null)
            {
                recipeImageB[i].sprite = unit.unitRecipe.recipeB[i].unitImage;
                starImageB[i].color = unitDic[unit.unitRecipe.recipeB[i].unitRating];
                recipeImageB[i].gameObject.SetActive(true);
                unitBNames[i].text = unit.unitRecipe.recipeB[i].unitName;
            }
            else
            {
                recipeImageB[i].gameObject.SetActive(false);
            }
        }

        if(unit.unitRecipe.nextUnitA != null)
        {
            resultImageA.gameObject.SetActive(true);
            resultImageA.sprite = unit.unitRecipe.nextUnitA.GetComponentInChildren<Unit>().unitRecipe.unitImage;
            starA.color = unitDic[unit.unitRecipe.nextUnitA.GetComponentInChildren<Unit>().unitRecipe.unitRating];
            unitAName.text = unit.unitRecipe.nextUnitA.GetComponentInChildren<Unit>().unitRecipe.unitName;
        }
        else
        {
            resultImageA.gameObject.SetActive(false);
        }
        if(unit.unitRecipe.nextUnitB != null)
        {
            resultImageB.gameObject.SetActive(true);
            resultImageB.sprite = unit.unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitImage;
            starB.color = unitDic[unit.unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitRating];
            unitBName.text = unit.unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitName;
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
            recipeUi.SetActive(true);
            menuUi.SetActive(false);
            isMenu = false;
        }
        else
        {
            recipeUi.SetActive(false);
            if (GameManager.instance.isX2)
            {
                Time.timeScale = 2;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void MenuOnOff()
    {
        isMenu = !isMenu;

        if(isMenu)
        {
            menuUi.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            menuUi.SetActive(false);

            if(GameManager.instance.isX2)
            {
                Time.timeScale = 2;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void EndGame()
    {
        unitUi.SetActive(false);
        closeUi.SetActive(false);
        menuUi.SetActive(false);
        recipeUi.SetActive(false);
        isRecipeUi = false;
        isMenu = false;
        curUnit = null;
        Time.timeScale = 1;
    }

    public void CloseUi()
    {
        unitUi.SetActive(false);
        CombineUi.SetActive(false);
        unitStateUi.SetActive(false);
        closeUi.SetActive(false);
        curUnit = null;
    }
}
