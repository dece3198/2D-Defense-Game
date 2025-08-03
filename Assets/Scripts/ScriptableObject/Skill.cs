using UnityEngine;

public enum SkillType
{
    Slash, Swoop, 
}

[CreateAssetMenu(fileName = "New Skill", menuName = "New Skill/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public Sprite skillImage;
    public float skillDamage;
}
