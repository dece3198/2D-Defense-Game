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
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Animator animator;
    [SerializeField] private TextManager textManager;
    private ViewDetector viewDetector;

    private void Awake()
    {
        viewDetector = GetComponent<ViewDetector>();
    }

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
        Hp -= damage;
        textManager.ShowDamageText(damage);
        viewDetector.FindTarget();
        if(viewDetector.Target != null)
        {
            viewDetector.Target.GetComponent<IInteractable>().DungeonTakeHit((damage * 0.5f));
        }
    }
}
