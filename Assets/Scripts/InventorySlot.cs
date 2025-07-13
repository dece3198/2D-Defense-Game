using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Item item;
    [SerializeField] private Image itemImage;
    [SerializeField] private Outline outline;
    public float atk;
    public float skillP;
    public float skillD;
    private Vector3 originalScales;

    private void Awake()
    {
        originalScales = transform.localScale;
        outline = GetComponent<Outline>();
    }

    public void AddItem(Item _item)
    {
        item = _item;
        itemImage.sprite = _item.itemImage;
        atk = _item.atk;
        skillP = _item.skillP;
        skillD = _item.skillD;
        outline.effectColor = UiManager.instance.unitDic[_item.itemRating];
        SetColor(1);
    }

    public void ClearSlot()
    {
        item = null;
        SetColor(0);
    }

    private void SetColor(float alpha)
    {
        Color color = itemImage.color;
        color.a = alpha;
        itemImage.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales * 0.9f, 0.1f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad);
            if(item != null)
            {
                InventoryManager.instance.ShowItem(this);
            }
        }
    }
}
