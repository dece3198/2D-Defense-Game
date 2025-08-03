using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DungenoState
{
    Idle, Walk, Attack, SkillA, SKillB, SkilLC, Die
}

public class IdleUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }
}

public class WalkUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }
}


public class AttackUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }
}

public class SkillA : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }
}

public class SkillB : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }
}

public class SkillC : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }
}

public class DungeonUnit : MonoBehaviour
{
    public UnitRecipe unitRecipe;
    [SerializeField] private float minAtk;
    [SerializeField] private float maxAtk;
    [SerializeField] private ViewDetector viewDetector;
    [SerializeField] private Animator skillAni;
    public SPUM_Prefabs sPUM_Prefabs;
    [SerializeField] private OrbitManager orbitManager;
    private StateMachine<DungenoState, DungeonUnit> stateMachine = new StateMachine<DungenoState, DungeonUnit> ();
    public DungenoState unitState;
    public bool isAtk = true;

    private void Awake()
    {
        stateMachine.Reset(this);
        viewDetector = GetComponent<ViewDetector>();
        sPUM_Prefabs = transform.parent.GetComponent<SPUM_Prefabs>();
        sPUM_Prefabs.OverrideControllerInit();
        stateMachine.AddState(DungenoState.Idle, new IdleUnit());
        stateMachine.AddState(DungenoState.Walk, new WalkUnit());
        stateMachine.AddState(DungenoState.Attack, new AttackUnit());
        stateMachine.AddState(DungenoState.SkillA, new SkillA());
        stateMachine.AddState(DungenoState.SKillB, new SkillB());
        stateMachine.AddState(DungenoState.SkilLC, new SkillC());
        ChangeState(DungenoState.Idle);
    }


    private void Update()
    {
        viewDetector.FindTarget();
        if(viewDetector.Target != null)
        {
            if(isAtk)
            {
                StartCoroutine(AtkCo());
            }
        }
    }

    public void ChangeState(DungenoState state)
    {
        unitState = state;
        stateMachine.ChangeState(state);
    }

    private IEnumerator AtkCo()
    {
        isAtk = false;
        /*
        sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 1);
        float unitMinAtk = unitRecipe.minAtk + (unitRecipe.minAtk * InventoryManager.instance.itemAtk * 0.01f);
        float unitMaxAtk = unitRecipe.maxAtk + (unitRecipe.maxAtk * InventoryManager.instance.itemAtk * 0.01f);
        minAtk = unitMinAtk + (unitMinAtk * (UpGradeManager.instance.atkUp.level *  0.05f));
        maxAtk = unitMaxAtk + (unitMaxAtk * (UpGradeManager.instance.atkUp.level * 0.05f));
        float randDamage = Random.Range(minAtk, maxAtk);
        viewDetector.Target.GetComponentInChildren<IInteractable>().TakeHit(randDamage, unitRecipe, 0);
        float atkSpeed = unitRecipe.atkCoolTime - (unitRecipe.atkCoolTime * (UpGradeManager.instance.atkSpeedUp.level * 0.005f));
        float skillP = unitRecipe.skillPercent + (unitRecipe.skillPercent * InventoryManager.instance.itemSkillP * 0.01f);
        float skillPercent = skillP + (skillP * (UpGradeManager.instance.skillPercent.level * 0.01f));
        if(skillAni != null && Random.value < skillPercent)
        {
            skillAni.SetTrigger("Skill");
            skillAni.gameObject.GetComponent<ViewDetector>().FindTarget();
            float skillD = unitRecipe.skillDamage + (unitRecipe.skillDamage * InventoryManager.instance.itemSKillD * 0.1f);
            float skillDamage = skillD + (skillD * (UpGradeManager.instance.skillDamage.level * 0.05f));
            float finalDamage = ((minAtk + maxAtk) * 0.5f) * skillDamage;
            if (skillAni.gameObject.GetComponent<ViewDetector>().Target)
            {
                skillAni.gameObject.GetComponent<ViewDetector>().Target.GetComponent<IInteractable>().TakeHit(finalDamage, unitRecipe, 0);
            }
        }
        */
        yield return new WaitForSeconds(1);
        isAtk = true;
    }
}
