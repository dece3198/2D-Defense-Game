using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterRating
{
    Normal, Rare, Epic, Unique, Legendary
}

public class IdleMonster : Monster, IInteractable
{
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            hpBar.value = Mathf.Clamp01(hp / maxHp);
            if (hp <= 0)
            {
                StartCoroutine(DieCo());
            }
        }
    }
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private int diaCount;
    [SerializeField] private int rubyCount;
    [SerializeField] private int diaValue;
    [SerializeField] private int rubyValue;
    [SerializeField] private MonsterRating monsterRating;
    public IdleMonster nextMonster;
    public SPUM_Prefabs sPUM_Prefabs;

    private void Awake()
    {
        maxHp = Hp;
        animator = GetComponent<Animator>();
    }

    public void TakeHit(float damage, UnitRecipe unitRecipe, float stun)
    {
        if(Hp > 0)
        {
            Hp -= damage;
            textManager.ShowDamageText(damage);
            StartCoroutine(HitCo());
        }
    }

    private IEnumerator HitCo()
    {
        foreach(var r in spriteRenderers)
        {
            r.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var r in spriteRenderers)
        {
            r.color = Color.white;
        }
    }

    private IEnumerator DieCo()
    {
        gameObject.layer = 0;
        textManager.StopAllTexts();
        animator.SetBool("isDeath",true);
        yield return new WaitForSeconds(1f);
        if(Random.value < 0.01)
        {
            rubyCount = MonsterSpawner.instance.ratingDic[monsterRating];
        }
        WaitingRoom.instance.ExitCoin(diaCount, rubyCount, diaValue, rubyValue);
        yield return new WaitForSeconds(1f);
        animator.SetBool("isDeath", false);
        yield return new WaitForSeconds(0.5f);
        
        rubyCount = 0;
        gameObject.layer = 7;
        Hp = maxHp;
    }
}
