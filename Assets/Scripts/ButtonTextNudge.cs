using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextNudge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform textTransform;
    private Vector2 originalPosition;

    private void Start()
    {
        originalPosition = textTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 pos = textTransform.anchoredPosition;
        pos.y = pos.y * 0.25f; 
        textTransform.anchoredPosition = pos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        textTransform.anchoredPosition = originalPosition;
    }
}
