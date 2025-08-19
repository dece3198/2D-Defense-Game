using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DungeonState
{
    Idle, Walk, Attack, Slash, DoubleSlash, Swoop, ConsecutiveSlash, Stormblade, Die
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
            unit.target = unit.viewDetector.Target.GetComponent<Rigidbody2D>();
            if (unit.isSkillDic.ContainsKey(SkillType.Swoop))
            {
                if (unit.isSkillDic[SkillType.Swoop])
                {
                    unit.ChangeState(DungeonState.Swoop);
                    return;
                }
            }

            unit.ChangeState(DungeonState.Walk);
        }
    }
}

public class WalkUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        unit.target = unit.viewDetector.Target.GetComponent<Rigidbody2D>();
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
        unit.SkillCheck(SkillType.Swoop);
        unit.SkillCheck(SkillType.Stormblade);
        if (Vector2.Distance(unit.transform.position, unit.target.transform.position) < 1f)
        {
            unit.SkillCheck(SkillType.ConsecutiveSlash);
            unit.SkillCheck(SkillType.DoubleSlash);
            unit.SkillCheck(SkillType.Slash);
            if (unit.isAtk)
            {
                unit.ChangeState(DungeonState.Attack);
            }
            unit.animator.SetBool("1_Move", false);
            return;
        }
        else
        {
            unit.animator.SetBool("1_Move", true);
            Vector2 dirVec = unit.target.position - unit.rigid.position;
            Vector2 nextVec = dirVec.normalized * unit.speed * Time.fixedDeltaTime;
            unit.rigid.MovePosition(unit.rigid.position + nextVec);
            unit.rigid.linearVelocity = Vector2.zero;
        }
    }
}


public class AttackUnit : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        unit.StartCoroutine(AttackCo(unit));
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }

    private IEnumerator AttackCo(DungeonUnit unit)
    {
        unit.isAtk = false;
        unit.animator.SetBool("1_Move", false);
        unit.sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        unit.viewDetector.FindAttackTarget(10);
        yield return new WaitForSeconds(0.5f);
        unit.ChangeState(DungeonState.Idle);
        float time = unit.atkCoolTime;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        unit.isAtk = true;

    }
}

public class Slash : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        if (unit.viewDetector.Target == null)
        {
            unit.ChangeState(DungeonState.Idle);
            return;
        }
        unit.StartCoroutine(SlashCo(unit));
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }

    private IEnumerator SlashCo(DungeonUnit unit)
    {
        if (unit.target != null)
        {
            float direction = unit.target.position.x < unit.rigid.position.x ? -1f : 1f;
            float zLocalRotation = direction > 0 ? 180f : 0f;
            unit.skillView[4].transform.localRotation = Quaternion.Euler(0, 0, zLocalRotation);
            float zCRotation = direction > 0 ? -75f : 120f;
            unit.skillView[4].animator.transform.localRotation = Quaternion.Euler(0, 0, zCRotation);
        }
        unit.isSkillDic[SkillType.Slash] = false;
        unit.skillView[4].animator.SetTrigger("Skill");
        unit.skillView[4].FindAngleTarget(6);
        unit.sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        unit.ChangeState(DungeonState.Idle);
        float time = 3f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            unit.SkillImageDic[SkillType.Slash].fillAmount = time / 3f;
            yield return null;
        }
        
        if(unit.isSkillDic.ContainsKey(SkillType.Slash))
        {
            unit.isSkillDic[SkillType.Slash] = true;
        }
    }
}

