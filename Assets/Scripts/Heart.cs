using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour, IInteractable
{
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set 
        { 
            hp = value;
            hpSlider.value = Mathf.Clamp01(hp / maxHp);
        }
    }
    private float maxHp;
    public float def;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Animator animator;
    [SerializeField] private TextManager textManager;

    private void Start()
    {
        maxHp = Hp;
    }

    public void DefenseTakeHit(float damage, UnitRecipe unitRecipe, float stun)
    {
    }

    public void DungeonTakeHit(float damage)
    {
        animator.SetTrigger("Hit");
        float multiplier = 1f / (1f + def * 0.015f);
        float finalDamage = damage * multiplier;
        Hp -= finalDamage;
        textManager.ShowDamageText(finalDamage);
    }
}
