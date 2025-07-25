using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle, Walk, Attack
}
public enum UnitType
{
    None, DefDebuff, Stun, SpeedDebuff, Buffer
}

public enum UnitAtkType
{
    DPS, DDps, AD
}


public class UnitIdle : BaseState<Unit>
{
    public override void Enter(Unit unit)
    {
        unit.animator.SetBool("1_Move", false);
    }

    public override void Exit(Unit unit)
    {

    }

    public override void Update(Unit unit)
    {
        unit.viewDetector.FindTarget();
        if(unit.viewDetector.Target != null)
        {
            if (unit.isAtk)
            {
                unit.ChanageState(UnitState.Attack);
            }
        }
    }
}

public class UnitWalk : BaseState<Unit>
{
    public override void Enter(Unit unit)
    {
        unit.animator.SetBool("1_Move", true);
    }

    public override void Exit(Unit unit)
    {

    }

    public override void Update(Unit unit)
    {
        unit.transform.position = Vector2.MoveTowards(unit.transform.position, unit.target, 7f * Time.deltaTime);

        if(Vector2.Distance(unit.transform.position, unit.target) < 0.1f)
        {
            unit.currentTilePos = unit.reservedTilePos;
            unit.ChanageState(UnitState.Idle);
        }
    }
}

public class UnitAttack : BaseState<Unit>
{
    public override void Enter(Unit unit)
    {
        unit.StartAttackCo();
        unit.StartCoroutine(AttackCo(unit));
    }

    public override void Exit(Unit unit)
    {
    }

    public override void Update(Unit unit)
    {

    }

    private IEnumerator AttackCo(Unit unit)
    {
        if (unit.unitRecipe.unitAtkType == UnitAtkType.DDps)
        {
            for (int i = 0; i < 5; i++)
            {
                unit.sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
                unit.Attack();
                yield return new WaitForSeconds(0.25f);
            }
        }
        else
        {
            unit.sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
            if (unit.transform.CompareTag("Archer"))
            {
                yield return new WaitForSeconds(0.5f);
            }
            unit.Attack();
        }
        unit.ChanageState(unit.unitState == UnitState.Walk ? UnitState.Walk : UnitState.Idle);
    }
}


public class Unit : MonoBehaviour
{
    public float minAtk;
    public float maxAtk;
    public float buff;
    public UnitRecipe unitRecipe;
    public SPUM_Prefabs sPUM_Prefabs;
    public Animator animator;
    public ViewDetector viewDetector;
    public UnitState unitState;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPos;
    [SerializeField] private GameObject skill;
    [SerializeField] private Transform skillPos;
    [SerializeField] private Stack<GameObject> skillStack = new Stack<GameObject>();

    public Stack<GameObject> bulletStack = new Stack<GameObject>();
    public bool isAtk = true;

    private StateMachine<UnitState, Unit> stateMachine = new StateMachine<UnitState, Unit>();
    private Dictionary<UnitRecipe, (int count, float buff)> buffUnits = new Dictionary<UnitRecipe, (int count, float buff)>();


    public Vector3 target;
    public Vector3Int currentTilePos;
    public Vector3Int reservedTilePos;
    private Vector2 dargStartPos;
    private bool isDragging = false;

    private float buffTime = 0;
    private bool isBuff = false;

    private void Awake()
    {
        sPUM_Prefabs = transform.parent.GetComponent<SPUM_Prefabs>();
        sPUM_Prefabs.OverrideControllerInit();
        animator = GetComponent<Animator>();
        viewDetector = GetComponent<ViewDetector>();
        stateMachine.Reset(this);
        stateMachine.AddState(UnitState.Idle, new UnitIdle());
        stateMachine.AddState(UnitState.Walk, new UnitWalk());
        stateMachine.AddState(UnitState.Attack, new UnitAttack());
        ChanageState(UnitState.Idle);
    }

