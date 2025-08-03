using UnityEngine;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GPGS_Manager : MonoBehaviour, IPointerClickHandler
{
    private string userID;
    private bool isLogIn = true;
    [SerializeField] private TextMeshProUGUI loginText;
    private string success = "연동 성공...";

    private void Start()
    {
        loginText.transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    public void GPGS_LogIn()
    {
        //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /*
    internal void ProcessAuthentication(SignInStatus status)
    {
        if(status == SignInStatus.Success)
        {
            //string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            //userID = PlayGamesPlatform.Instance.GetUserId();
            StartCoroutine(LogInCo());
        }
        else
        {
            //loginText.text = "연동 실패";
        }
    }
    */


    private IEnumerator LogInCo()
    {
        for(int i = 0; i < success.Length; i++)
        {
            loginText.text = success.Substring(0,i);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(3f);
        userID = "zzz";
        DataManager.instance.userID = userID;
        GameManager.instance.waitRoom.SetActive(true);
        DataManager.instance.LoadData();
        FadeInOut.instance.Fade();
        yield return new WaitForSeconds(1f);
        
        WaitingRoom.instance.dungeonMap.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(isLogIn)
            {
                //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
                
                //string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
                //userID = PlayGamesPlatform.Instance.GetUserId();
                StartCoroutine(LogInCo());
                
                isLogIn = false;
            }
        }
    }
}
