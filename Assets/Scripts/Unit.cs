using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if(unit.unitRecipe.unitType != UnitType.Buffer)
        {
            unit.viewDetector.FindTarget();
            if (unit.viewDetector.Target != null)
            {
                if (unit.isAtk)
                {
                    unit.ChanageState(UnitState.Attack);
                }
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
        unit.transform.position = Vector2.MoveTowards(unit.transform.position, unit.target, 0.08f);

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
            if (unit.unitRecipe.unitAtkType == UnitAtkType.AD)
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
    private Dictionary<UnitRecipe, int> buffUnits = new Dictionary<UnitRecipe, int>();


    public Vector3 target;
    public Vector3Int currentTilePos;
    public Vector3Int reservedTilePos;
    private Vector2 dargStartPos;
    private bool isDragging = false;

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
                if (GameManager.instance.isSelect)
                {
                    ChanageState(UnitState.Walk);
                    Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int currentCell = GameManager.instance.groundTileMap.WorldToCell(mPos);

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
                viewDetector.Target.GetComponentInChildren<IInteractable>().TakeHit(rand, unitRecipe.unitType, unitRecipe.stun);
            }

            if (unitRecipe.unitRating == UnitRating.Legendary)
            {
                if (Random.value < unitRecipe.skillPercent)
                {
                    GameObject _skill = skillStack.Pop();
                    _skill.GetComponent<Animator>().SetTrigger("Skill");
                    _skill.transform.position = viewDetector.Target.transform.position;
                    float skillDamage = ((minAtk + maxAtk) / 2) * unitRecipe.skillDamage;
                    _skill.GetComponent<ViewDetector>().FindSkillTarget(skillDamage, unitRecipe.unitType, unitRecipe.skillStun);
                    StartCoroutine(SkillCo(_skill));
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
            viewDetector.FindBufferTarget(unitRecipe);
        }
    }

    public void SetBuff(UnitRecipe _unitRecipe)
    {
        if(buffUnits.ContainsKey(_unitRecipe))
        {
            buffUnits[_unitRecipe]++;
        }
        else
        {
            buffUnits[_unitRecipe] = 1;
        }

        RecalculateBuff();
    }

    public void ResetBuff(UnitRecipe _unitRecipe)
    {
        if(buffUnits.ContainsKey(_unitRecipe))
        {
            buffUnits[_unitRecipe]--;
            if (buffUnits[_unitRecipe] <= 0)
            {
                buffUnits.Remove(_unitRecipe);
            }

            if(buffUnits.Count > 0)
            {
                RecalculateBuff();
            }
            else
            {
                maxAtk = unitRecipe.maxAtk;
                minAtk = unitRecipe.minAtk;
            }
        }
    }

    public void RecalculateBuff()
    {
        float tempBuff = 0;
        foreach (var u in buffUnits)
        {
            var unitRecipe = u.Key;

            tempBuff += unitRecipe.buff;
        }
        maxAtk = unitRecipe.maxAtk + (unitRecipe.maxAtk * tempBuff);
        minAtk = unitRecipe.minAtk + (unitRecipe.minAtk * tempBuff);
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
