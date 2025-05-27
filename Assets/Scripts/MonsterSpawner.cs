using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
    [SerializeField] private GameObject[] monsters;
    private Dictionary<int, List<GameObject>> monsterPool = new Dictionary<int, List<GameObject>>();
    

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
                yield return new WaitForSeconds(3f);
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
}
