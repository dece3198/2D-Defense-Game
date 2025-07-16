using UnityEngine;

public enum Rating
{
    Normal, Rare, Epic, Unique, Legendary, Superior, Myth
}

public enum UnitSkillType
{
    PD, MD
}

public enum UnitRangeType
{
    Range, Single
}

[CreateAssetMenu(fileName = "New UnitRecipe", menuName = "New UnitRecipe/UnitRecipe")]
public class UnitRecipe : ScriptableObject
{
    public GameObject unitObj;
    public int level;
    public string unitName;
    public string unitCharacteristic;
    public string explanation;
    public Sprite unitImage;
    public float atkCoolTime;
    public int minAtk;
    public int maxAtk;
    public float stun;
    public float skillStun;
    public float skillPercent;
    public float skillDamage;
    public float singleSKillPercent;
    public float singleSKillDamage;
    public int debuff;
    public float buff;
    public float speedDebuff;
    public UnitRecipe[] recipes;
    public UnitRecipe nextUnitA;
    public UnitRecipe nextUnitB;
    public UnitType unitType;
    public UnitAtkType unitAtkType;
    public UnitSkillType unitSkillType;
    public Rating unitRating;
}
