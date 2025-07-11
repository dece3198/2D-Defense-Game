using System.Collections;
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
    public SPUM_Prefabs sPUM_Prefabs;

    private void Awake()
    {
        viewDetector = GetComponent<ViewDetector>();
        sPUM_Prefabs = transform.parent.GetComponent<SPUM_Prefabs>();
        sPUM_Prefabs.OverrideControllerInit();
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

    private IEnumerator AtkCo()
    {
        isAtk = false;
        sPUM_Prefabs.PlayAnimation(PlayerState.ATTACK, 0);
        minAtk = unitRecipe.minAtk + (unitRecipe.minAtk * (UpGradeManager.instance.atkUp.level *  0.05f));
        maxAtk = unitRecipe.maxAtk + (unitRecipe.maxAtk * (UpGradeManager.instance.atkUp.level * 0.05f));
        float randDamage = Random.Range(minAtk, maxAtk);
        viewDetector.Target.GetComponentInChildren<IInteractable>().TakeHit(randDamage, unitRecipe, 0);
        float atkSpeed = unitRecipe.atkCoolTime - (unitRecipe.atkCoolTime * (UpGradeManager.instance.atkSpeedUp.level * 0.005f));
        float skillPercent = unitRecipe.skillPercent + (unitRecipe.skillPercent * (UpGradeManager.instance.skillPercent.level * 0.01f));
        if(skillAni != null && Random.value < skillPercent)
        {

        }
        yield return new WaitForSeconds(atkSpeed);
        isAtk = true;
    }
}
