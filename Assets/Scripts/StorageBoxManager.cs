using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageBoxManager : Singleton<StorageBoxManager>
{
    [SerializeField] private GameObject unitState;
    [SerializeField] private Image unitImage;
    [SerializeField] private GameObject def;
    [SerializeField] private GameObject stun;
    [SerializeField] private GameObject speed;
    [SerializeField] private GameObject skill;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitLevel;
    [SerializeField] private TextMeshProUGUI explanation;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI stunText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Slider expSlider;
    public UnitSlot curUnitSlot;
    [SerializeField] private GameObject slotParent;
    public UnitSlot[] slots;
    public Dictionary<Rating, Color> unitColorDic = new Dictionary<Rating, Color>();

    private new void Awake()
    {
        base.Awake();
        unitColorDic.Add(Rating.Normal, Color.gray);
        unitColorDic.Add(Rating.Rare, Color.white);
        unitColorDic.Add(Rating.Epic, Color.yellow);
        unitColorDic.Add(Rating.Unique, new Color32(0, 75, 255,255));
        unitColorDic.Add(Rating.Legendary, new Color32(255, 75, 75,255));
    }

    private void Start()
    {
        slots = slotParent.GetComponentsInChildren<UnitSlot>();
    }

    public void AddSlot(UnitRecipe unitRecipe, float value)
    {
        unitState.SetActive(true);
        expSlider.value = value;
        unitImage.sprite = unitRecipe.unitImage;
        unitName.text = unitRecipe.unitName;
        explanation.text = unitRecipe.unitCharacteristic;
        unitLevel.text = "lv" + (unitRecipe.level + 1).ToString();
        float min = unitRecipe.minAtk * ((unitRecipe.level + 1) * 0.2f);
        float max = unitRecipe.maxAtk * ((unitRecipe.level + 1) * 0.2f);
        atkText.text = (unitRecipe.minAtk + min).ToString("N0") + " ~ " + (unitRecipe.maxAtk + max).ToString("N0");
        atkSpeedText.text = (1 / unitRecipe.atkCoolTime).ToString("N1");
        expText.text = curUnitSlot.Exp.ToString() + " / " + curUnitSlot.maxExp.ToString();
        priceText.text = ((((int)unitRecipe.unitRating + 1) * 500) + (unitRecipe.level * 100)).ToString();
        if (unitRecipe.debuff != 0)
        {
            def.SetActive(true);
            defText.text = unitRecipe.debuff.ToString();
        }
        else
        {
            def.SetActive(false);
        }

        if (unitRecipe.stun != 0)
        {
            stun.SetActive(true);
            stunText.text = unitRecipe.stun.ToString();
        }
        else
        {
            stun.SetActive(false);
        }

        if (unitRecipe.speedDebuff != 0)
        {
            speed.SetActive(true);
            speedText.text = (unitRecipe.speedDebuff * 100).ToString() + "%";
        }
        else
        {
            speed.SetActive(false);
        }

        if (unitRecipe.skillDamage != 0)
        {
            skill.SetActive(true);
            skillText.text = (unitRecipe.skillDamage * 100).ToString() + "%";
        }
        else
        {
            skill.SetActive(false);
        }
    }

    public void XButton()
    {
        unitState.SetActive(false);
    }

    public void UpGradeButton()
    {
        int price = (((int)curUnitSlot.unitRecipe.unitRating + 1) * 500) + (curUnitSlot.unitRecipe.level * 100);

        if (GameManager.instance.Dia >= price)
        {
            if (curUnitSlot.Exp >= curUnitSlot.maxExp)
            {
                curUnitSlot.Exp -= curUnitSlot.maxExp;
                curUnitSlot.unitRecipe.level++;
                AddSlot(curUnitSlot.unitRecipe, curUnitSlot.levelSlider.value);
            }
        }
    }
}
