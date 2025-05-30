using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        float time = 5;

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
        MonsterSpawner.instance.StartStage();
        game.StartCoroutine(StageStartCo(game));
        game.roundText.text = (game.stage + 1).ToString() + "R";
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
        game.Gold += 4;
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
    [SerializeField] private int gold;
    public int Gold
    {
        get { return gold; }
        set 
        {
            gold = value;
            goldText.text = gold.ToString();
        }
    }
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI goldText;
    public StateMachine<GameState, GameManager> stateMachine = new StateMachine<GameState, GameManager>();
    public GameState gameState;
    public Tilemap groundTileMap;
    public HashSet<UnitRecipe> defHashSet = new HashSet<UnitRecipe>();
    public int debuff = 0;


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
        Gold += 12;
    }

    public void AddUnit(UnitRecipe unit)
    {
        defHashSet.Add(unit);
        DefDebuff();
    }

    public void RemoveUnit(UnitRecipe unit)
    {
        defHashSet.Remove(unit);
        DefDebuff();
    }

    private void DefDebuff()
    {
        debuff = 0;
        foreach (var u in defHashSet)
        {
            debuff += u.debuff;
        }
    }

    public void ChanageState(GameState state)
    {
        gameState = state;
        stateMachine.ChangeState(state);
    }
}
