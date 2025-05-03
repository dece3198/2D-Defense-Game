using System.Collections;
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
                    monster.hpBar.transform.localScale = new Vector3(-0.004f, 0.01f, 1);
                    break;
                case 2:
                case 3:
                    monster.transform.localScale = new Vector3(1f, 1f, 1);
                    monster.hpBar.transform.localScale = new Vector3(0.004f, 0.01f, 1);
                    break;

            }

            monster.transform.position = Vector2.MoveTowards(monster.transform.position, target.position, (monster.speed - GameManager.instance.speedDebuff) * Time.deltaTime);

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
        MonsterSpawner.instance.EnterPool(monster.gameObject, GameManager.instance.stage);
        monster.posIndex = 0;
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {

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
                ChangeState(MonsterState.Die);
            }
            hpBar.value = Hp / maxHp;
        }
    }
    public MonsterState monsterState;
    StateMachine<MonsterState, BasicMonster> stateMachine = new StateMachine<MonsterState, BasicMonster>();
    public TextManager textManager;
    public Slider hpBar;
    public int posIndex = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine.Reset(this);
        stateMachine.AddState(MonsterState.Idle, new MonsterIdle());
        stateMachine.AddState(MonsterState.Walk, new MonsterWalk());
        stateMachine.AddState(MonsterState.Hit, new MonsterHit());
        stateMachine.AddState(MonsterState.Stun, new MonsterStun());
        stateMachine.AddState(MonsterState.Die, new MonsterDie());
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
        def = 30 + (2.5f * (GameManager.instance.stage + 1));
        ChangeState(MonsterState.Walk);
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

    public void ChangeState(MonsterState _state)
    {
        monsterState = _state;
        stateMachine.ChangeState(_state);
    }

    public void TakeHit(int damage, UnitType unit, float stun)
    {
        float damageP = ((def - GameManager.instance.debuff) * 0.06f) / (1 + (def - GameManager.instance.debuff) * 0.06f);
        float curDamage = damage * (1f - damageP);
        Hp -= curDamage;
        textManager.ExitPool(curDamage);
        if (unit == UnitType.Stun)
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
