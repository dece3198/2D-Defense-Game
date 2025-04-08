using System.Collections;
using TMPro;
using UnityEngine;

public enum GameState
{
    None, Wait, StageStart, StageEnd
}

public class None : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }
}

public class Wait : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
        game.StartCoroutine(WaitCo(game));
    }

    public override void Exit(GameManager monster)
    {   
    }

    public override void Update(GameManager game)
    {
    }

    private IEnumerator WaitCo(GameManager game)
    {
        float time = 30;

        while (time > 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            game.timeText.text = $"{minutes:00} : {seconds:00}";
            yield return null;
        }
        game.ChanageState(GameState.StageStart);
    }
}

public class StageStart : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
        MonsterGenerator.instance.StartStage();
        game.StartCoroutine(StageStartCo(game));
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }

    private IEnumerator StageStartCo(GameManager game)
    {
        float time = 120;

        while(time > 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            game.timeText.text = $"{minutes:00} : {seconds:00}";
            yield return null;
        }
        game.ChanageState(GameState.StageEnd);
    }
}

public class StageEnd : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
        game.StartCoroutine(StageEndCo(game));
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }

    private IEnumerator StageEndCo(GameManager game)
    {
        float time = 30;
        while(time > 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            game.timeText.text = $"{minutes:00} : {seconds:00}";
            yield return null;
        }
        if(game.stage < 90)
        {
            game.ChanageState(GameState.StageStart);
        }
        else
        {
            //게임 승리창 실행
            game.ChanageState(GameState.None);
        }
    }
}

public class GameManager : Singleton<GameManager>
{
    public GameObject grid;
    public GameObject curTower;
    public Transform[] targetPos;
    public bool isSelect = false;
    public int stage = 0;
    public TextMeshProUGUI timeText;
    public StateMachine<GameState, GameManager> stateMachine = new StateMachine<GameState, GameManager>();
    public GameState gameState;

    private new void Awake()
    {
        base.Awake();
        stateMachine.Reset(this);
        stateMachine.AddState(GameState.None, new None());
        stateMachine.AddState(GameState.Wait, new Wait());
        stateMachine.AddState(GameState.StageStart, new StageStart());
        stateMachine.AddState(GameState.StageEnd, new StageEnd());
        ChanageState(GameState.Wait);
    }

    private void Start()
    {
        Time.timeScale = 2;
    }

    private void Update()
    {
        if(isSelect)
        {
        }
    }

    public void ChanageState(GameState state)
    {
        gameState = state;
        stateMachine.ChangeState(state);
    }
}
