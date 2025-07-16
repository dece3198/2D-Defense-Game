using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class RecioeManager : Singleton<RecioeManager>
{
    [SerializeField] private GameObject unitState;
    [SerializeField] private Image unitImage;
    [SerializeField] private GameObject def;
    [SerializeField] private GameObject stun;
    [SerializeField] private GameObject rangeStun;
    [SerializeField] private GameObject speed;
    [SerializeField] private GameObject skill;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI explanation;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI stunText;
    [SerializeField] private TextMeshProUGUI rangeStunText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI skillText;
    public RecipeSlot curSlot;


    public void AddSlot(UnitRecipe unitRecipe, RecipeSlot slot)
    {
        curSlot = slot;
        unitState.SetActive(true);
        unitImage.sprite = unitRecipe.unitImage;
        unitName.text = unitRecipe.unitName;
        explanation.text = unitRecipe.unitCharacteristic;
        float min = unitRecipe.minAtk * ((unitRecipe.level + 1) * 0.2f);
        float max = unitRecipe.maxAtk * ((unitRecipe.level + 1) * 0.2f);
        atkText.text = (unitRecipe.minAtk + min).ToString("N0") + " ~ " + (unitRecipe.maxAtk + max).ToString("N0");
        atkSpeedText.text = (1 / unitRecipe.atkCoolTime).ToString("N1");
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

        if(unitRecipe.skillStun != 0)
        {
            rangeStun.SetActive(true);
            rangeStunText.text = unitRecipe.skillStun.ToString();
        }
        else
        {
            rangeStun.SetActive(false);
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

    public void Combine()
    {
        if(GameManager.instance.mainUi.activeInHierarchy)
        {
            foreach (var unit in UnitSpawner.instance.unitList)
            {
                if (unit.GetComponentInChildren<Unit>().unitRecipe == curSlot.unitRecipe)
                {
                    UnitSpawner.instance.curUnit = unit;
                    if (curSlot.slotType == RecipeSlotType.A)
                    {
                        UnitSpawner.instance.CombineA();
                    }
                    else
                    {
                        UnitSpawner.instance.CombineB();
                    }
                    return;
                }
            }
        }
    }

    public void HelperButton()
    {
        if(GameManager.instance.mainUi.activeInHierarchy)
        {
            HelperManager.instance.StartHelper(curSlot.units);
        }
    }

    public void XButton()
    {
        unitState.SetActive(false);
        UnitSpawner.instance.curUnit = null;
    }
}
