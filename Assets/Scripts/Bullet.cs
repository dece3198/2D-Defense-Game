using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    public Unit unit;
    public GameObject target;

    private void Update()
    {
        if(target == null)
        {
            unit.EnterPool(gameObject);
            return;
        }

        Vector2 direction = (target.GetComponentInChildren<Monster>().center.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponentInChildren<IInteractable>() != null)
        {
            if(target != null)
            {
                if (collision.GetComponentInChildren<BasicMonster>().gameObject == target.gameObject)
                {
                    int rand = Random.Range(unit.unitRecipe.minAtk, unit.unitRecipe.maxAtk);
                    collision.GetComponentInChildren<IInteractable>().TakeHit(rand, unit.unitRecipe.unitType, unit.unitRecipe.stun);
                    target = null;
                    unit.EnterPool(gameObject);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
