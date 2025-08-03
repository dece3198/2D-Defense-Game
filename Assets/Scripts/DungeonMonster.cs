using System.Collections;
using UnityEngine;

public enum MonsterRating
{
    Normal, Rare, Epic, Unique, Legendary
}

public class DungeonMonster : Monster, IInteractable
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
    public DungeonMonster nextMonster;
    public SPUM_Prefabs sPUM_Prefabs;
    public Rigidbody2D target;
    private Rigidbody2D rigid;

    private bool isKnockback = true;
    private bool isDie = true;

    private void Awake()
    {
        maxHp = Hp;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.rigid;
    }

    private void FixedUpdate()
    {
        if (!isDie)
        {
            rigid.linearVelocity = Vector2.zero;
            rigid.MovePosition(rigid.position);
            return;
        }

        if(isKnockback)
        {
            Vector2 dirVec = target.position - rigid.position;
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<OrbitingObject>() != null)
        {
            if(isDie)
            {
                StartCoroutine(KnockBackCo());
                Vector2 knockbackDir = (rigid.position - target.position).normalized;
                rigid.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
                Hp -= collision.GetComponent<OrbitingObject>().damage;
                textManager.ShowDamageText(collision.GetComponent<OrbitingObject>().damage);
                StartCoroutine(HitCo());
            }
        }
    }


    private void LateUpdate()
    {
        animator.SetBool("1_Move", isDie);
        float direction = target.position.x < rigid.position.x ? 1f : -1f;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
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

    public void StartMonster()
    {
        animator.SetBool("isDeath", false);
        rubyCount = 0;
        gameObject.layer = 7;
        Hp = maxHp;
    }

    private IEnumerator KnockBackCo()
    {
        isKnockback = false;
        rigid.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        isKnockback = true;
    }

    private IEnumerator DieCo()
    {
        isDie = false;
        rigid.linearVelocity = Vector2.zero;
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
        isDie = true;
        rubyCount = 0;
        gameObject.layer = 7;
        Hp = maxHp;
    }
}
