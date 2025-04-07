using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject grid;
    public GameObject curTower;
    public Transform[] targetPos;
    public int posIndex = 0;
    public bool isSelect = false;
    public int stage = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(isSelect)
        {
        }
    }
}
