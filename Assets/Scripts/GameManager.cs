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
        game.Gold += 2;
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
    public HashSet<UnitRecipe> speedHashSet = new HashSet<UnitRecipe>();
    public int debuff = 0;
    public float speedDebuff = 0;


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
        //Time.timeScale = 4;
        Gold += 8;
    }

    public void AddUnit(UnitRecipe unit)
    {
        if(unit.unitType == UnitType.Debuff)
        {
            defHashSet.Add(unit);
            DefDebuff();
        }
        else if(unit.unitType == UnitType.SpeedDebuff)
        {
            speedHashSet.Add(unit);
            SpeedDebuff();
        }
    }

    public void RemoveUnit(UnitRecipe unit)
    {
        if (unit.unitType == UnitType.Debuff)
        {
            defHashSet.Remove(unit);
            DefDebuff();
        }
        else if (unit.unitType == UnitType.SpeedDebuff)
        {
            speedHashSet.Remove(unit);
            SpeedDebuff();
        }
    }

    private void DefDebuff()
    {
        debuff = 0;
        foreach (var u in defHashSet)
        {
            debuff += u.debuff;
        }
    }

    private void SpeedDebuff()
    {
        speedDebuff = 0;
        foreach(var u in speedHashSet)
        {
            speedDebuff += u.speedDebuff;
        }
    }

    public void ChanageState(GameState state)
    {
        gameState = state;
        stateMachine.ChangeState(state);
    }
}
