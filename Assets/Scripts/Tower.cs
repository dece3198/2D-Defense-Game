using UnityEngine;

public class Tower : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMouseDown()
    {
        GameManager.instance.grid.SetActive(true);
    }
}
