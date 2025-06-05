using System.Collections.Generic;
using UnityEngine;

public class ViewDetector : MonoBehaviour
{
    private Unit unit;
    [SerializeField] private GameObject target;
    public GameObject Target { get { return target; } }
    public Transform center;
    [SerializeField] private float radius;
    [SerializeField] private float debuffRadius;
    [SerializeField] private LayerMask layerMask;
    private List<BasicMonster> monsterList = new List<BasicMonster>();
    private List<Unit> unitList = new List<Unit>();

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void FindTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, radius, layerMask);
        float min = Mathf.Infinity;

        foreach(Collider2D collider2D in targets)
        {
            float distance = Vector2.Distance(center.position, collider2D.transform.position);

            if(distance < min)
            {
                min = distance;
                target = collider2D.gameObject;
            }
        }

        if(targets.Length <= 0)
        {
            target = null;
        }
    }

    public void FindSkillTarget(float atk,UnitType unitType , float stun)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, radius, layerMask);

        for(int i = 0; i < targets.Length; i++)
        {
            targets[i].GetComponent<BasicMonster>().TakeHit(atk, unitType, stun);
        }

        target = null;
    }

    public void DebuffTarget(float deBuff)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, debuffRadius, layerMask);
        HashSet<BasicMonster> curMonster = new();

        foreach(var col in targets)
        {
            BasicMonster monster = col.GetComponent<BasicMonster>();
            if(monster != null)
            {
                curMonster.Add(monster);
                if(!monsterList.Contains(monster))
                {
                    monster.SetSpeed(deBuff, unit.unitRecipe);
                    monsterList.Add(monster);
                }
            }
        }

        for(int i = monsterList.Count -1; i >= 0; i--)
        {
            if (!curMonster.Contains(monsterList[i]))
            {
                monsterList[i].ResetSpeed(unit.unitRecipe);
                monsterList.RemoveAt(i);
            }
        }
    }

    public void FindBufferTarget(UnitRecipe unitRecipe)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, debuffRadius, layerMask);
        HashSet<Unit> curUnit = new();

        foreach(var col in targets)
        {
            Unit unit = col.GetComponentInChildren<Unit>();
            if(unit != null)
            {
                curUnit.Add(unit);
                if(!unitList.Contains(unit))
                {
                    unit.SetBuff(unitRecipe);
                    unitList.Add(unit);
                }
            }
        }

        for(int i = unitList.Count -1; i >= 0; i--)
        {
            if (!curUnit.Contains(unitList[i]))
            {
                unitList[i].ResetBuff(unitRecipe);
                unitList.RemoveAt(i);
            }
        }
    }

    private void OnDestroy()
    {
        foreach(var monster in monsterList)
        {
            if(monster != null)
            {
                monster.ResetSpeed(unit.unitRecipe);
            }
        }
        monsterList.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center.position, debuffRadius);
    }
}
