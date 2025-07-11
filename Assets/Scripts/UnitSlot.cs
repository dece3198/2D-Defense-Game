using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UnitSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public UnitRecipe unitRecipe;
    public Slider levelSlider;
    [SerializeField] private Image unitImage;
    [SerializeField] private Image backImage;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private float exp;
    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;
            maxExp = 3 + (unitRecipe.level);
            levelSlider.value = Mathf.Clamp01(exp / maxExp);
            expText.text = Exp.ToString() + " / " + maxExp.ToString();
        }
    }
    public float maxExp;
    [SerializeField] private TextMeshProUGUI expText;
    private Vector3 originalScales;
    private bool isDragging = true;

    private void Awake()
    {
        originalScales = transform.localScale;
        maxExp = 3 + (unitRecipe.level);
        expText.text = Exp.ToString() + " / " + maxExp.ToString();
        backImage = GetComponent<Image>();
    }

    private void Start()
    {
        unitImage.sprite = unitRecipe.unitImage;
        unitName.text = unitRecipe.unitName;
        unitName.color = UiManager.instance.unitDic[unitRecipe.unitRating];
        backImage.color = StorageBoxManager.instance.unitColorDic[unitRecipe.unitRating];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
            transform.DOScale(originalScales * 0.9f, 0.1f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad);
            if(!isDragging)
            {
                StorageBoxManager.instance.curUnitSlot = this;
                StorageBoxManager.instance.AddSlot(unitRecipe, levelSlider.value);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
    }
}
