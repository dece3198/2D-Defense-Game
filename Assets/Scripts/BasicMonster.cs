using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MonsterIdle : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        monster.animator.SetBool("1_Move", false);
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {

    }
}

public class MonsterWalk : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        monster.animator.SetBool("1_Move", true);
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {
        if(GameManager.instance.targetPos.Length != 0)
        {
            Transform target = GameManager.instance.targetPos[monster.posIndex];
            switch(monster.posIndex)
            {
                case 0:
                case 1:
                    monster.transform.localScale = new Vector3(-1f, 1f, 1);
                    monster.textManager.transform.localScale = new Vector3(1f, 1f, 1);
                    break;
                case 2:
                case 3:
                    monster.transform.localScale = new Vector3(1f, 1f, 1);
                    monster.textManager.transform.localScale = new Vector3(-1f, 1f, 1);
                    break;

            }

            monster.transform.position = Vector2.MoveTowards(monster.transform.position, target.position, monster.GetCurSpeed() * Time.deltaTime);

            if(Vector2.Distance(monster.transform.position, target.position) < 0.1f)
            {
                monster.posIndex++;
                if(monster.posIndex >= GameManager.instance.targetPos.Length)
                {
                    monster.posIndex = 0;
                }
            }
        }
    }
}

public class MonsterHit : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        monster.animator.SetTrigger("3_Damaged");
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {

    }
}

public class MonsterStun : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        monster.animator.SetTrigger("5_Debuff");
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {

    }
}

public class MonsterDie : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        monster.gameObject.layer = 0;
        monster.animator.SetTrigger("4_Death");
        switch (monster.posIndex)
        {
            case 0:
            case 1:
                monster.textManager.transform.localScale = new Vector3(1f, 1f, 1);
                break;
            case 2:
            case 3:
                monster.textManager.transform.localScale = new Vector3(-1f, 1f, 1);
                break;

        }
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {
        if(monster.animator.GetCurrentAnimatorStateInfo(0).IsName("IDLE"))
        {
            monster.gameObject.layer = 7;
            switch (monster.posIndex)
            {
                case 0:
                case 1:
                    monster.textManager.transform.localScale = new Vector3(-1f, 1f, 1);
                    break;
                case 2:
                case 3:
                    monster.textManager.transform.localScale = new Vector3(1f, 1f, 1);
                    break;

            }
            monster.posIndex = 0;
            MonsterSpawner.instance.EnterPool(monster.gameObject, GameManager.instance.stage);
        }
    }
}

public class BasicMonster : Monster, IInteractable
{
    
    [SerializeField] private float hp;
    public float Hp 
    { 
        get { return hp; } 
        set 
        { 
            hp = value;
            if(hp <= 0)
            {
                if(monsterState != MonsterState.Die)
                {
                    ChangeState(MonsterState.Die);
                }
            }
            hpBar.value = Hp / maxHp;
        }
    }
    public MonsterState monsterState;
    public Dictionary<MonsterType, float> speedDic = new Dictionary<MonsterType, float>();
    StateMachine<MonsterState, BasicMonster> stateMachine = new StateMachine<MonsterState, BasicMonster>();
    public TextManager textManager;
    public Slider hpBar;
    public int posIndex = 0;
    private HashSet<UnitRecipe> units = new HashSet<UnitRecipe>();


    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine.Reset(this);
        stateMachine.AddState(MonsterState.Idle, new MonsterIdle());
        stateMachine.AddState(MonsterState.Walk, new MonsterWalk());
        stateMachine.AddState(MonsterState.Hit, new MonsterHit());
        stateMachine.AddState(MonsterState.Stun, new MonsterStun());
        stateMachine.AddState(MonsterState.Die, new MonsterDie());
        speedDic.Add(MonsterType.Normal, 1f);
        speedDic.Add(MonsterType.Fast, 1.5f);
        speedDic.Add(MonsterType.VeryFast, 2f);
        speedDic.Add(MonsterType.Boss, 0.5f);
    }

    private void OnEnable()
    {
        if((GameManager.instance.stage + 1) % 10 == 0 && GameManager.instance.stage != 0)
        {
            Hp = 200 * Mathf.Pow(1.127745f, GameManager.instance.stage) * 3;
        }
        else
        {
            Hp = 200 * Mathf.Pow(1.127745f, GameManager.instance.stage);
        }
        maxHp = Hp;
        def = 10 + (2.5f * (GameManager.instance.stage + 1));
        ChangeState(MonsterState.Walk);
        speed = speedDic[monsterType];
    }

    private void Update()
    {
        stateMachine.Update();

        if(Hp < maxHp)
        {
            Hp += maxHp * 0.002f * Time.deltaTime;
            if(Hp > maxHp)
            {
                Hp = maxHp;
            }
        }
    }

    public void SetSpeed(float debuff, UnitRecipe unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
            RecalculateSpeed();
        }
    }

    public float GetCurSpeed()
    {
        return speed;
    }

    public void ResetSpeed(UnitRecipe unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);

            
            if(units.Count > 0)
            {
                RecalculateSpeed();
            }
            else
            {
                speed = speedDic[monsterType];
            }
        }
    }

    public void RecalculateSpeed()
    {
        float tempDeBuff = 1f;

        foreach (var u in units)
        {
            tempDeBuff *= (1f - u.speedDebuff);
        }
        speed = Mathf.Clamp(speedDic[monsterType] * tempDeBuff, 0.1f, 2f);
    }

    public void ChangeState(MonsterState _state)
    {
        monsterState = _state;
        stateMachine.ChangeState(_state);
    }

    public void TakeHit(float damage, UnitType unit, float stun)
    {
        float damageP = ((def - GameManager.instance.debuff) * 0.06f) / (1 + (def - GameManager.instance.debuff) * 0.06f);
        float curDamage = damage * (1f - damageP);
        Hp -= curDamage;
        textManager.ShowDamageText(curDamage);
        if (unit == UnitType.Stun && gameObject.activeSelf)
        {
            StartCoroutine(StunCo(stun));
        }
    }

    public IEnumerator StunCo(float stunCool)
    {
        ChangeState(MonsterState.Stun);
        animator.SetBool("1_Move", false);
        animator.SetBool("5_Debuff", true);
        yield return new WaitForSeconds(stunCool);
        animator.SetBool("5_Debuff", false);
        ChangeState(MonsterState.Walk);
    }
}
