using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitStroeSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnitRecipe unitRecipe;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI unitName;
    public GameObject buyImage;
    private Vector3 originalScales;
    [SerializeField] private int price;
    public bool isBuy = true;

    private void Awake()
    {
        originalScales = transform.localScale;
    }

    public void AddUnit(UnitRecipe _unitRecipe, int _price)
    {
        unitRecipe = _unitRecipe;
        unitImage.sprite = _unitRecipe.unitImage;
        unitName.text = _unitRecipe.unitName;
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
            if(GameManager.instance.Ruby >= price)
            {
                if(isBuy)
                {
                    GameManager.instance.Ruby -= price;
                    foreach (var u in StorageBoxManager.instance.slots)
                    {
                        if (u.unitRecipe == unitRecipe)
                        {
                            u.Exp++;
                            isBuy = false;
                            buyImage.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                StoreManager.instance.ShowLackText();
            }
        }
    }
}
