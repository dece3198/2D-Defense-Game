using UnityEngine;

public enum MonsterState
{
    Idle, Walk, Hit, Stun, Die
}

public abstract class BaseState<T>
{
    public abstract void Enter(T monster);
    public abstract void Exit(T monster);
    public abstract void Update(T monster);
}

public class Monster : MonoBehaviour
{
    public float def;
    public float speed;
    public float maxHp;
    public Transform center;
    public Animator animator;
}


