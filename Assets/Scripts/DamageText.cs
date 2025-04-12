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
        float time = 2;
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            text.color = alpha;
            yield return null;
        }
        if (transform.CompareTag("DamageText"))
        {
            textManager.EnterPool(this.gameObject);
        }
        gameObject.SetActive(false);
    }
}
