using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        monster.animator.SetBool("1_Move", false);
        monster.animator.SetBool("5_Debuff", true);
    }

    public override void Exit(BasicMonster monster)
    {
        monster.animator.SetBool("5_Debuff", false);
    }

    public override void Update(BasicMonster monster)
    {
        if(monster.stunTime <= 0)
        {
            monster.ChangeState(MonsterState.Walk);
        }
        else
        {
            monster.stunTime -= Time.deltaTime;
        }
    }
}

public class MonsterDie : BaseState<BasicMonster>
{
    public override void Enter(BasicMonster monster)
    {
        MonsterSpawner.instance.MonsterCount--;
        monster.gameObject.layer = 0;
        monster.textManager.StopAllTexts();
        monster.animator.SetTrigger("4_Death");
        if (monster.transform.localScale.x == -1)
        {
            monster.textManager.transform.localScale = new Vector3(1f, 1f, 1);
        }
        else
        {
            monster.textManager.transform.localScale = new Vector3(-1f, 1f, 1);
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
            if (monster.transform.localScale.x == -1)
            {
                monster.textManager.transform.localScale = new Vector3(1f, 1f, 1);
            }
            else
            {
                monster.textManager.transform.localScale = new Vector3(-1f, 1f, 1);
            }

            if(monster.monsterType == MonsterType.Mission)
            {
                GameManager.instance.Jam += monster.jam;
                monster.Hp = monster.maxHp;
                MonsterSpawner.instance.missionTimeUi[monster.value].SetActive(false);
                monster.gameObject.SetActive(false);
            }
            else
            {
                MonsterSpawner.instance.EnterPool(monster.transform.parent.GetComponent<SPUM_Prefabs>().gameObject, monster.stage);
            }
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
            hpBar.value = Mathf.Clamp01(hp/maxHp);
        }
    }
    public float stunTime = 0;
    public MonsterState monsterState;
    public SpriteRenderer[] spriteRenderers;
    StateMachine<MonsterState, BasicMonster> stateMachine = new StateMachine<MonsterState, BasicMonster>();
    private Dictionary<UnitRecipe, int> units = new Dictionary<UnitRecipe, int>();
    private float missionTime = 0;
    public int value = 0;
    public float missionCoolTime = 0;

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
        if(monsterType != MonsterType.Mission)
        {
            if (monsterType == MonsterType.Boss)
            {
                Hp = 200 * Mathf.Pow(1.127745f, GameManager.instance.stage) * 3;
            }
            else
            {
                Hp = (200 * Mathf.Pow(1.127745f, GameManager.instance.stage)) * 0.5f;
            }
            def = (2f * (GameManager.instance.stage + 1));
        }
        maxHp = Hp;
        ChangeState(MonsterState.Walk);
        speed = MonsterSpawner.instance.speedDic[monsterType];
        missionTime = 0;
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

        if(monsterType == MonsterType.Mission)
        {
            missionTime += Time.deltaTime;

            if(missionTime >= 90)
            {
                missionTime = 0;
                GameManager.instance.missionFail += 3;
                Hp = maxHp;
            }
        }

    }

    public void SetSpeed(float debuff, UnitRecipe unit)
    {
        if(units.ContainsKey(unit))
        {
            units[unit]++;
        }
        else
        {
            units[unit] = 1;
        }
        RecalculateSpeed();
    }

    public float GetCurSpeed()
    {
        return speed;
    }

    public void ResetSpeed(UnitRecipe unit)
    {
        if (units.ContainsKey(unit))
        {
            units[unit]--;

            if (units[unit] <= 0)
            {
                units.Remove(unit);
            }
            
            if(units.Count > 0)
            {
                RecalculateSpeed();
            }
            else
            {
                speed = MonsterSpawner.instance.speedDic[monsterType];
            }
        }
    }

    public void RecalculateSpeed()
    {
        float tempDeBuff = 1f;

        foreach (var u in units)
        {
            UnitRecipe unitRecipe = u.Key;
            int count = u.Value;

            for (int i = 0; i < count; i++)
            {
                tempDeBuff *= (1f - unitRecipe.speedDebuff);
            }
        }
        speed = Mathf.Clamp(MonsterSpawner.instance.speedDic[monsterType] * tempDeBuff, 0.1f, 2f);
    }

    public void ChangeState(MonsterState _state)
    {
        monsterState = _state;
        stateMachine.ChangeState(_state);
    }

    public void TakeHit(float damage, UnitRecipe unitRecipe, float stun)
    {
        if(Hp > 0)
        {
            if(unitRecipe.unitSkillType == UnitSkillType.PD)
            {
                float defFactor = def - GameManager.instance.DefDeBuff;
                float scale = GameManager.instance.defDamageDic[GameManager.instance.curGameLevel];
                float multiplier;
                if(defFactor >= 0f)
                {
                    multiplier = 1f / (1f + defFactor * scale);
                }
                else
                {
                    multiplier = 1 - defFactor * scale;
                }
                float finalDamage = damage * multiplier;
                Hp -= finalDamage;
                textManager.ShowDamageText(finalDamage);
            }
            else
            {
                float finalDamage = damage + (damage * (GameManager.instance.DefDeBuff * 0.002f));
                Hp -= finalDamage;
                textManager.ShowDamageText(finalDamage);
            }
            
            StartCoroutine(HitCo());
            if (unitRecipe.unitType == UnitType.Stun && gameObject.activeSelf)
            {
                if(monsterState != MonsterState.Die)
                {
                    if (monsterState == MonsterState.Stun)
                    {
                        stunTime = Mathf.Max(stunTime, stun);
                    }
                    else
                    {
                        stunTime = Mathf.Max(stunTime, stun);
                        ChangeState(MonsterState.Stun);
                    }
                }
            }
        }
    }

    private IEnumerator HitCo()
    {
        foreach(var r in spriteRenderers)
        {
            r.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var r in spriteRenderers)
        {
            r.color = Color.white;
        }
    }
}
