using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private InventorySlot showSlot;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI skillPText;
    [SerializeField] private TextMeshProUGUI skillDText;
    [SerializeField] private TextMeshProUGUI equippedText;
    [SerializeField] private GameObject slotParent;
    public InventorySlot[] slots;
    public InventorySlot curSlot;
    [SerializeField] private InventorySlot weaponSlot;
    [SerializeField] private InventorySlot armorSlot;
    [SerializeField] private InventorySlot necklaceSlot;
    [SerializeField] private InventorySlot ringSlot;
    private Dictionary<ItemType, InventorySlot> slotDic = new Dictionary<ItemType, InventorySlot>();
    [SerializeField] private Item item;
    [SerializeField] private Item itemB;

    public float itemAtk;
    public float itemSkillP;
    public float itemSKillD;

    private new void Awake()
    {
        base.Awake();
        slotDic.Add(ItemType.Weapon, weaponSlot);
        slotDic.Add(ItemType.Armor, armorSlot);
        slotDic.Add(ItemType.Necklace, necklaceSlot);
        slotDic.Add(ItemType.Ring, ringSlot);
    }

    private void Start()
    {
        slots = slotParent.GetComponentsInChildren<InventorySlot>();

        foreach(var slot in slots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            AcquireItem(item);
            AcquireItem(itemB);
        }
    }

    public void AcquireItem(Item _item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].AddItem(_item);
                return;
            }
        }
    }

    public void ShowItem(InventorySlot slot)
    {
        curSlot = slot;
        showSlot.AddItem(slot.item);

        if(slot.atk != 0)
        {
            atkText.gameObject.SetActive(true);
            atkText.text = "공격력 + " + slot.atk.ToString() + "%";
        }
        else
        {
            atkText.gameObject.SetActive(false);
        }

        if (slot.skillP != 0)
        {
            skillPText.gameObject.SetActive(true);
            skillPText.text = "스킬확률 + " + slot.skillP.ToString() + "%";
        }
        else
        {
            skillPText.gameObject.SetActive(false);
        }

        if (slot.skillD != 0)
        {
            skillDText.gameObject.SetActive(true);
            skillDText.text = "스킬데미지 + " + slot.skillD.ToString() + "%";
        }
        else
        {
            skillDText.gameObject.SetActive(false);
        }

        EquippedTextSetting();
    }

    private void EquippedTextSetting()
    {
        if(curSlot != slotDic[curSlot.slotType])
        {
            if (slotDic[curSlot.slotType].item == null)
            {
                equippedText.text = "장착";
            }
            else
            {
                equippedText.text = "교체";
            }
        }
        else
        {
            equippedText.text = "해제";
        }
    }

    public void EquippedItem()
    {
        if (curSlot != null)
        {
            if(curSlot != slotDic[curSlot.slotType])
            {
                if (slotDic[curSlot.slotType].item == null)
                {
                    //빈슬롯에 아이템 장착
                    slotDic[curSlot.slotType].AddItem(showSlot.item);
                    itemAtk += slotDic[curSlot.slotType].atk;
                    itemSkillP += slotDic[curSlot.slotType].skillP;
                    itemSKillD += slotDic[curSlot.slotType].skillD;
                    showSlot.ClearSlot();
                    curSlot.ClearSlot();
                }
                else
                {
                    //아이템 교체
                    itemAtk -= slotDic[curSlot.slotType].atk;
                    itemSkillP -= slotDic[curSlot.slotType].skillP;
                    itemSKillD -= slotDic[curSlot.slotType].skillD;
                    AcquireItem(slotDic[curSlot.slotType].item);
                    slotDic[curSlot.slotType].AddItem(showSlot.item);
                    itemAtk += slotDic[curSlot.slotType].atk;
                    itemSkillP += slotDic[curSlot.slotType].skillP;
                    itemSKillD += slotDic[curSlot.slotType].skillD;
                    showSlot.ClearSlot();
                    curSlot.ClearSlot();
                }
            }
            else
            {
                //아이템 장착 해제
                itemAtk -= slotDic[curSlot.slotType].atk;
                itemSkillP -= slotDic[curSlot.slotType].skillP;
                itemSKillD -= slotDic[curSlot.slotType].skillD;
                AcquireItem(curSlot.item);
                curSlot.ClearSlot();
                curSlot.itemImage.sprite = curSlot.itemTypeImage;
                curSlot.SetColor(1);
                showSlot.ClearSlot();
            }

            atkText.gameObject.SetActive(false);
            skillPText.gameObject.SetActive(false);
            skillDText.gameObject.SetActive(false);
            curSlot = null;
        }
        UpGradeManager.instance.UnitSetting();
    }
}
