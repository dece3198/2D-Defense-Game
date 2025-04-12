using TMPro;
using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private GameObject towerUi;
    [SerializeField] private TextMeshProUGUI recipeA;
    [SerializeField] private TextMeshProUGUI recipeB;

    public void AddTower(UnitRecipe unitRecipe)
    {
        towerUi.SetActive(true);
        recipeA.text = unitRecipe.recipeTextA;
        recipeB.text = unitRecipe.recipeTextB;
    }

}
