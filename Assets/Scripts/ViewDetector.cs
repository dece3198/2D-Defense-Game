using UnityEngine;

public class ViewDetector : MonoBehaviour
{
    [SerializeField] private GameObject target;
    public GameObject Target { get { return target; } }
    public Transform center;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center.position, radius);
    }
}
