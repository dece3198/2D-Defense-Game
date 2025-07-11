using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
    [SerializeField] private int monsterCount;
    public int MonsterCount
    {
        get { return monsterCount; }
        set
        {
            monsterCount = value;
            if (monsterCount >= 80)
            {
                if (monsterCount >= 100)
                {
                    monsterCount = 0;
                    GameManager.instance.ChanageState(GameState.GameEnd);
                }
            }

        }
    }
    [SerializeField] private GameObject[] monsters;
    [SerializeField] private GameObject[] missionMonsters;
    [SerializeField] private GameObject missionUi;
    public GameObject[] missionTimeUi;
    [SerializeField] private TextMeshProUGUI[] timeText;
    [SerializeField] private GameObject[] missionTime;
    [SerializeField] private TextMeshProUGUI[] missionTimeText;
    private bool isMission = false;
    private Dictionary<int, List<GameObject>> monsterPool = new Dictionary<int, List<GameObject>>();
    public Dictionary<MonsterType, float> speedDic = new Dictionary<MonsterType, float>();
    public Dictionary<MonsterRating, int> ratingDic = new Dictionary<MonsterRating, int>();
    [SerializeField] private GameObject monsterState;
    [SerializeField] private Image monsterImage;
    [SerializeField] private TextMeshProUGUI monsterHp;
    [SerializeField] private TextMeshProUGUI monsterDef;
    [SerializeField] private TextMeshProUGUI monsterSpeed;
    private bool isState = false;
    private IEnumerator monsterCo;


    private new void Awake()
    {
        base.Awake();
        speedDic.Add(MonsterType.Normal, 1f);
        speedDic.Add(MonsterType.Fast, 1.5f);
        speedDic.Add(MonsterType.VeryFast, 2f);
        speedDic.Add(MonsterType.Boss, 0.5f);
        speedDic.Add(MonsterType.Mission, 0.5f);

        ratingDic.Add(MonsterRating.Normal, 1);
        ratingDic.Add(MonsterRating.Rare, 2);
        ratingDic.Add(MonsterRating.Epic, 5);
        ratingDic.Add(MonsterRating.Unique, 10);
        ratingDic.Add(MonsterRating.Legendary, 100);
    }

    public void StartStage()
    {
        monsterCo = MonsterCo();
        StartCoroutine(monsterCo);
    }

    public void StopStage()
    {
        StopCoroutine(monsterCo);
    }


    private IEnumerator MonsterCo()
    {
        if(((GameManager.instance.stage + 1) % 10) == 0 && GameManager.instance.stage != 0)
        {
            GameObject monster = GetFormPool(GameManager.instance.stage);
            monster.transform.position = transform.position;
            monster.SetActive(true);
            MonsterCount++;
            yield return new WaitForSeconds(40);
        }
        else
        {
            for (int i = 0; i < 40; i++)
            {
                GameObject monster = GetFormPool(GameManager.instance.stage);
                monster.transform.position = transform.position;
                monster.SetActive(true);
                MonsterCount++;
                yield return new WaitForSeconds(1f);
            }
        }
        GameManager.instance.stage++;
    }

    public GameObject GetFormPool(int stage)
    {
        if (monsterPool.ContainsKey(stage))
        {
            foreach(var monster in monsterPool[stage])
            {
                if (monster == null) continue;

                if(!monster.activeInHierarchy &&
                    monster.GetComponentInChildren<BasicMonster>().stage ==
                    monsters[stage].GetComponentInChildren<BasicMonster>().stage)
                {
                    monster.SetActive(true);
                    return monster;
                }
            }
        }

        GameObject newMonster = Instantiate(monsters[stage], transform);
        EnterPool(newMonster, stage);
        return newMonster;
    }

    public void EnterPool(GameObject monster, int stage)
    {
        if (!monsterPool.ContainsKey(stage))
            monsterPool[stage] = new List<GameObject>();
        monster.transform.position = transform.position;
        monster.GetComponentInChildren<BasicMonster>().transform.position = transform.position;
        monster.GetComponentInChildren<BasicMonster>().posIndex = 0;
        monster.SetActive(false);
        monsterPool[stage].Add(monster);
    }

    //게임 종료시 활성화되있는 몬스터를 EnterPool로 집어넣음
    public void EndGame()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            GameObject monsterObject = child.gameObject;

            if (!monsterObject.activeInHierarchy) continue;

            BasicMonster monster = child.GetComponentInChildren<BasicMonster>();
            if (monster == null) continue;

            if(monster.monsterType == MonsterType.Mission)
            {
                monster.gameObject.SetActive(false);
            }
            EnterPool(monsterObject, monster.stage);
        }
    }

    public void DeleteMonster()
    {
        StartCoroutine(DestoryMonsterCo());
    }

    private IEnumerator DestoryMonsterCo()
    {
        int bfStage = GameManager.instance.stage - 1;

        if (bfStage < 0) yield break;

        if (monsterPool.Count > 0)
        {
            if (monsterPool.ContainsKey(bfStage))
            {
                for (int j = monsterPool[bfStage].Count - 1; j >= 0; j--)
                {
                    GameObject monster = monsterPool[bfStage][j];

                    if (monster == null || !monster.activeInHierarchy)
                    {
                        monsterPool[bfStage].RemoveAt(j);
                        if (monster != null)
                            Destroy(monster);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }
        CleanPool();
    }

    private void CleanPool()
    {
        foreach(var key in monsterPool.Keys.ToList())
        {
            monsterPool[key] = monsterPool[key].Where(m => m != null).ToList();
        }
    }

    public void MissionOnOf()
    {
        isMission = !isMission;
        if(isMission)
        {
            missionUi.SetActive(true);
        }
        else
        {
            missionUi.SetActive(false);
        }
    }

    public void Mission(int value)
    {
        missionMonsters[value].GetComponent<BasicMonster>().value = value;
        missionMonsters[value].SetActive(true);
        missionMonsters[value].transform.position = transform.position;
        StartCoroutine(TimeCo(value));
        StartCoroutine(MissionTimeCo(missionMonsters[value].GetComponent<BasicMonster>().missionCoolTime, value));
    }

    public void MonsterState()
    {
        isState = !isState;
        if(isState)
        {
            monsterState.SetActive(true);
            BasicMonster monster = monsters[GameManager.instance.stage].GetComponentInChildren<BasicMonster>();
            monsterImage.sprite = monster.monsterImage;
            monsterHp.text = ((200 * Mathf.Pow(1.127745f, GameManager.instance.stage)) * 0.5f).ToString("N0");
            monsterDef.text = (2 * (GameManager.instance.stage + 1)).ToString("N1");
            monsterSpeed.text = (speedDic[monster.monsterType]).ToString("N1");
        }
        else
        {
            monsterState.SetActive(false);
        }
    }
        

    private IEnumerator MissionTimeCo(float _time, int value)
    { 
        missionTime[value].SetActive(true);
        float time = _time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            missionTimeText[value].text = $"{minutes:00} : {seconds:00}";
            yield return null;
        }
        missionTime[value].SetActive(false);
    }

    private IEnumerator TimeCo(int value)
    {
        missionTimeUi[value].SetActive(true);
        float time = 90;

        while (time > 0)
        {
            time -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText[value].text = $"{minutes:00} : {seconds:00}";
            yield return null;
        }

        missionTimeUi[value].SetActive(false);
    }
}
