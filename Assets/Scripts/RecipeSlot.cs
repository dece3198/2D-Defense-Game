using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public enum RecipeSlotType
{
    A, B
}


public class RecipeSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public UnitRecipe unitRecipe;
    private Vector3 originalScales;
    [SerializeField] private bool isDragging = true;
    public RecipeSlotType slotType;
    public UnitRecipe[] units;

    private void Start()
    {
        originalScales = transform.localScale;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
            transform.DOScale(originalScales * 0.9f, 0.1f).SetEase(Ease.OutQuad).SetUpdate(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad).SetUpdate(true);
            if (!isDragging)
            {
                if(slotType == RecipeSlotType.A)
                {
                    RecioeManager.instance.AddSlot(unitRecipe.nextUnitA, this);
                }
                else
                {
                    RecioeManager.instance.AddSlot(unitRecipe.nextUnitB, this);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
    }
}
