using System.Collections;
using UnityEngine;

public enum MonsterRating
{
    Normal, Rare, Epic, Unique, Legendary
}

public class DungeonMonsterWalk : BaseState<DungeonMonster>
{
    public override void Enter(DungeonMonster monster)
    {
        monster.animator.SetBool("1_Move", true);
    }

    public override void Exit(DungeonMonster monster)
    {
    }

    public override void Update(DungeonMonster monster)
    {
        Vector2 dirVec = monster.target.position - monster.rigid.position;
        Vector2 nextVec = dirVec.normalized * monster.speed * Time.fixedDeltaTime;
        monster.rigid.MovePosition(monster.rigid.position + nextVec);
        monster.rigid.linearVelocity = Vector2.zero;

        monster.viewDetector.FindTarget();

        if (monster.viewDetector.Target != null)
        {
            monster.ChangeState(MonsterState.Attack);
        }
    }
}

public class DungeonMonsterHit : BaseState<DungeonMonster>
{
    public override void Enter(DungeonMonster monster)
    {
        monster.rigid.linearVelocity = Vector2.zero;
        monster.animator.SetTrigger("Debuff");
        monster.StartStateCroutine(HitCo(monster));
    }

    public override void Exit(DungeonMonster monster)
    {

    }

    public override void Update(DungeonMonster monster)
    {

    }

    private IEnumerator HitCo(DungeonMonster monster)
    {
        foreach (var r in monster.spriteRenderers)
        {
            r.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var r in monster.spriteRenderers)
        {
            r.color = Color.white;
        }

        if(monster.monsterState != MonsterState.Die)
        {
            monster.ChangeState(MonsterState.Walk);
        }
    }
}

public class DungeonMonsterAttack : BaseState<DungeonMonster>
{
    public override void Enter(DungeonMonster monster)
    {
        monster.animator.SetBool("1_Move", false);
        monster.StartStateCroutine(AttackCo(monster));
    }

    public override void Exit(DungeonMonster monster)
    {
    }

    public override void Update(DungeonMonster monster)
    {
    }

    private IEnumerator AttackCo(DungeonMonster monster)
    {
        yield return new WaitForSeconds(2f);
        monster.animator.SetTrigger("2_Attack");
        monster.viewDetector.FindTarget();
        if(monster.viewDetector.Target != null)
        {
            monster.viewDetector.Target.GetComponent<Heart>().DungeonTakeHit(monster.damage);
        }

        if (monster.monsterState == MonsterState.Attack)
        {
            monster.ChangeState(MonsterState.Walk);
        }
    }
}

public class DungeonMonsterDie : BaseState<DungeonMonster>
{
    public override void Enter(DungeonMonster monster)
    {
        monster.StartStateCroutine(DieCo(monster));
        monster.animator.SetBool("1_Move", false);
        monster.animator.SetTrigger("4_Death");
        monster.isDie = false;
        monster.rigid.linearVelocity = Vector2.zero;
        monster.gameObject.layer = 0;
        monster.textManager.StopAllTexts();
    }

    public override void Exit(DungeonMonster monster)
    {
    }

    public override void Update(DungeonMonster monster)
    {
    }

    private IEnumerator DieCo(DungeonMonster monster)
    {
        foreach (var r in monster.spriteRenderers)
        {
            r.color = Color.white;
        }
        yield return new WaitForSeconds(2f);
        if (Random.value < 0.01)
        {
        }
        //WaitingRoom.instance.ExitCoin(diaCount, rubyCount, diaValue, rubyValue);
        monster.gameObject.layer = 7;
        monster.Hp = monster.maxHp;
    }
}

public class DungeonMonster : Monster, IInteractable
{
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            hpBar.value = Mathf.Clamp01(hp / maxHp);
            if (hp <= 0)
            {
                ChangeState(MonsterState.Die);
            }
        }
    }
    public float damage;
    public SpriteRenderer[] spriteRenderers;
    public SPUM_Prefabs sPUM_Prefabs;
    public Rigidbody2D target;
    public MonsterState monsterState;
    public Rigidbody2D rigid;
    public ViewDetector viewDetector;
    private StateMachine<MonsterState, DungeonMonster> stateMachine = new StateMachine<MonsterState, DungeonMonster>();
    public Coroutine curCoroutine;
    public bool isDie = true;

    private void Awake()
    {
        maxHp = Hp;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        viewDetector = GetComponent<ViewDetector>();
        stateMachine.Reset(this);
        stateMachine.AddState(MonsterState.Walk, new DungeonMonsterWalk());
        stateMachine.AddState(MonsterState.Hit, new DungeonMonsterHit());
        stateMachine.AddState(MonsterState.Attack, new DungeonMonsterAttack());
        stateMachine.AddState(MonsterState.Die, new DungeonMonsterDie());
        ChangeState(MonsterState.Walk);
    }

    private void Start()
    {
        target = GameManager.instance.heart;
    }


    private void FixedUpdate()
    {
        stateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<OrbitingObject>() != null)
        {
            if(isDie)
            {
                ChangeState(MonsterState.Hit);
                Vector2 knockbackDir = (rigid.position - target.position).normalized;
                rigid.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
                Hp -= collision.GetComponent<OrbitingObject>().damage;
                textManager.ShowDamageText(collision.GetComponent<OrbitingObject>().damage);
            }
        }
    }


    private void LateUpdate()
    {
        float direction = target.position.x < rigid.position.x ? 1f : -1f;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    public void StartMonster()
    {
        animator.SetBool("isDeath", false);
        gameObject.layer = 7;
        Hp = maxHp;
    }

    public void ChangeState(MonsterState state)
    {
        if(curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }
        monsterState = state;
        stateMachine.ChangeState(state);
    }

    public void StartStateCroutine(IEnumerator enumerator)
    {
        curCoroutine = StartCoroutine(enumerator);
    }

    public void DefenseTakeHit(float damage, UnitRecipe unitRecipe, float stun)
    {
    }

    public void DungeonTakeHit(float damage)
    {
        if (Hp > 0)
        {
            Hp -= damage;
            textManager.ShowDamageText(damage);
            ChangeState(MonsterState.Hit);
        }
    }
}
