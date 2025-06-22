using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GameState
{
    None, Wait, StageStart, StageEnd, GameEnd, GameWinner
}

public enum RankType
{
    unRank, Bronze1, Bronze2, Bronze3, Silver1, Silver2, Silver3, Gold1, Gold2, Gold3,
    Platinum1, Platinum2, Platinum3, Diamond1, Diamond2, Diamond3, Master, GrandMaster, Challenger
}

public enum GameLevel
{
    Bronze, Silver, Gold, Platinum, Diamond, Master, Challenger
}

[System.Serializable]
public class RankCondition
{
    public GameLevel level;
    public int monsterThreshold0; // ex) 0마리일 때
    public int monsterThreshold50; // 50마리 이하일 때
    public RankType rankIfZero;
    public RankType rankIfBelow50;
    public RankType rankIfOther;
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

        if(game.gameState != GameState.GameEnd)
        {
            game.ChanageState(GameState.StageEnd);
        }
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

        if(game.gameState != GameState.GameEnd)
        {
            if (game.stage < 100)
            {
                game.ChanageState(GameState.StageStart);
            }
            else
            {
                //게임 승리창 실행
                game.ChanageState(GameState.GameWinner);
            }
        }
    }
}

public class GameEnd : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
        MonsterSpawner.instance.StopStage();
        MonsterSpawner.instance.EndGame();
        UnitSpawner.instance.EndGame();
        UiManager.instance.EndGame();
        game.stage = 0;
        game.Gold = 12;
        game.Jam = 0;
        game.loseText.SetActive(true);
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }
}

public class GameWinner : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
        MonsterSpawner.instance.StopStage();
        MonsterSpawner.instance.EndGame();
        UnitSpawner.instance.EndGame();
        UiManager.instance.EndGame();
        game.stage = 0;
        game.Gold = 12;
        game.Jam = 0;
        game.clear.GameClear();
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }
}

public class GameManager : Singleton<GameManager>
{
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
    [SerializeField] private int jam;
    public int Jam
    {
        get { return jam; }
        set
        {
            jam = value;
            jamText.text = jam.ToString();
        }
    }
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
    public int stage = 0;
    public int missionFail = 0;

    public Rating rating;
    public Rank[] ranks;
    public RankType curRank;
    public GameLevel curGameLevel;
    public GameObject grid;
    public GameObject[] lockImage;
    public GameObject loseText;
    public GameObject mainUi;
    public Transform[] targetPos;
    public FadeInOut fade;
    public Clear clear;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI jamText;
    [SerializeField] private TextMeshProUGUI defDeBuffText;
    public StateMachine<GameState, GameManager> stateMachine = new StateMachine<GameState, GameManager>();
    private Dictionary<UnitRecipe, int> defUnitCount = new Dictionary<UnitRecipe, int>();
    public Dictionary<RankType, Rank> rankDic = new Dictionary<RankType, Rank>();
    public Dictionary<GameLevel, RankCondition> rankConditionDic = new Dictionary<GameLevel, RankCondition>();
    public Dictionary<GameLevel, float> defDamageDic = new Dictionary<GameLevel, float>();
    public GameState gameState;
    public Tilemap groundTileMap;
    public bool isSelect = false;
    public bool isX2 = false;

    public List<RankCondition> rankConditionList = new();

