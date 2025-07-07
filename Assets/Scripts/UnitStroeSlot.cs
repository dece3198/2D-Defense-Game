using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitStroeSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private GameObject buyImage;
    private Vector3 originalScales;
    [SerializeField] private int price;

    private void Awake()
    {
        originalScales = transform.localScale;
    }

    public void AddUnit(UnitRecipe unitRecipe, int _price)
    {
        unitImage.sprite = unitRecipe.unitImage;
        unitName.text = unitRecipe.unitName;
        price = _price;
        priceText.text = price.ToString();
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
        }
    }
}
