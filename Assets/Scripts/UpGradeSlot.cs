using TMPro;
using UnityEngine;


public class UpGradeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI nextValueText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private int basePrice;
    [SerializeField] private int price;
    public int Price
    {
        get { return price; }
        set
        {
            price = value;
            if(level == maxLevel)
            {
                priceText.text = "Max";
            }
            else
            {
                priceText.text = price.ToString();
            }
        }
    }
    public int level;
    [SerializeField] private int increase;
    [SerializeField] private int priceIncrease;
    [SerializeField] private float percent;
    [SerializeField] private int maxLevel;
    [SerializeField] private UpGradeSlotType slotType;
    public GameObject lockImage;

    private void Start()
    {
        SlotSetting();
    }

    public void SlotSetting()
    {
        if(level == maxLevel)
        {
            string unit = UpGradeManager.instance.stringDic[slotType];
            valueText.text = "Max";
            nextValueText.text = "Max";
            Price = basePrice + (priceIncrease * level);
        }
        else
        {
            int value = UpGradeManager.instance.upGradeDic[slotType];
            string unit = UpGradeManager.instance.stringDic[slotType];
            valueText.text = (value + (increase * level)).ToString() + unit;
            nextValueText.text = (value + (increase * (level + 1))).ToString() + unit;
            Price = basePrice + (priceIncrease * level);
        }
    }

    private void LevelUp()
    {
        DungeonUnit tempUnit = UpGradeManager.instance.curUnit;
        //UpGradeManager.instance.curUnit = UpGradeManager.instance.curUnit.nextUnit;
        UpGradeManager.instance.curUnit.sPUM_Prefabs.gameObject.SetActive(true);
        tempUnit.sPUM_Prefabs.gameObject.SetActive(false);
    }

    private void MonsterLevelUp()
    {
        DungeonMonster tempUnit = UpGradeManager.instance.curMonster;
        UpGradeManager.instance.curMonster = UpGradeManager.instance.curMonster.nextMonster;
        UpGradeManager.instance.curMonster.sPUM_Prefabs.gameObject.SetActive(true);
        tempUnit.sPUM_Prefabs.gameObject.SetActive(false);
    }


    public void UpButton()
    {
        if(level < maxLevel)
        {
            if (GameManager.instance.Dia >= price)
            {
                GameManager.instance.Dia -= price;
                if (Random.value < percent)
                {
                    level++;
                    SlotSetting();
                    switch (slotType)
                    {
                        case UpGradeSlotType.Level: LevelUp(); break;
                        case UpGradeSlotType.MonsterLevel: MonsterLevelUp(); break;
                    }
                    UpGradeManager.instance.UnitSetting();
                }
            }
        }
    }
}
