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
        game.roundText.text = (game.stage + 1).ToString() + "Lv";

        switch (game.stage)
        {
            case 14 : game.lockImage[0].SetActive(false); break;
            case 24: game.lockImage[1].SetActive(false); break;
            case 39: game.lockImage[2].SetActive(false); break;
            case 54: game.lockImage[3].SetActive(false); break;
        }

    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }

    private IEnumerator StageStartCo(GameManager game)
    {
        float time = 40;

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
        if(game.missionFail <= 0)
        {
            game.Gold += 2;
        }
        else
        {
            game.missionFail -= 1;
        }
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }

    private IEnumerator StageEndCo(GameManager game)
    {
        float time = 10;
        while(time > 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            game.timeText.text = $"{minutes:00} : {seconds:00}";
            yield return null;
        }
        if(game.stage < 100)
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
    private Dictionary<UnitRecipe, int> defUnitCount = new Dictionary<UnitRecipe, int>();
    public GameState gameState;
    public Tilemap groundTileMap;
    [SerializeField] private int defDeBuff;
    public int DefDeBuff
    {
        get { return defDeBuff; }
        set
        {
            defDeBuff = value;
            defDeBuffText.text = defDeBuff.ToString();
        }
    }
    public int missionFail = 0;
    public GameObject[] lockImage;
    [SerializeField] private TextMeshProUGUI defDeBuffText;

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
        if(defUnitCount.ContainsKey(unit))
        {
            defUnitCount[unit]++;
        }
        else
        {
            defUnitCount[unit] = 1;
        }
        DefDebuff();
    }

    public void RemoveUnit(UnitRecipe unit)
    {
        if(defUnitCount.ContainsKey(unit))
        {
            defUnitCount[unit]--;
            if (defUnitCount[unit] <= 0)
            {
                defUnitCount.Remove(unit);
            }
            DefDebuff();
        }
    }

    private void DefDebuff()
    {
        DefDeBuff = 0;
        foreach (var u in defUnitCount.Keys)
        {
            DefDeBuff += u.debuff;
        }
    }

    public void ChanageState(GameState state)
    {
        gameState = state;
        stateMachine.ChangeState(state);
    }
}
