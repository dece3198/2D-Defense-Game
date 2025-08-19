using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    public SkillSlot[] slots;
    [SerializeField] private Skill[] skills;
    public bool isSkill = false;
    public Skill curSkill;
    public GameObject mark;
    public DungeonUnit player;

    public void EquippedButton(int number)
    {
        isSkill = true; 
        curSkill = skills[number];
        WaitingRoom.instance.ChangeMenu(2);

        if(!UpGradeManager.instance.isChange)
        {
            UpGradeManager.instance.ChangeSlot();
        }
        mark.SetActive(true);
    }
}
