using UnityEngine;

public enum MonsterState
{
    Idle, Walk, Attack, Stun, Die
}

public abstract class BaseState<T>
{
    public abstract void Enter(T monster);
    public abstract void Exit(T monster);
    public abstract void Update(T monster);
}

public class Monster : MonoBehaviour
{
    public State state;
    public float atk;
    public float def;
    public float hpReg;
    public float speed;

    public Animator animator;
}


