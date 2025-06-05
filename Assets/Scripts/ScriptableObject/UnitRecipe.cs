using UnityEngine;
public enum UnitRating
{
    Normal, Rare, Epic, Unique, Legendary
}

[CreateAssetMenu(fileName = "New UnitRecipe", menuName = "New UnitRecipe/UnitRecipe")]
public class UnitRecipe : ScriptableObject
{
    public string unitName;
    public string unitCharacteristic;
    public Sprite unitImage;
    public float atkCoolTime;
    public int minAtk;
    public int maxAtk;
    public float stun;
    public float skillStun;
    public float skillPercent;
    public float skillDamage;
    public int debuff;
    public float buff;
    public float speedDebuff;
    public UnitRecipe[] recipeA;
    public UnitRecipe[] recipeB;
    public GameObject nextUnitA;
    public GameObject nextUnitB;
    public UnitType unitType;
    public UnitAtkType unitAtkType;
    public UnitRating unitRating;
}
