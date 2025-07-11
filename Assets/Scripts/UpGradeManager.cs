using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UpGradeSlotType
{
    Level, Atk, AtkSpeed, SkillPercent, SkillDamage, MonsterLevel
}

public class UpGradeManager : Singleton<UpGradeManager>
{
    public IdleUnit curUnit;
    public IdleMonster curMonster;
    public UpGradeSlot levelUp;
    public UpGradeSlot atkUp;
    public UpGradeSlot atkSpeedUp;
    public UpGradeSlot skillPercent;
    public UpGradeSlot skillDamage;
    public Dictionary<UpGradeSlotType, int> upGradeDic = new Dictionary<UpGradeSlotType, int>();
    public Dictionary<UpGradeSlotType, string> stringDic = new Dictionary<UpGradeSlotType, string>();
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI skillPText;
    [SerializeField] private TextMeshProUGUI skillDText;

    private new void Awake()
    {
        base.Awake();
        upGradeDic.Add(UpGradeSlotType.Level, 1);
        upGradeDic.Add(UpGradeSlotType.Atk, 100);
        upGradeDic.Add(UpGradeSlotType.AtkSpeed, 0);
        upGradeDic.Add(UpGradeSlotType.SkillPercent, 1);
        upGradeDic.Add(UpGradeSlotType.SkillDamage, 200);
        upGradeDic.Add(UpGradeSlotType.MonsterLevel, 1);
        stringDic.Add(UpGradeSlotType.Level, "lv");
        stringDic.Add(UpGradeSlotType.Atk, "%");
        stringDic.Add(UpGradeSlotType.AtkSpeed, "%");
        stringDic.Add(UpGradeSlotType.SkillPercent, "%");
        stringDic.Add(UpGradeSlotType.SkillDamage, "%");
        stringDic.Add(UpGradeSlotType.MonsterLevel, "lv");
    }

    private void Start()
    {
        UnitSetting();
    }

    public void UnitSetting()
    {
        float damage = ((curUnit.unitRecipe.minAtk + (curUnit.unitRecipe.minAtk * (atkUp.level * 0.05f))) + (curUnit.unitRecipe.maxAtk + (curUnit.unitRecipe.maxAtk * (atkUp.level * 0.05f)))) * 0.5f;
        levelText.text = (levelUp.level + 1).ToString() + "lv";
        atkText.text = damage.ToString("N1");
        atkSpeedText.text = (1 / (curUnit.unitRecipe.atkCoolTime - (curUnit.unitRecipe.atkCoolTime * (atkSpeedUp.level * 0.005f)))).ToString("N3");
        skillPText.text = (curUnit.unitRecipe.skillPercent + (curUnit.unitRecipe.skillPercent * (skillPercent.level * 0.01f))).ToString() + "%";
        skillDText.text = ((curUnit.unitRecipe.skillDamage + (curUnit.unitRecipe.skillDamage + (skillDamage.level * 0.05f))) * 100).ToString() + "%";
    }
}
