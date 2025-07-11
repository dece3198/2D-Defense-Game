using DG.Tweening;
using UnityEngine;

public enum CoinType
{
    Dia, Ruby
}

public class MoveCoin : MonoBehaviour
{
    public Transform target;
    public Transform startPos;
    public CoinType coinType;

    private void OnEnable()
    {
        if (target != null)
        {
            if (startPos != null)
            {
                ExplosionCoin(startPos.position, target.position, 250);
            }
        }
    }

    public void ExplosionCoin(Vector2 from, Vector2 _target, float range)
    {
        transform.position = from;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(from + Random.insideUnitCircle * range, 0.5f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMove(_target, 1f).SetEase(Ease.OutCubic));
        sequence.AppendCallback(() => { WaitingRoom.instance.EnterCoin(gameObject); });
    }
}
