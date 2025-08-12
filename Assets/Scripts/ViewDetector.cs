using System.Collections.Generic;
using UnityEngine;

public class ViewDetector : MonoBehaviour
{
    private Unit unit;
    public Animator animator;
    [SerializeField] private GameObject target;
    public GameObject Target { get { return target; } }
    public Transform center;
    [SerializeField] private Transform player;
    [SerializeField] private float radius;
    [SerializeField] private float angle;
    [SerializeField] private float debuffRadius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask buffLayerMask;
    private List<BasicMonster> monsterList = new List<BasicMonster>();
    private List<Unit> unitList = new List<Unit>();
    private List<Unit> buffUnits = new List<Unit>();

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

            if (distance < min)
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

    public void FindRangeTarget(float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, radius, layerMask);

        foreach (Collider2D collider2D in targets)
        {
            collider2D.GetComponent<IInteractable>().DungeonTakeHit(damage);
        }

        if (targets.Length > 0)
        {
            target = targets[0].gameObject;
        }
        else
        {
            targets = null;
        }
    }

    public void FindAngleTarget(float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, radius, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector2 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.right, findTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            float findTargetRange = Vector2.Distance(transform.position, targets[i].transform.position);
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            targets[i].GetComponent<IInteractable>().DungeonTakeHit(damage);

            target = targets[i].gameObject;
        }
        target = null;
    }

    public void FindSkillTarget(float atk,UnitRecipe unitRecipe , float stun)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, radius, layerMask);

        for(int i = 0; i < targets.Length; i++)
        {
            targets[i].GetComponent<BasicMonster>().DefenseTakeHit(atk, unitRecipe, stun);
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

    /*주변 Unit을 찾아 공격력 버프를 주는 코드인데 HashSet를 사용하여 업데이트 문에서 실행하더라도
     한번만 실행됨 그래서 버프를 제거를 할때도 간단히 제거를 할 수있음
      */
    public void FindBufferTarget(UnitRecipe unitRecipe, float buff)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, debuffRadius, buffLayerMask);
        HashSet<Unit> curUnit = new();

        foreach(var col in targets)
        {
            Unit unit = col.GetComponentInChildren<Unit>();
            if(unit != null)
            {
                curUnit.Add(unit);
                if(!unitList.Contains(unit))
                {
                    unit.SetBuff(unitRecipe, buff);
                    unitList.Add(unit);
                }
            }
        }

        for(int i = unitList.Count -1; i >= 0; i--)
        {
            if (!curUnit.Contains(unitList[i]))
            {
                unitList[i].ResetBuff(unitRecipe, buff);
                unitList.RemoveAt(i);
            }
        }
    }

    public void UnitSetBuff(UnitRecipe unitRecipe, float buff)
    {
        buffUnits.Clear();

        Collider2D[] targets = Physics2D.OverlapCircleAll(center.position, debuffRadius, buffLayerMask);

        foreach (var col in targets)
        {
            Unit targetUnit = col.GetComponentInChildren<Unit>();
            if (targetUnit != null)
            {
                targetUnit.SetBuff(unitRecipe, buff);
                buffUnits.Add(targetUnit);
            }
        }
    }

    public void UnitResetBuff(UnitRecipe unitRecipe, float buff)
    {
        foreach (var unit in buffUnits)
        {
            if (unit != null)
            {
                unit.ResetBuff(unitRecipe, buff);
            }
        }

        buffUnits.Clear();
    }

    public void RemoveAllBuffsGiven()
    {
        foreach (var u in unitList)
        {
            if (u != null)
            {
                u.ResetBuff(unit.unitRecipe, unit.buff);
            }
        }
        unitList.Clear();
    }

    public void RemoveAllSpeedDebuff()
    {
        foreach (var m in monsterList)
        {
            if (m != null)
            {
                m.ResetSpeed(unit.unitRecipe);
            }
        }
        monsterList.Clear();
    }

    public void ClearTarget()
    {
        target = null;
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


        Vector2 rightDir = transform.right;
        float halfRad = angle * 0.5f * Mathf.Deg2Rad;
        Vector2 rightBoundary = Quaternion.Euler(0,0, angle * 0.5f) * rightDir;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, -angle * 0.5f) * rightDir;

        Debug.DrawRay(transform.position, rightDir * radius, Color.yellow);
        Debug.DrawRay(transform.position, rightBoundary * radius, Color.green);
        Debug.DrawRay(transform.position, leftBoundary * radius, Color.green);
    }
}
