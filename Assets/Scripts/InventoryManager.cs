using TMPro;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private InventorySlot showSlot;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI skillPText;
    [SerializeField] private TextMeshProUGUI skillDText;
    [SerializeField] private GameObject slotParent;
    public InventorySlot[] slots;
    public InventorySlot curSlot;

    private void Start()
    {
        slots = slotParent.GetComponentsInChildren<InventorySlot>();
    }

    public void AcquireItem(Item _item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
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
    }
}
