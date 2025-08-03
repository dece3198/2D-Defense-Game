using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 inputVec;
    public Rigidbody2D rigid;
    private Animator animator;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        animator.SetBool("1_Move", inputVec != Vector2.zero);

        if (inputVec.x != 0)
        {
            float direction = inputVec.x > 0 ? 1f : -1f;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
    }
}
