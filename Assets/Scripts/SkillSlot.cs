using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScales;
    [SerializeField] private GameObject maskImage;
    public Image coolTimeImage;
    public Image skillImage;
    public Skill curSkill;
    private float time = 0;

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

    public void ClearSLot()
    {
        maskImage.SetActive(false);
        coolTimeImage.fillAmount = 0;
        curSkill = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad);
            if(SkillManager.instance.isSkill)
            {
                foreach(var skill in SkillManager.instance.slots)
                {
                    if(skill.curSkill == SkillManager.instance.curSkill)
                    {
                        SkillManager.instance.player.isSkillDic.Remove(SkillManager.instance.curSkill.skillType);
                        SkillManager.instance.player.SkillImageDic.Remove(SkillManager.instance.curSkill.skillType);
                        time = skill.coolTimeImage.fillAmount;
                        skill.ClearSLot();
                    }
                }
                
                curSkill = SkillManager.instance.curSkill;
                maskImage.gameObject.SetActive(true);
                skillImage.sprite = curSkill.skillImage;
                SkillManager.instance.mark.SetActive(false);
                SkillManager.instance.player.SkillImageDic[curSkill.skillType] = coolTimeImage;
                if (time <= 0)
                {
                    SkillManager.instance.player.isSkillDic[curSkill.skillType] = true;
                }
                else
                {
                    SkillManager.instance.player.isSkillDic[curSkill.skillType] = false;
                }
                SkillManager.instance.isSkill = false;
            }
            else
            {
                if (curSkill == null)
                {
                    WaitingRoom.instance.ChangeMenu(3);
                    UpGradeManager.instance.ChangeButton(1);
                }
                else
                {
                    SkillManager.instance.player.SkillUse(curSkill.skillType);
                }
            }
        }
    }
}
