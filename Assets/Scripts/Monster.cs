using UnityEngine;
using UnityEngine.UI;

public enum MonsterState
{
    Idle, Walk, Hit, Stun, Die
}

public enum MonsterType
{
    Normal, Fast, VeryFast, Boss, Mission
}

public abstract class BaseState<T>
{
    public abstract void Enter(T monster);
    public abstract void Exit(T monster);
    public abstract void Update(T monster);
}

public class Monster : MonoBehaviour
{
    public Sprite monsterImage;
    public int stage;
    public float def;
    public float maxHp;
    public float speed;
    public int jam;
    public int posIndex;
    public Transform center;
    public Animator animator;
    public TextManager textManager;
    public Slider hpBar;
    public MonsterType monsterType;
}


