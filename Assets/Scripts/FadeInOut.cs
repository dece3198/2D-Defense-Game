using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    private Image fadeImage;
    private float F_time = 1;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public void Fade()
    {
        StartCoroutine(FadeCo());
    }

    private IEnumerator FadeCo()
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
