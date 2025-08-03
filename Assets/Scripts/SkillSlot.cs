using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScales;
    public Skill curSkill;
    

    private void Awake()
    {
        originalScales = transform.localScale;
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
            if(curSkill == null)
            {
                WaitingRoom.instance.ChangeMenu(2);
                UpGradeManager.instance.ChangeButton(1);
            }
        }
    }
}
