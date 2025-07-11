using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterRating
{
    Normal, Rare, Epic, Unique, Legendary
}

public class IdleMonster : MonoBehaviour, IInteractable
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
    [SerializeField] private float maxHp;
    [SerializeField] private Slider hpBar;
    [SerializeField] private int def;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private TextManager textManager;
    [SerializeField] private int diaCount;
    [SerializeField] private int rubyCount;
    [SerializeField] private MonsterRating monsterRating;
    public IdleMonster nextMonster;
    public SPUM_Prefabs sPUM_Prefabs;
    private Animator animator;

    private void Awake()
    {
        maxHp = Hp;
        animator = GetComponent<Animator>();
    }

    public void TakeHit(float damage, UnitRecipe unitRecipe, float stun)
    {
        if(Hp > 0)
        {
            float multiplier = 1f / (1f + def * 0.015f);
            float finalDamage = damage * multiplier;
            Hp -= finalDamage;
            textManager.ShowDamageText(finalDamage);
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
        animator.SetBool("isDeath",true);
        yield return new WaitForSeconds(1f);
        if(Random.value < 0.01)
        {
            rubyCount = MonsterSpawner.instance.ratingDic[monsterRating];
        }
        WaitingRoom.instance.ExitCoin(diaCount, rubyCount);
        yield return new WaitForSeconds(1f);
        animator.SetBool("isDeath", false);
        rubyCount = 0;
        gameObject.layer = 7;
        Hp = maxHp;
    }
}
