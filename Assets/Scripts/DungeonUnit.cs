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
        unit.animator.SetBool("1_Move", false);
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
        unit.viewDetector.FindTarget();
        if (unit.viewDetector.Target != null)
        {
            unit.ChangeState(DungenoState.Walk);
        }
    }
}

public class WalkUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        unit.animator.SetBool("1_Move", true);
        unit.target = unit.viewDetector.Target.GetComponent<Rigidbody2D>();
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
        Vector2 dirVec = unit.target.position - unit.rigid.position;
        Vector2 nextVec = dirVec.normalized * unit.speed * Time.fixedDeltaTime;
        unit.rigid.MovePosition(unit.rigid.position + nextVec);
        unit.rigid.linearVelocity = Vector2.zero;

        if(Vector2.Distance(unit.transform.position, unit.target.transform.position) < 1f)
        {
            unit.ChangeState(DungenoState.Attack);
        }
    }
}


public class AttackUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        unit.animator.SetBool("1_Move", false);
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
    public float speed;
    public ViewDetector viewDetector;
    public Animator animator;
    public Rigidbody2D rigid;
    [SerializeField] private ViewDetector[] skillView;
    public SPUM_Prefabs sPUM_Prefabs;
    [SerializeField] private OrbitManager orbitManager;
    private StateMachine<DungenoState, DungeonUnit> stateMachine = new StateMachine<DungenoState, DungeonUnit> ();
    public DungenoState unitState;
    public bool isAtk = true;
    public Rigidbody2D target;
    [SerializeField] private Transform slashP;
    [SerializeField] private Transform slashC;


    private void Awake()
    {
        stateMachine.Reset(this);
        viewDetector = GetComponent<ViewDetector>();
        sPUM_Prefabs = transform.parent.GetComponent<SPUM_Prefabs>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
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
        if(Input.GetKeyDown(KeyCode.K))
        {
            /*
            skillView[4].animator.SetTrigger("Skill");
            skillView[4].FindAngleTarget(10);
            sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
            */
            StartCoroutine(SkillA());
        }
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            float direction = target.position.x < rigid.position.x ? -1f : 1f;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;

            float zPRotation = direction > 0 ? 180f : 0f;
            slashP.localRotation = Quaternion.Euler(0, 0, zPRotation);
            float zCRotation = direction > 0 ? -75f : 120f;
            slashC.localRotation = Quaternion.Euler(0, 0, zCRotation);
        }
    }

    public void ChangeState(DungenoState state)
    {
        unitState = state;
        stateMachine.ChangeState(state);
    }

    private IEnumerator SkillA()
    {
        animator.SetTrigger("SkillA");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 4; i++)
        {
            skillView[i].animator.SetTrigger("Skill");
            skillView[i].FindRangeTarget(11);
            skillView[i].FindRangeTarget(11);
            yield return new WaitForSeconds(0.25f);
        }
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
