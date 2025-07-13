using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum RankNumber
{
    One, Two, Three
}

public class RankRating : Singleton<RankRating>
{
    public GameObject rankObj;
    [SerializeField] private GameObject[] pieces;
    [SerializeField] private Transform[] targets;
    [SerializeField] private Vector3[] centersA;
    [SerializeField] private Vector3[] centersB;
    [SerializeField] private Vector3[] centersC;
    [SerializeField] private GameObject rankLight;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private GameObject checkButton;
    private Dictionary<RankNumber, Vector3[]> centersDic = new Dictionary<RankNumber, Vector3[]>();
    public Rank curRank;
    public int value;
    public bool isRank = false;

    private new void Awake()
    {
        base.Awake();
        centersDic.Add(RankNumber.One, centersA);
        centersDic.Add(RankNumber.Two, centersB);
        centersDic.Add(RankNumber.Three, centersC);
    }

    public void OnMouseDown()
    {
        if(isRank)
        {
            RankUp(curRank, value);
            isRank = false;
        }
    }


    public void RankUp(Rank rank, int steps)
    {
        for(int i = 0; i <pieces.Length; i++)
        {
            pieces[i].GetComponent<SpriteRenderer>().sprite = rank.pieces[i];
        }
        StartCoroutine(GatherFragments(GetNextRank(rank, steps)));
    }

    private Rank GetNextRank(Rank rank, int steps)
    {
        Rank temp = rank;
        for(int i = 0; i < steps && temp.nextRank != null; i++)
        {
            temp = temp.nextRank;
        }
        return temp;
    }

    private IEnumerator GatherFragments(Rank rank)
    {
        transform.DOShakePosition(1f, new Vector3(0.2f, 0.2f, 0), 20, 90, false, true);
        yield return new WaitForSeconds(1f);
        rankObj.SetActive(false);

        for (int i = 0; i < pieces.Length; i++)
        {
            GameObject piece = pieces[i];
            piece.SetActive(true);
            piece.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            piece.transform.DOMove(targets[i].position, 0.25f).SetEase(Ease.InOutQuad);
        }
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < pieces.Length; i++)
        {
            int index = i;
            var piece = pieces[index];
            var target = centersDic[rank.rankNumber][index];
            Vector3 originalScales = piece.transform.localScale;
            piece.transform.DOShakePosition(0.1f, new Vector3(0.1f, 0.1f, 0), 20, 90, false, true);
            yield return new WaitForSeconds(0.1f);
            piece.transform.DOMove(target, 0.25f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    // 도착 시 살짝 커졌다가 돌아오기
                    piece.GetComponent<SpriteRenderer>().sprite = rank.pieces[index];
                    piece.transform.DOScale(originalScales * 1.2f, 0.1f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            piece.transform.DOScale(originalScales, 0.1f).SetEase(Ease.InOutSine);
                        });
                });

            yield return new WaitForSeconds(0.1f);
            piece.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        yield return new WaitForSeconds(0.3f);

        Vector3 finalScale = transform.localScale;
        transform.DOScale(finalScale * 1.4f, 0.1f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                transform.DOScale(finalScale * 0.9f, 0.1f).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        transform.DOScale(finalScale, 0.1f);
                    });
            });
        foreach(var p in pieces)
        {
            p.SetActive(false);
        }
        rankObj.GetComponent<SpriteRenderer>().sprite = rank.rankImage;
        rankObj.SetActive(true);

        float time = 1;
        Color alpha = rankLight.GetComponent<SpriteRenderer>().color;
        rankText.text = rank.rankName;
        while(time > 0)
        {
            time -= Time.deltaTime;
            alpha.a = Mathf.Lerp(alpha.a, 1f, Time.deltaTime * 2.5f);
            rankLight.GetComponent<SpriteRenderer>().color = alpha;
            rankText.color = alpha;
            yield return null;
        }
        checkButton.SetActive(true);
    }

    public void CheckButton()
    {
        StartCoroutine(CheckCo());
    }

    private IEnumerator CheckCo()
    {
        FadeInOut.instance.Fade(GameManager.instance.waitRoom);
        yield return new WaitForSeconds(1f);
        rankLight.SetActive(false);
        checkButton.SetActive(false);
        gameObject.SetActive(false);
        rankText.gameObject.SetActive(false);
    }

}
