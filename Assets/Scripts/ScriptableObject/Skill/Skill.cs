using UnityEngine;

public enum SkillType
{
    Slash, DoubleSlash, Swoop, ConsecutiveSlash, Stormblade
}

[CreateAssetMenu(fileName = "New Skill", menuName = "New Skill/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public Sprite skillImage;
    public float skillDamage;
    public SkillType skillType;
}
