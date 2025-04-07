using UnityEngine;
using UnityEngine.UI;

public class MonsterIdle : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        monster.animator.SetFloat("RunState", 0f);
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
        monster.animator.SetFloat("RunState", 0.5f);
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {
        if(GameManager.instance.targetPos.Length != 0)
        {
            Transform target = GameManager.instance.targetPos[GameManager.instance.posIndex];
            switch(GameManager.instance.posIndex)
            {
                case 0:
                case 1:
                    monster.transform.localScale = new Vector3(-1f, 1f, 1);
                    monster.hpBar.transform.localScale = new Vector3(-0.005f, 0.02f, 1);
                    break;
                case 2:
                case 3:
                    monster.transform.localScale = new Vector3(1f, 1f, 1);
                    monster.hpBar.transform.localScale = new Vector3(0.005f, 0.02f, 1);
                    break;

            }
            monster.transform.position = Vector2.MoveTowards(monster.transform.position, target.position, monster.speed);

            if(Vector2.Distance(monster.transform.position, target.position) < 0.1f)
            {
                GameManager.instance.posIndex++;
                if(GameManager.instance.posIndex >= GameManager.instance.targetPos.Length)
                {
                    GameManager.instance.posIndex = 0;
                }
            }
        }
    }
}

public class MonsterAttack : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {

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
        monster.animator.SetFloat("RunState", 1f);
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
    }

    public override void Exit(BasicMonster monster)
    {

    }

    public override void Update(BasicMonster monster)
    {

    }
}

public class BasicMonster : Monster
{
    [SerializeField] private float hp;
    public float Hp 
    { 
        get { return hp; } 
        set 
        { 
            value = hp; 
        }
    }

    public MonsterState state;
    StateMachine<MonsterState, BasicMonster> stateMachine = new StateMachine<MonsterState, BasicMonster>();
    public Slider hpBar;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine.Reset(this);
        stateMachine.AddState(MonsterState.Idle, new MonsterIdle());
        stateMachine.AddState(MonsterState.Walk, new MonsterWalk());
        stateMachine.AddState(MonsterState.Attack, new MonsterAttack());
        stateMachine.AddState(MonsterState.Stun, new MonsterStun());
        stateMachine.AddState(MonsterState.Die, new MonsterDie());
    }

    private void Start()
    {
        ChangeState(MonsterState.Walk);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(MonsterState _state)
    {
        state = _state;
        stateMachine.ChangeState(_state);
    }

}
