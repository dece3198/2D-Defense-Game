using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class WaitingRoom : Singleton<WaitingRoom>
{
    public GameButton curButton;    
    [SerializeField] private RectTransform level;
    [SerializeField] private float slotWidth = 500f;
    private int rouletteCount = 0;
    private bool isWait = true;

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

    private IEnumerator StartCo()
    {
        yield return new WaitForSeconds(1f);
        isWait = true;
        gameObject.SetActive(false);
        GameManager.instance.ChanageState(GameState.Wait);
        GameManager.instance.Gold = 12;
        GameManager.instance.Jam = 0;
    }
}
