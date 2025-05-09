using UnityEngine;
public enum UnitRating
{
    Normal, Rare, Epic, Unique, Legendary
}

[CreateAssetMenu(fileName = "New UnitRecipe", menuName = "New UnitRecipe/UnitRecipe")]
public class UnitRecipe : ScriptableObject
{
    public string unitName;
    public Sprite unitImage;
    public int minAtk;
    public int maxAtk;
    public int debuff;
    public float speedDebuff;
    public UnitRecipe[] recipeA;
    public UnitRecipe[] recipeB;
    public GameObject nextUnitA;
    public GameObject nextUnitB;
    public UnitType unitType;
    public UnitRating unitRating;
}
