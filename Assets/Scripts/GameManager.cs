using System;
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
    Bronze, Silver, Gold, Platinum, Diamond, Master, GrandMaster, Challenger
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
        game.EndGame();
        game.fail.SetActive(true);
        game.StartCoroutine(FailCo(game));
    }

    public override void Exit(GameManager game)
    {
    }

    public override void Update(GameManager game)
    {
    }

    private IEnumerator FailCo(GameManager game)
    {
        float time = 2f;
        Color alpha = game.failText.color;
        while (time > 0)
        {
            time -= Time.deltaTime;
            alpha.a = Mathf.Lerp(alpha.a, 1f, Time.deltaTime);
            game.failText.color = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        FadeInOut.instance.Fade(game.waitRoom);
        yield return new WaitForSeconds(1f);
        alpha.a = 0;
        game.failText.color = alpha;
        game.fail.SetActive(false);
        game.mainUi.SetActive(false);
    }
}

public class GameWinner : BaseState<GameManager>
{
    public override void Enter(GameManager game)
    {
        game.EndGame();
        game.mainUi.SetActive(false);
        Clear.instance.GameClear();
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
            defDeBuffText.text = defDeBuff.ToString() + "\n" + (defDeBuff * 0.2f).ToString("N1") + "%";
        }
    }
    [SerializeField] private int ruby;
    public int Ruby
    {
        get { return ruby; }
        set
        {
            ruby = value;
            rubyText.text = ruby.ToString();
        }
    }
    [SerializeField] private int dia;
    public int Dia
    {
        get { return dia; }
        set
        {
            dia = value;
            diaText.text = dia.ToString();
        }
    }
    public int stage = 0;
    public int missionFail = 0;

    public Rating rating;
    public Rank[] ranks;
    public GameObject grid;
    public GameObject[] lockImage;
    public GameObject fail;
    public GameObject mainUi;
    public GameObject waitRoom;
    public Transform[] targetPos;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI failText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI jamText;
    [SerializeField] private TextMeshProUGUI defDeBuffText;
    [SerializeField] private TextMeshProUGUI rubyText;
    [SerializeField] private TextMeshProUGUI diaText;
    public RankType curRank;
    public GameLevel curGameLevel;
    public GameState gameState;
    public StateMachine<GameState, GameManager> stateMachine = new StateMachine<GameState, GameManager>();
    private Dictionary<UnitRecipe, int> defUnitCount = new Dictionary<UnitRecipe, int>();
    public Dictionary<RankType, Rank> rankDic = new Dictionary<RankType, Rank>();
    public Dictionary<GameLevel, RankCondition> rankConditionDic = new Dictionary<GameLevel, RankCondition>();
    public Dictionary<GameLevel, float> defDamageDic = new Dictionary<GameLevel, float>();
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
        defDamageDic.Add(GameLevel.Bronze, 0.04f);
        defDamageDic.Add(GameLevel.Silver, 0.035f);
        defDamageDic.Add(GameLevel.Gold, 0.03f);
        defDamageDic.Add(GameLevel.Platinum, 0.015f);
        defDamageDic.Add(GameLevel.Diamond, 0.01f);
        defDamageDic.Add(GameLevel.Master, 0.005f);
        defDamageDic.Add(GameLevel.GrandMaster, 0.0025f);
        defDamageDic.Add(GameLevel.Challenger, 0.001f);
        ChanageState(GameState.None);

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
            ChanageState(GameState.GameEnd);
        }
    }

    public void SaveData()
    {

    }

    public void LoadData()
    {
        if(Enum.IsDefined(typeof(RankType), DataManager.instance.curData.rank))
        {
            curRank = (RankType)DataManager.instance.curData.rank;
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

    public void EndGame()
    {
        MonsterSpawner.instance.StopStage();
        MonsterSpawner.instance.EndGame();
        UnitSpawner.instance.EndGame();
        UiManager.instance.EndGame();
        stage = 0;
        Gold = 12;
        Jam = 0;
    }

    public void ChanageState(GameState state)
    {
        gameState = state;
        stateMachine.ChangeState(state);
    }

    public void CheckTime()
    {
        StartCoroutine(CheckTimeCo());
    }

    private IEnumerator CheckTimeCo()
    {
        var curData = DataManager.instance.curData;
        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime lastCheckTime = Convert.ToDateTime(curData.lastCheckTimeString);
            DateTime todayReseTime = now.Date;
            DateTime nextDay = todayReseTime.AddDays(1);

            if (lastCheckTime < todayReseTime)
            {
                DataManager.instance.curData.isTimeCompensation = true;
                SaveData();
            }

            if (curData.isTimeCompensation)
            {
                //첫 보상 또는 12시가 지났을 때
                StoreManager.instance.NewDay();
            }

            //다음날 - 현재시간 = 다음날까지 남은시간
            TimeSpan timeUntilReset = nextDay - DateTime.Now; 
            yield return new WaitForSecondsRealtime((float)timeUntilReset.TotalSeconds);
            curData.isTimeCompensation = true;
            SaveData();

        }
    }
}
