using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : Singleton<FadeInOut>
{
    private Image fadeImage;
    private float F_time = 1;

    private new void Awake()
    {
        base.Awake();
        fadeImage = GetComponent<Image>();
    }

    public void Fade(GameObject openObj)
    {
        StartCoroutine(FadeCo(openObj));
    }

    private IEnumerator FadeCo(GameObject openobj)
    {
        float time = 0;
        Color alpha = fadeImage.color;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            fadeImage.color = alpha;
            yield return null;
        }

        time = 0;
        openobj.SetActive(true);
        yield return new WaitForSeconds(1f);

        while (alpha.a > 0)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }
    }
}
