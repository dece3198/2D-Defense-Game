using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoom : Singleton<WaitingRoom>
{
    public GameButton curButton;    
    [SerializeField] private RectTransform level;
    [SerializeField] private float slotWidth = 500f;
    [SerializeField] private GameObject dungeonMap;
    [SerializeField] private GameObject defenseMap;
    [SerializeField] private GameObject coinParent;
    [SerializeField] private Sprite dia;
    [SerializeField] private Sprite ruby;
    [SerializeField] private Transform diaTargetPos;
    [SerializeField] private Transform rubyTargetPos;
    [SerializeField] private Transform startPos;
    [SerializeField] private GameObject coin;
    private Stack<GameObject> coinStack = new Stack<GameObject>();
    private int rouletteCount = 0;
    private bool isWait = true;

    private void Start()
    {
        for(int i = 0; i < 15; i++)
        {
            GameObject _coin = Instantiate(coin, coinParent.transform);
            coinStack.Push(_coin);
        }
    }

    public void RightButton()
    {
        if (rouletteCount < level.transform.childCount - 1)
        {
            rouletteCount++;
            MoveToIndec(rouletteCount);
        }
    }

    public void LeftButton()
    {
        if (rouletteCount > 0)
        {
            rouletteCount--;
            MoveToIndec(rouletteCount);
        }
    }

    private void MoveToIndec(int index)
    {
        Vector2 targetPos = new Vector2(-slotWidth * index, level.anchoredPosition.y);
        level.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutQuart);
    }

    public void StartGame()
    {
        if(isWait)
        {
            if (Enum.IsDefined(typeof(GameLevel), rouletteCount))
            {
                GameManager.instance.curGameLevel = (GameLevel)rouletteCount;
            }
            FadeInOut.instance.Fade(GameManager.instance.mainUi);
            StartCoroutine(StartCo());
            isWait = false;
        }
    }

    public void EnterCoin(GameObject _coin)
    {
        coinStack.Push(_coin);
        _coin.SetActive(false);
        if(_coin.GetComponent<MoveCoin>().coinType == CoinType.Dia)
        {
            GameManager.instance.Dia++;
        }
        else
        {
            GameManager.instance.Ruby++;
        }
    }

    public void ExitCoin(int diaCount, int rubyCount)
    {
        if(diaCount != 0)
        {
            SettingCoin(diaCount, dia, diaTargetPos, CoinType.Dia);
        }

        if(rubyCount != 0)
        {
            SettingCoin(rubyCount, ruby, rubyTargetPos, CoinType.Ruby);
        }
    }

    private void SettingCoin(int count, Sprite image, Transform pos, CoinType coinType)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject coinObj;

            if(coinStack.Count > 0)
            {
                coinObj = coinStack.Pop();
            }
            else
            {
                coinObj = Instantiate(coin, coinParent.transform);
            }

            MoveCoin moveCoin = coinObj.GetComponent<MoveCoin>();
            moveCoin.target = pos;
            moveCoin.startPos = startPos;
            moveCoin.GetComponent<Image>().sprite = image;
            moveCoin.coinType = coinType;
            moveCoin.gameObject.SetActive(true);
        }
    }

    private IEnumerator StartCo()
    {
        yield return new WaitForSeconds(1f);
        isWait = true;
        GameManager.instance.ChanageState(GameState.Wait);
        GameManager.instance.Gold = 12;
        GameManager.instance.Jam = 0;
        dungeonMap.SetActive(false);
        defenseMap.SetActive(true);
        gameObject.SetActive(false);
    }
}