public class DoubleSlash : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        if (unit.viewDetector.Target == null)
        {
            unit.ChangeState(DungeonState.Idle);
            return;
        }
        unit.StartCoroutine(DoubleSlashCo(unit));
    }

    public override void Exit(DungeonUnit unit)
    {

    }

    public override void Update(DungeonUnit unit)
    {
    }

    private IEnumerator DoubleSlashCo(DungeonUnit unit)
    {
        if (unit.target != null)
        {
            float direction = unit.target.position.x < unit.rigid.position.x ? -1f : 1f;

            float zLocalRotation = direction > 0 ? 180f : 0f;
            unit.skillView[5].transform.localRotation = Quaternion.Euler(0, 0, zLocalRotation);
            float zRotation = direction > 0 ? 0f : 180f;
            unit.skillView[5].animator.transform.localRotation = Quaternion.Euler(0, 0, zRotation);
        }
        unit.isSkillDic[SkillType.DoubleSlash] = false;
        unit.skillView[5].animator.SetTrigger("Skill");
        unit.skillView[5].FindAngleTarget(5);
        unit.sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        yield return new WaitForSeconds(0.5f);
        unit.skillView[5].FindAngleTarget(5);
        unit.sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        yield return new WaitForSeconds(0.5f);
        unit.ChangeState(DungeonState.Idle);
        float time = 4f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            unit.SkillImageDic[SkillType.DoubleSlash].fillAmount = time / 4f;
            yield return null;
        }

        if(unit.isSkillDic.ContainsKey(SkillType.DoubleSlash))
        {
            unit.isSkillDic[SkillType.DoubleSlash] = true;
        }
    }
}

public class Swoop : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        if (unit.viewDetector.Target == null)
        {
            unit.ChangeState(DungeonState.Idle);
            return;
        }
        unit.StartCoroutine(SwoopCo(unit));
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }

    private IEnumerator SwoopCo(DungeonUnit unit)
    {
        unit.animator.SetBool("1_Move", false);
        unit.animator.SetTrigger("Swoop");
        var target = unit.viewDetector.Target;
        unit.isSkillDic[SkillType.Swoop] = false;
        yield return new WaitForSeconds(0.1f);
        if (target != null)
        {
            Vector3 toHeratDir = (GameManager.instance.heart.transform.position - target.transform.position).normalized;
            Vector3 behindDir = -toHeratDir;

            float distance = 1.5f;
            Vector3 behindPos = target.transform.position + behindDir * distance;
            unit.transform.DOMove(behindPos, 0.1f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.25f);
            unit.skillView[6].transform.position = target.transform.position;
            unit.skillView[6].animator.SetTrigger("Skill");
            target.GetComponent<IInteractable>().DungeonTakeHit(1);
            target.GetComponent<DungeonMonster>().stun = 1f;
        }
        else
        {
            unit.ChangeState(DungeonState.Walk);
        }

        unit.ChangeState(DungeonState.Idle);


        float time = 1f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            unit.SkillImageDic[SkillType.Swoop].fillAmount = time;
            yield return null;
        }

        if (unit.isSkillDic.ContainsKey(SkillType.Swoop))
        {
            unit.isSkillDic[SkillType.Swoop] = true;
        }
    }
}

public class ConsecutiveSlash : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        if(unit.viewDetector.Target == null)
        {
            unit.ChangeState(DungeonState.Idle);
            return;
        }
        unit.StartCoroutine(ConsecutiveSlashCo(unit));
       
    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }

    private IEnumerator ConsecutiveSlashCo(DungeonUnit unit)
    {
        unit.isSkillDic[SkillType.ConsecutiveSlash] = false;
        unit.animator.SetTrigger("SkillA");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 4; i++)
        {
            unit.skillView[i].animator.SetTrigger("Skill");
            unit.skillView[i].FindRangeTarget(3);
            unit.skillView[i].FindRangeTarget(3);
            yield return new WaitForSeconds(0.25f);
        }
        unit.ChangeState(DungeonState.Idle);

        float time = 8.5f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            unit.SkillImageDic[SkillType.ConsecutiveSlash].fillAmount = time / 8.5f;
            yield return null;
        }

        if(unit.isSkillDic.ContainsKey(SkillType.ConsecutiveSlash))
        {
            unit.isSkillDic[SkillType.ConsecutiveSlash] = true;
        }
    }
}

