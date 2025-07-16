using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelperManager : Singleton<HelperManager>
{
    [SerializeField] private HelperSlot[] helperSlots;
    public GameObject helparUi;
    

    public void StartHelper(UnitRecipe[] units)
    {
        helparUi.SetActive(true);

        Dictionary<UnitRecipe, int> countDic = new();

        foreach(var r in units)
        {
            if (!countDic.ContainsKey(r))
                countDic[r] = 1;
            else
                countDic[r]++;
        }

        int slotIndex = 0;

        foreach(var pair in countDic)
        {
            if (slotIndex >= helperSlots.Length) break;
            helperSlots[slotIndex].SetRecipe(pair.Key, pair.Value);
            slotIndex++;
        }

        for(int i = slotIndex; i < helperSlots.Length; i++)
        {
            helperSlots[i].SetRecipe(null, 0);
        }


        /*
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
        */
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