    private new void Awake()
    {
        base.Awake();
        stateMachine.Reset(this);
        stateMachine.AddState(GameState.None, new None());
        stateMachine.AddState(GameState.Wait, new Wait());
        stateMachine.AddState(GameState.StageStart, new StageStart());
        stateMachine.AddState(GameState.StageEnd, new StageEnd());
        stateMachine.AddState(GameState.GameEnd, new GameEnd());
        stateMachine.AddState(GameState.GameWinner, new GameWinner());
        rankDic.Add(RankType.unRank, ranks[0]);
        rankDic.Add(RankType.Bronze1, ranks[1]);
        rankDic.Add(RankType.Bronze2, ranks[2]);
        rankDic.Add(RankType.Bronze3, ranks[3]);
        defDamageDic.Add(GameLevel.Bronze, 0.001f);
        defDamageDic.Add(GameLevel.Silver, 0.0025f);
        defDamageDic.Add(GameLevel.Gold, 0.005f);
        defDamageDic.Add(GameLevel.Platinum, 0.01f);
        defDamageDic.Add(GameLevel.Diamond, 0.015f);
        defDamageDic.Add(GameLevel.Master, 0.03f);
        ChanageState(GameState.Wait);

        RankCondition bronzeCond = new RankCondition
        {
            level = GameLevel.Bronze,
            monsterThreshold0 = 0,
            monsterThreshold50 = 50,
            rankIfZero = RankType.Bronze3,
            rankIfBelow50 = RankType.Bronze2,
            rankIfOther = RankType.Bronze1
        };

        RankCondition SilverCond = new RankCondition
        {
            level = GameLevel.Silver,
            monsterThreshold0 = 0,
            monsterThreshold50 = 50,
            rankIfZero = RankType.Silver3,
            rankIfBelow50 = RankType.Silver2,
            rankIfOther = RankType.Silver1
        };

        RankCondition GoldCond = new RankCondition
        {
            level = GameLevel.Gold,
            monsterThreshold0 = 0,
            monsterThreshold50 = 50,
            rankIfZero = RankType.Gold3,
            rankIfBelow50 = RankType.Gold2,
            rankIfOther = RankType.Gold1
        };

        RankCondition PlatinumCond = new RankCondition
        {
            level = GameLevel.Platinum,
            monsterThreshold0 = 0,
            monsterThreshold50 = 50,
            rankIfZero = RankType.Platinum3,
            rankIfBelow50 = RankType.Platinum2,
            rankIfOther = RankType.Platinum1
        };

        RankCondition DiamondCond = new RankCondition
        {
            level = GameLevel.Diamond,
            monsterThreshold0 = 0,
            monsterThreshold50 = 50,
            rankIfZero = RankType.Diamond3,
            rankIfBelow50 = RankType.Diamond2,
            rankIfOther = RankType.Diamond1
        };

        RankCondition MasterCond = new RankCondition
        {
            level = GameLevel.Master,
            monsterThreshold0 = 0,
            monsterThreshold50 = 50,
            rankIfZero = RankType.Challenger,
            rankIfBelow50 = RankType.GrandMaster,
            rankIfOther = RankType.Master
        };

        rankConditionDic.Add(GameLevel.Bronze,bronzeCond);
        rankConditionDic.Add(GameLevel.Silver, SilverCond);
        rankConditionDic.Add(GameLevel.Gold, GoldCond);
        rankConditionDic.Add(GameLevel.Platinum, PlatinumCond);
        rankConditionDic.Add(GameLevel.Diamond, DiamondCond);
        rankConditionDic.Add(GameLevel.Master, MasterCond);
    }

    private void Start()
    {
        Gold += 12;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            ChanageState(GameState.GameWinner);
        }
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

    public void X2Button()
    {
        isX2 = !isX2;

        if(isX2)
        {
            Time.timeScale = 2;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void RankReset(RankType rankTypeA, RankType rankTypeB)
    {
        int next = (int)rankTypeA;
        int cur = (int)rankTypeB;
        rating.curRank = rankDic[curRank];
        rating.value = next - cur;
        rating.isRank = true;
        rating.rankObj.GetComponent<SpriteRenderer>().sprite = rankDic[curRank].rankImage;
        mainUi.SetActive(false);
        rating.rankObj.SetActive(true);
        rating.gameObject.SetActive(true);
    }

    public void EvaluateRank(RankCondition condition)
    {
        int monsterCount = MonsterSpawner.instance.MonsterCount;
        RankType targetRank;

        if (monsterCount == condition.monsterThreshold0)
            targetRank = condition.rankIfZero;
        else if (monsterCount <= condition.monsterThreshold50)
            targetRank = condition.rankIfBelow50;
        else
            targetRank = condition.rankIfOther;

        if (curRank < targetRank)
        {
            RankReset(targetRank, curRank);
        }
    }

    public void ChanageState(GameState state)
    {
        gameState = state;
        stateMachine.ChangeState(state);
    }
}