public class Stormblade : BaseState<DungeonUnit>
{
    public override void Enter(DungeonUnit unit)
    {
        if (unit.viewDetector.Target == null)
        {
            unit.ChangeState(DungeonState.Idle);
            return;
        }
        unit.StartCoroutine(StormbladeCo(unit));
        Debug.Log(unit.isSkillDic[SkillType.Stormblade]);

    }

    public override void Exit(DungeonUnit unit)
    {
    }

    public override void Update(DungeonUnit unit)
    {
    }

    private IEnumerator StormbladeCo(DungeonUnit unit)
    {
        unit.isSkillDic[SkillType.Stormblade] = false;
        unit.orbitManager.gameObject.SetActive(true);
        unit.orbitManager.RepositionOrbit();
        unit.ChangeState(DungeonState.Idle);
        yield return new WaitForSeconds(5f);
        unit.orbitManager.gameObject.SetActive(false);
        float time = 8.5f;

        while (time > 0)
        {
            unit.SkillImageDic[SkillType.Stormblade].fillAmount = time / 8.5f;
            time -= Time.deltaTime;
            yield return null;
        }

        if (unit.isSkillDic.ContainsKey(SkillType.Stormblade))
        {
            unit.isSkillDic[SkillType.Stormblade] = true;
        }
    }
}
public class DungeonUnit : MonoBehaviour
{
    public float atk;
    public float atkCoolTime;
    public float speed;
    public ViewDetector viewDetector;
    public Animator animator;
    public Rigidbody2D rigid;
    public Rigidbody2D target;
    public ViewDetector[] skillView;
    public SPUM_Prefabs sPUM_Prefabs;
    public OrbitManager orbitManager;
    private StateMachine<DungeonState, DungeonUnit> stateMachine = new StateMachine<DungeonState, DungeonUnit> ();
    private Dictionary<SkillType, DungeonState> skillDic = new Dictionary<SkillType, DungeonState>();
    public Dictionary<SkillType, bool> isSkillDic = new ();
    public Dictionary<SkillType, Image> SkillImageDic = new();
    public DungeonState unitState;
    public bool isAtk = true;


    private void Awake()
    {
        stateMachine.Reset(this);
        viewDetector = GetComponent<ViewDetector>();
        sPUM_Prefabs = transform.parent.GetComponent<SPUM_Prefabs>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sPUM_Prefabs.OverrideControllerInit();
        stateMachine.AddState(DungeonState.Idle, new IdleUnit());
        stateMachine.AddState(DungeonState.Walk, new WalkUnit());
        stateMachine.AddState(DungeonState.Attack, new AttackUnit());
        stateMachine.AddState(DungeonState.Slash, new Slash());
        stateMachine.AddState(DungeonState.DoubleSlash, new DoubleSlash());
        stateMachine.AddState(DungeonState.Swoop, new Swoop());
        stateMachine.AddState(DungeonState.ConsecutiveSlash, new ConsecutiveSlash());
        stateMachine.AddState(DungeonState.Stormblade, new Stormblade());
        ChangeState(DungeonState.Idle);

        skillDic.Add(SkillType.Slash, DungeonState.Slash);
        skillDic.Add(SkillType.DoubleSlash, DungeonState.DoubleSlash);
        skillDic.Add(SkillType.Swoop, DungeonState.Swoop);
        skillDic.Add(SkillType.ConsecutiveSlash, DungeonState.ConsecutiveSlash);
        skillDic.Add(SkillType.Stormblade, DungeonState.Stormblade);
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
        }
    }

    public void ChangeState(DungeonState state)
    {
        unitState = state;
        stateMachine.ChangeState(state);
    }

    public void SkillUse(SkillType skillType)
    {
        if (skillDic[skillType] == unitState || isSkillDic[skillType] == false) return;
        ChangeState(skillDic[skillType]);
    }

    public void SkillCheck(SkillType skillType)
    {
        if(isSkillDic.ContainsKey(skillType))
        {
            if(isSkillDic[skillType])
            {
                ChangeState(skillDic[skillType]);
            }
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
