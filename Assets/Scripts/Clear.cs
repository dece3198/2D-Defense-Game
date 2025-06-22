using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class Clear : MonoBehaviour
{
    [SerializeField] private float alphaSpeed;
    [SerializeField] private SpriteRenderer clearText;
    [SerializeField] private GameObject statistics;
    [SerializeField] private TextMeshProUGUI statisticsText;
    [SerializeField] private GameObject nextButton;
    private SpriteRenderer clear;
    Color alphaA;
    Color alphaB;
    Vector3 originScaleA;
    Vector3 originScaleB;

    private void Awake()
    {
        clear = GetComponent<SpriteRenderer>();
        alphaA = clear.color;
        alphaB = clearText.color;
        originScaleA = transform.localScale;
        originScaleB = clearText.transform.localScale;
    }

    public void GameClear()
    {
        StartCoroutine(ClearCo());
    }

    public void NextButton()
    {
        nextButton.SetActive(false);
        statistics.SetActive(false);
        GameManager.instance.EvaluateRank(GameManager.instance.rankConditionDic[GameManager.instance.curGameLevel]);
    }

    private IEnumerator ClearCo()
    {
        float time = 0f;
        transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.5f).SetEase(Ease.InOutQuad).
            OnComplete(() =>
            {
                transform.DOScale(new Vector3(2f, 2f, 1), 0.25f).SetEase(Ease.InOutSine);
            });
        while(time < 1)
        {
            time += Time.deltaTime;
            alphaA.a = Mathf.Lerp(alphaA.a, 1, Time.deltaTime * alphaSpeed);
            clear.color = alphaA;
            yield return null;
        }

        clearText.gameObject.SetActive(true);

        time = 0;
        clearText.transform.DOScale(new Vector3(0.75f, 0.75f, 1), 0.5f).SetEase(Ease.InOutQuad);
        while (time < 1)
        {
            time += Time.deltaTime;
            alphaB.a = Mathf.Lerp(alphaB.a, 1, Time.deltaTime * alphaSpeed);
            clearText.color = alphaB;
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        alphaA.a = 0;
        clear.color = alphaA;
        clearText.color = alphaA;
        transform.localScale = originScaleA;
        clearText.transform.localScale = originScaleB;
        statistics.SetActive(true);

        string _clearText = "남은몬스터 : " + MonsterSpawner.instance.MonsterCount.ToString() + "마리" + "\n" + "\n"
            + "예상티어 : 브론즈3....";

        for(int i = 0; i < _clearText.Length;i++)
        {
            statisticsText.text = _clearText.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }

        nextButton.SetActive(true);
    }
}
