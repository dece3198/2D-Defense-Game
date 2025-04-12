using UnityEngine;

public enum UnitState
{
    Idle, Walk, Attack
}

public class UnitIdle : BaseState<Unit>
{
    public override void Enter(Unit tower)
    {
        tower.animator.SetBool("Move", false);
    }

    public override void Exit(Unit tower)
    {

    }

    public override void Update(Unit tower)
    {

    }
}

public class UnitWalk : BaseState<Unit>
{
    public override void Enter(Unit tower)
    {
        tower.animator.SetBool("Move", true);
    }

    public override void Exit(Unit tower)
    {

    }

    public override void Update(Unit tower)
    {
        tower.transform.position = Vector2.MoveTowards(tower.transform.position, tower.target, 0.08f);

        if(Vector2.Distance(tower.transform.position, tower.target) < 0.1f)
        {
            tower.currentTilePos = tower.reservedTilePos;
            tower.ChanageState(UnitState.Idle);
        }
    }
}

public class UnitAttack : BaseState<Unit>
{
    public override void Enter(Unit tower)
    {

    }

    public override void Exit(Unit tower)
    {

    }

    public override void Update(Unit tower)
    {

    }
}


public class Unit : MonoBehaviour
{
    public Animator animator;
    public UnitRecipe unitRecipe;
    public UnitState unitState;
    private StateMachine<UnitState, Unit> stateMachine = new StateMachine<UnitState, Unit>();
    public Vector3 target;
    public Vector3Int currentTilePos;
    public Vector3Int reservedTilePos;
    private Vector2 dargStartPos;
    private bool isDragging = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine.Reset(this);
        stateMachine.AddState(UnitState.Idle, new UnitIdle());
        stateMachine.AddState(UnitState.Walk, new UnitWalk());
        stateMachine.AddState(UnitState.Attack, new UnitAttack());
        ChanageState(UnitState.Idle);
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

                UnitSpawner.instance.usedTiles.Remove(currentTilePos);
                UnitSpawner.instance.usedTiles.Add(currentCell);

                target = GameManager.instance.groundTileMap.GetCellCenterWorld(currentCell);

                reservedTilePos = currentCell;

                ChanageState(UnitState.Walk);
            }
            else
            {
                UnitSpawner.instance.curTower = this;
                UiManager.instance.AddTower(unitRecipe);
            }
        }


        GameManager.instance.grid.SetActive(false);
        isDragging = false;
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
}
