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
