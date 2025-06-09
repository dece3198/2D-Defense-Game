using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
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
    [SerializeField] private GameObject monsterState;
    [SerializeField] private Image monsterImage;
    [SerializeField] private TextMeshProUGUI monsterHp;
    [SerializeField] private TextMeshProUGUI monsterDef;
    [SerializeField] private TextMeshProUGUI monsterSpeed;
    private bool isState = false;

    private new void Awake()
    {
        base.Awake();
        speedDic.Add(MonsterType.Normal, 1f);
        speedDic.Add(MonsterType.Fast, 1.5f);
        speedDic.Add(MonsterType.VeryFast, 2f);
        speedDic.Add(MonsterType.Boss, 0.5f);
        speedDic.Add(MonsterType.Mission, 0.5f);
    }

    public void StartStage()
    {
        StartCoroutine(MonsterCo());
    }


    private IEnumerator MonsterCo()
    {
        if(((GameManager.instance.stage + 1) % 10) == 0 && GameManager.instance.stage != 0)
        {
            GameObject monster = GetFormPool(GameManager.instance.stage);
            monster.transform.position = transform.position;
            monster.SetActive(true);
        }
        else
        {
            for (int i = 0; i < 40; i++)
            {
                GameObject monster = GetFormPool(GameManager.instance.stage);
                monster.transform.position = transform.position;
                monster.SetActive(true);
                yield return new WaitForSeconds(1f);
            }
        }
        GameManager.instance.stage++;
    }

    public GameObject GetFormPool(int stage)
    {
        if(monsterPool.ContainsKey(stage))
        {
            foreach(var monster in monsterPool[stage])
            {
                if(!monster.activeInHierarchy && monster.GetComponentInChildren<BasicMonster>().stage == monsters[stage].GetComponentInChildren<BasicMonster>().stage)
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
        monster.SetActive(false);
        monsterPool[stage].Add(monster);
    }

    public void DeleteMonster()
    {
        StartCoroutine(DestoryMonsterCo());
    }

    private IEnumerator DestoryMonsterCo()
    {
        for (int i = monsterPool.Count - 1; i >= 0; i--)
        {
            if (GameManager.instance.stage != i)
            {
                if (monsterPool.ContainsKey(i))
                {
                    for (int j = monsterPool[i].Count - 1; j >= 0; j--)
                    {
                        if (!monsterPool[i][j].activeInHierarchy)
                        {
                            Destroy(monsterPool[i][j]);
                            monsterPool[i].RemoveAt(j);
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }
            }
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
            monsterHp.text = (200 * Mathf.Pow(1.127745f, GameManager.instance.stage)).ToString("N0");
            monsterDef.text = monster.def.ToString("N1");
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
