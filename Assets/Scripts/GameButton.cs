using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScales;
    public GameObject buttonUi;
    public Image backImage;

    private void Awake()
    {
        originalScales = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales * 0.9f, 0.1f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad);
            if(transform.CompareTag("Button"))
            {
                WaitingRoom.instance.curButton.buttonUi.SetActive(false);
                WaitingRoom.instance.curButton.backImage.color = Color.black;
                WaitingRoom.instance.curButton = this;
                backImage.color = Color.white;
                buttonUi.SetActive(true);
            }
        }
    }
}
