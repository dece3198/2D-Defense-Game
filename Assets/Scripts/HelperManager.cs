using System.Linq;
using UnityEngine;

public class HelperManager : Singleton<HelperManager>
{
    [SerializeField] private HelperSlot[] helperSlots;
    [SerializeField] private GameObject helparUi;

    public void StartHelper(UnitRecipe[] units)
    {
        helparUi.SetActive(true);
        for (int i = 0; i < units.Length; i++)
        {
            helperSlots[i].unitRecipe = units[i];
            helperSlots[i].unitImage.sprite = units[i].unitImage;
            helperSlots[i].unitrating.color = UiManager.instance.unitDic[units[i].unitRating];
        }

        foreach(var slot in helperSlots)
        {
            if(slot.unitRecipe != null)
            {
                slot.gameObject.SetActive(true);
                slot.FindUnit();
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    public void UnitCheck()
    {
        if(helparUi.activeSelf)
        {
            foreach (var slot in helperSlots)
            {
                if (slot.unitRecipe != null)
                {
                    slot.FindUnit();
                }
            }
        }
    }

}
