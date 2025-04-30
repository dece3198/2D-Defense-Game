using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private GameObject towerUi;
    [SerializeField] private GameObject closeUi;
    [SerializeField] private Image unitImage;
    [SerializeField] private Image[] recipeImageA;
    [SerializeField] private Image[] recipeImageB;
    [SerializeField] private Image resultImageA;
    [SerializeField] private Image resultImageB;
    [SerializeField] private GameObject buttonA;
    [SerializeField] private GameObject buttonB;



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
                recipeImageA[i].gameObject.SetActive(true);
            }
            else
            {
                recipeImageA[i].gameObject.SetActive(false);
            }

            if (i < unitRecipe.recipeB.Length && unitRecipe.recipeB[i] != null)
            {
                recipeImageB[i].sprite = unitRecipe.recipeB[i].unitImage;
                recipeImageB[i].gameObject.SetActive(true);
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
        }
        else
        {
            resultImageA.gameObject.SetActive(false);
        }
        if(unitRecipe.nextUnitB != null)
        {
            resultImageB.gameObject.SetActive(true);
            resultImageB.sprite = unitRecipe.nextUnitB.GetComponentInChildren<Unit>().unitRecipe.unitImage;
        }
        else
        {
            resultImageB.gameObject.SetActive(false);
        }

        buttonA.SetActive(recipeImageA.Any(img => img.gameObject.activeSelf) || resultImageA.gameObject.activeSelf);
        buttonB.SetActive(recipeImageB.Any(img => img.gameObject.activeSelf) || resultImageB.gameObject.activeSelf);
    }

    public void CloseUi()
    {
        towerUi.SetActive(false);
        closeUi.SetActive(false);
    }

}
