using UnityEngine;

[CreateAssetMenu(fileName = "New UnitRecipe", menuName = "New UnitRecipe/UnitRecipe")]
public class UnitRecipe : ScriptableObject
{
    public string unitName;
    public UnitRecipe[] recipeA;
    public UnitRecipe[] recipeB;
    public string recipeTextA;
    public string recipeTextB;
    public GameObject nextTowerA;
    public GameObject nextTowerB;
}