    private void Start()
    {
        maxAtk = unitRecipe.maxAtk;
        minAtk = unitRecipe.minAtk;
        buff = unitRecipe.buff;

        if (unitRecipe.unitType == UnitType.DefDebuff)
        {
            GameManager.instance.AddUnit(unitRecipe);
        }

        if(unitRecipe.unitAtkType == UnitAtkType.AD)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject _bullet = Instantiate(bullet, sPUM_Prefabs.transform);
                _bullet.GetComponent<Bullet>().unit = this;
                bulletStack.Push(_bullet);
            }
        }

        if(skill != null)
        {
            for(int i = 0; i < 10; i++)
            {
                GameObject _skill = Instantiate(skill, sPUM_Prefabs.transform);
                if(skillPos != null)
                {
                    _skill.transform.position = skillPos.position;
                }
                skillStack.Push(_skill);
            }
        }
    }

    public void EnterPool(GameObject _bullet)
    {
        _bullet.SetActive(false);
        _bullet.transform.position = transform.position;
        bulletStack.Push(_bullet);
    }

    public void OnMouseDown()
    {
        dargStartPos = Input.mousePosition;
        isDragging = false;
    }

    public void OnMouseDrag()
    {
        float dragDistance = Vector2.Distance(dargStartPos, Input.mousePosition);

        if(dragDistance > 20f && !isDragging)
        {
            isDragging = true;
            GameManager.instance.grid.SetActive(true);
        }
    }

    public void OnMouseUp()
    {
        if (unitState != UnitState.Walk)
        {
            if (isDragging)
            {
                Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int currentCell = GameManager.instance.groundTileMap.WorldToCell(mPos);

                GameObject targetUnitGO = UnitSpawner.instance.unitList.Find(u => u.GetComponentInChildren<Unit>().currentTilePos == currentCell);

                if(targetUnitGO != null)
                {
                    Unit targetUnit = targetUnitGO.GetComponentInChildren<Unit>();
                    SwapUnit(this, targetUnit);
                }
                else if (GameManager.instance.isSelect)
                {
                    ChanageState(UnitState.Walk);

                    UnitSpawner.instance.usedTiles.Remove(currentTilePos);
                    UnitSpawner.instance.usedTiles.Add(currentCell);

                    target = GameManager.instance.groundTileMap.GetCellCenterWorld(currentCell);

                    reservedTilePos = currentCell;
                }
            }
            else
            {
                UnitSpawner.instance.curUnit = transform.parent.GetComponent<SPUM_Prefabs>().gameObject;
                UiManager.instance.AddUnit(this);
            }
        }
        

        GameManager.instance.grid.SetActive(false);
        isDragging = false;
    }

    private void SwapUnit(Unit a, Unit b)
    {
        Vector3Int tempTile = a.currentTilePos;
        Vector3 tempTarget = a.target;

        a.reservedTilePos = b.currentTilePos;
        a.target = GameManager.instance.groundTileMap.GetCellCenterWorld(b.currentTilePos);
        a.ChanageState(UnitState.Walk);

        b.reservedTilePos = tempTile;
        b.target = GameManager.instance.groundTileMap.GetCellCenterWorld(tempTile);
        b.ChanageState(UnitState.Walk);
    }

    public void Attack()
    {
        viewDetector.FindTarget();
        if (viewDetector.Target != null)
        {
            float rand = Random.Range(maxAtk, minAtk);
            if (unitRecipe.unitAtkType == UnitAtkType.AD)
            {
                GameObject _bullet = bulletStack.Pop();
                _bullet.transform.position = bulletPos.position;
                _bullet.GetComponent<Bullet>().target = viewDetector.Target;
                _bullet.SetActive(true);
            }
            else
            {
                viewDetector.Target.GetComponentInChildren<IInteractable>().TakeHit(rand, unitRecipe, unitRecipe.stun);
            }

            if (unitRecipe.unitRating >= Rating.Legendary)
            {
                if (Random.value < unitRecipe.skillPercent)
                {
                    if(unitRecipe.unitType != UnitType.Buffer)
                    {
                        GameObject _skill = skillStack.Pop();
                        _skill.GetComponent<Animator>().SetTrigger("Skill");
                        _skill.transform.position = viewDetector.Target.transform.position;
                        float skillDamage = ((minAtk + maxAtk) / 2) * unitRecipe.skillDamage;
                        _skill.GetComponent<ViewDetector>().FindSkillTarget(skillDamage, unitRecipe, unitRecipe.skillStun);
                        StartCoroutine(SkillCo(_skill));
                    }
                    else
                    {
                        GameObject _skill = skillStack.Pop();
                        _skill.GetComponent<Animator>().SetTrigger("Skill");
                        isBuff = true;
                        buffTime = Mathf.Max(buffTime, 3f);
                        buff = unitRecipe.skillDamage;
                        viewDetector.UnitSetBuff(unitRecipe, buff);
                    }
                }
            }
        }
    }

    private void Update()
    {
        stateMachine.Update();
        if(unitRecipe.unitType == UnitType.SpeedDebuff)
        {
            viewDetector.DebuffTarget(unitRecipe.speedDebuff);
        }
        else if(unitRecipe.unitType == UnitType.Buffer)
        {
            viewDetector.FindBufferTarget(unitRecipe, buff);
        }

        if(isBuff)
        {
            buffTime -= Time.deltaTime;
            if(buffTime <= 0f)
            {
                isBuff = false;
                buff = unitRecipe.buff;
                viewDetector.UnitResetBuff(unitRecipe, buff);
            }
        }
    }

    public void SetBuff(UnitRecipe _unitRecipe, float _buff)
    {
        if(buffUnits.ContainsKey(_unitRecipe))
        {
            var current = buffUnits[_unitRecipe];
            buffUnits[_unitRecipe] = (current.count +1, _buff);
        }
        else
        {
            buffUnits[_unitRecipe] = (1, _buff);

        }

        RecalculateBuff();
    }

    public void ResetBuff(UnitRecipe _unitRecipe, float buff)
    {
        if (buffUnits.ContainsKey(_unitRecipe))
        {
            var current = buffUnits[_unitRecipe];
            int newCount = current.count - 1;

            if (newCount <= 0)
            {
                buffUnits.Remove(_unitRecipe);
            }
            else
            {
                buffUnits[_unitRecipe] = (newCount, buff);
            }

            RecalculateBuff();
        }
    }

    public void RecalculateBuff()
    {
        float tempBuff = 0;
        foreach (var u in buffUnits)
        {
            float buffValue = u.Value.buff;

            tempBuff += buffValue;
        }
        
        if(unitRecipe.unitSkillType == UnitSkillType.PD)
        {
            maxAtk = unitRecipe.maxAtk + (unitRecipe.maxAtk * tempBuff);
            minAtk = unitRecipe.minAtk + (unitRecipe.minAtk * tempBuff);
        }
        else
        {
            maxAtk = unitRecipe.maxAtk + (unitRecipe.maxAtk * tempBuff * 0.2f);
            minAtk = unitRecipe.minAtk + (unitRecipe.minAtk * tempBuff * 0.2f);
        }
    }



    public void ChanageState(UnitState state)
    {
        unitState = state;
        stateMachine.ChangeState(state);
    }

    private void OnDestroy()
    {
        if (unitRecipe.unitType == UnitType.DefDebuff)
        {
            GameManager.instance.RemoveUnit(unitRecipe);
        }
    }


    private IEnumerator SkillCo(GameObject _skill)
    {
        yield return new WaitForSeconds(3f);
        skillStack.Push(_skill);
    }

    public void StartAttackCo()
    {
        StartCoroutine(AttackCo());

    }

    private IEnumerator AttackCo()
    {
        isAtk = false;
        yield return new WaitForSeconds(unitRecipe.atkCoolTime);
        isAtk = true;
    }
}
