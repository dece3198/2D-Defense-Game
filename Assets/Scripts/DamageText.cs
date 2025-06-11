using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextManager textManager;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float alphaSpeed;
    [SerializeField] private TextMeshProUGUI text;
    Color alpha;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        alpha = text.color;
    }

    private void OnEnable()
    {
        alpha.a = 1;
        text.color = alpha;
        StartCoroutine(TextCo());
    }

    private IEnumerator TextCo()
    {
        float time = 1f;

        while (time > 0)
        {
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }

            time -= Time.deltaTime;
            text.fontSize += 0.005f;
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            text.color = alpha;
            yield return null;
        }

        text.fontSize = 0.15f;
        transform.localScale = Vector2.one;
        textManager.EnterPool(this.gameObject);
        gameObject.SetActive(false);
    }
}
