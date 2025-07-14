using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleUnit : MonoBehaviour
{
    public UnitRecipe unitRecipe;
    [SerializeField] private ViewDetector viewDetector;
    public IdleUnit nextUnit;
    [SerializeField] private float minAtk;
    [SerializeField] private float maxAtk;
    [SerializeField] private bool isAtk = true;
    [SerializeField] private Animator skillAni;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPos;
    [SerializeField] private Stack<GameObject> bulletStack = new Stack<GameObject>();
    public SPUM_Prefabs sPUM_Prefabs;

    private void Awake()
    {
        viewDetector = GetComponent<ViewDetector>();
        sPUM_Prefabs = transform.parent.GetComponent<SPUM_Prefabs>();
        sPUM_Prefabs.OverrideControllerInit();
    }

    private void Start()
    {
        if(unitRecipe.unitAtkType == UnitAtkType.AD)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject _bullet = Instantiate(bullet, sPUM_Prefabs.transform);
                _bullet.GetComponent<IdleBullet>().unit = this;
                bulletStack.Push(_bullet);
            }
        }
    }

    private void Update()
    {
        viewDetector.FindTarget();
        if(viewDetector.Target != null)
        {
            if(isAtk)
            {
                StartCoroutine(AtkCo());
            }
        }
    }

    public void EnterPool(GameObject _bullet)
    {
        _bullet.SetActive(false);
        _bullet.transform.position = transform.position;
        bulletStack.Push(_bullet);
    }

    private void ExitPool()
    {
        GameObject _bullet = bulletStack.Pop();
        _bullet.transform.position = bulletPos.position;
        _bullet.GetComponent<IdleBullet>().target = viewDetector.Target;
        _bullet.SetActive(true);
    }

    private IEnumerator AtkCo()
    {
        isAtk = false;
        sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        float unitMinAtk = unitRecipe.minAtk + (unitRecipe.minAtk * InventoryManager.instance.itemAtk * 0.01f);
        float unitMaxAtk = unitRecipe.maxAtk + (unitRecipe.maxAtk * InventoryManager.instance.itemAtk * 0.01f);
        minAtk = unitMinAtk + (unitMinAtk * (UpGradeManager.instance.atkUp.level *  0.05f));
        maxAtk = unitMaxAtk + (unitMaxAtk * (UpGradeManager.instance.atkUp.level * 0.05f));
        float randDamage = Random.Range(minAtk, maxAtk);
        if(unitRecipe.unitAtkType == UnitAtkType.AD)
        {
            if(transform.CompareTag("Archer"))
            {
                yield return new WaitForSeconds(0.5f);
            }
            ExitPool();
        }
        else
        {
            viewDetector.Target.GetComponentInChildren<IInteractable>().TakeHit(randDamage, unitRecipe, 0);
        }
        float atkSpeed = unitRecipe.atkCoolTime - (unitRecipe.atkCoolTime * (UpGradeManager.instance.atkSpeedUp.level * 0.005f));
        float skillPercent = unitRecipe.skillPercent + (unitRecipe.skillPercent * (UpGradeManager.instance.skillPercent.level * 0.01f));
        if(skillAni != null && Random.value < skillPercent)
        {
            skillAni.SetTrigger("Skill");
            skillAni.gameObject.GetComponent<ViewDetector>().FindTarget();
            float skillDamage = unitRecipe.skillDamage + (unitRecipe.skillDamage * (UpGradeManager.instance.skillDamage.level * 0.05f));
            float finalDamage = ((minAtk + maxAtk) * 0.5f) * skillDamage;
            if (skillAni.gameObject.GetComponent<ViewDetector>().Target != null)
            {
                skillAni.gameObject.GetComponent<ViewDetector>().Target.GetComponent<IInteractable>().TakeHit(finalDamage, unitRecipe, 0);
            }
        }
        yield return new WaitForSeconds(atkSpeed);
        isAtk = true;
    }
}
