using UnityEngine;

public enum TowerState
{
    Idle, Walk, Attack
}

public class TowerIdle : BaseState<Tower>
{
    public override void Enter(Tower tower)
    {
        tower.animator.SetFloat("RunState", 0f);
    }

    public override void Exit(Tower tower)
    {

    }

    public override void Update(Tower tower)
    {

    }
}

public class TowerWalk : BaseState<Tower>
{
    public override void Enter(Tower tower)
    {
        tower.animator.SetFloat("RunState", 0.5f);
    }

    public override void Exit(Tower tower)
    {

    }

    public override void Update(Tower tower)
    {
        tower.transform.position = Vector2.MoveTowards(tower.transform.position, tower.target, 0.08f);

        if(Vector2.Distance(tower.transform.position, tower.target) < 0.1f)
        {
            tower.currentTilePos = tower.reservedTilePos;
            tower.ChanageState(TowerState.Idle);
        }
    }
}

public class TowerAttack : BaseState<Tower>
{
    public override void Enter(Tower tower)
    {

    }

    public override void Exit(Tower tower)
    {

    }

    public override void Update(Tower tower)
    {

    }
}


public class Tower : MonoBehaviour
{
    public Animator animator;
    public TowerState towerState;
    private StateMachine<TowerState, Tower> stateMachine = new StateMachine<TowerState, Tower>();
    public Vector3 target;
    public Vector3Int currentTilePos;
    public Vector3Int reservedTilePos;
    private Vector2 dargStartPos;
    private bool isDragging = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine.Reset(this);
        stateMachine.AddState(TowerState.Idle, new TowerIdle());
        stateMachine.AddState(TowerState.Walk, new TowerWalk());
        stateMachine.AddState(TowerState.Attack, new TowerAttack());
        ChanageState(TowerState.Idle);
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
        if(GameManager.instance.isSelect)
        {
            if (towerState != TowerState.Walk)
            {
                if (isDragging)
                {
                    Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int currentCell = GameManager.instance.groundTileMap.WorldToCell(mPos);

                    TowerSpawner.instance.usedTiles.Remove(currentTilePos);
                    TowerSpawner.instance.usedTiles.Add(currentCell);

                    target = GameManager.instance.groundTileMap.GetCellCenterWorld(currentCell);

                    reservedTilePos = currentCell;

                    ChanageState(TowerState.Walk);
                }
                else
                {
                    Debug.Log("UIÁ¤º¸");
                }
            }
        }
        else
        {

        }


        GameManager.instance.grid.SetActive(false);
        isDragging = false;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void ChanageState(TowerState state)
    {
        towerState = state;
        stateMachine.ChangeState(state);
    }
}
