using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle, Walk, Attack
}
public enum UnitType
{
   DPS, DDps, AD, Magic, Debuff, Stun, SpeedDebuff
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
        if (unit.viewDetector.Target != null)
        {
            if(unit.isAtk)
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
        if (unit.unitRecipe.unitType == UnitType.DDps)
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
            if (unit.unitRecipe.unitType == UnitType.AD)
            {
                yield return new WaitForSeconds(0.5f);
            }
            unit.Attack();
        }
        unit.ChanageState(UnitState.Idle);
    }
}


public class Unit : MonoBehaviour
{
    public float atkSpeed;
    public float stun;
    public UnitRecipe unitRecipe;

    public SPUM_Prefabs sPUM_Prefabs;
    public Animator animator;
    public ViewDetector viewDetector;
    public UnitState unitState;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPos;
    public Stack<GameObject> bulletStack = new Stack<GameObject>();
    public bool isAtk = true;

    private StateMachine<UnitState, Unit> stateMachine = new StateMachine<UnitState, Unit>();


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
        if(unitRecipe.unitType == UnitType.Debuff || unitRecipe.unitType == UnitType.SpeedDebuff)
        {
            GameManager.instance.AddUnit(unitRecipe);
        }

        if(unitRecipe.unitType == UnitType.AD || unitRecipe.unitType == UnitType.Magic)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject _bullet = Instantiate(bullet, transform);
                _bullet.GetComponent<Bullet>().unit = this;
                bulletStack.Push(_bullet);
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
                UnitSpawner.instance.curTower = this;
                UiManager.instance.AddUnit(unitRecipe);
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
            int rand = Random.Range(unitRecipe.minAtk, unitRecipe.maxAtk);
            if (unitRecipe.unitType == UnitType.AD || unitRecipe.unitType == UnitType.Magic)
            {
                GameObject _bullet = bulletStack.Pop();
                _bullet.transform.position = bulletPos.position;
                _bullet.GetComponent<Bullet>().target = viewDetector.Target;
                _bullet.SetActive(true);
            }
            else
            {
                viewDetector.Target.GetComponentInChildren<IInteractable>().TakeHit(rand, unitRecipe.unitType, stun);
            }

        }
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void ChanageState(UnitState state)
    {
        unitState = state;
        stateMachine.ChangeState(state);
    }

    private void OnDestroy()
    {
        if (unitRecipe.unitType == UnitType.Debuff || unitRecipe.unitType == UnitType.SpeedDebuff)
        {
            GameManager.instance.RemoveUnit(unitRecipe);
        }
    }

    public void StartAttackCo()
    {
        StartCoroutine(AttackCo());
    }

    private IEnumerator AttackCo()
    {
        isAtk = false;
        yield return new WaitForSeconds(atkSpeed);
        isAtk = true;
    }
}
