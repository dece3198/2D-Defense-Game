using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private UpGradeSlot monsterLevel;
    public Dictionary<UpGradeSlotType, int> upGradeDic = new Dictionary<UpGradeSlotType, int>();
    public Dictionary<UpGradeSlotType, string> stringDic = new Dictionary<UpGradeSlotType, string>();
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI skillPText;
    [SerializeField] private TextMeshProUGUI skillDText;
    [SerializeField] private Image unitImage;

    private new void Awake()
    {
        base.Awake();
        upGradeDic.Add(UpGradeSlotType.Level, 1);
        upGradeDic.Add(UpGradeSlotType.Atk, 100);
        upGradeDic.Add(UpGradeSlotType.AtkSpeed, 0);
        upGradeDic.Add(UpGradeSlotType.SkillPercent, 1);
        upGradeDic.Add(UpGradeSlotType.SkillDamage, 0);
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
        switch (levelUp.level)
        {
            case 1: 
                skillDamage.lockImage.SetActive(false);
                skillPercent.lockImage.SetActive(false); break;
            case 2 : monsterLevel.lockImage.SetActive(false); break;
        }
        unitImage.sprite = curUnit.unitRecipe.unitImage;
        float minAtk = curUnit.unitRecipe.minAtk + (curUnit.unitRecipe.minAtk * InventoryManager.instance.itemAtk * 0.01f);
        float maxAtk = curUnit.unitRecipe.maxAtk + (curUnit.unitRecipe.maxAtk * InventoryManager.instance.itemAtk * 0.01f);
        float unitMinAtk = minAtk + (minAtk * (atkUp.level * 0.05f));
        float unitMaxAtk = maxAtk + (maxAtk * (atkUp.level * 0.05f));
        levelText.text = (levelUp.level + 1).ToString() + "lv";
        atkText.text = unitMinAtk.ToString("N1") + " ~ " + unitMaxAtk.ToString("N1");
        atkSpeedText.text = (1 / (curUnit.unitRecipe.atkCoolTime - (curUnit.unitRecipe.atkCoolTime * (atkSpeedUp.level * 0.005f)))).ToString("N3");
        float skillP = curUnit.unitRecipe.skillPercent + (curUnit.unitRecipe.skillPercent * InventoryManager.instance.itemSkillP * 0.01f);
        skillPText.text = ((skillP + (skillP * (skillPercent.level * 0.01f))) * 100).ToString() + "%";
        float skillD = curUnit.unitRecipe.skillDamage + (curUnit.unitRecipe.skillDamage * InventoryManager.instance.itemSKillD * 0.01f);
        skillDText.text = ((skillD + (skillD * (skillDamage.level * 0.05f))) * 100).ToString() + "%";
    }
}
