using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject damageText;
    private Stack<GameObject> textStack = new Stack<GameObject>();
    private List<DamageText> activeTexts = new List<DamageText>();
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject text = Instantiate(damageText, transform);
            text.transform.position = transform.position;
            text.GetComponent<DamageText>().textManager = this;
            textStack.Push(text);
        }
    }

    public void ShowDamageText(float damage)
    {
        float rnadX = Random.Range(-(rect.rect.width / 4), rect.rect.width / 4);
        float randY = Random.Range(0, 0.5f);
        Vector2 pos = new Vector2(rnadX, randY);

        if(textStack.Count == 0)
        {
            Refill(10);
        }

        GameObject text = textStack.Pop();
        DamageText dt = text.GetComponent<DamageText>();
        activeTexts.Add(dt);

        text.GetComponent<RectTransform>().localPosition = pos;
        if(damage >= 1_000_000)
        {
            text.GetComponent<TextMeshProUGUI>().text = (damage / 1000000).ToString("0.#") + "M";
        }
        else if(damage >= 1_000)
        {
            text.GetComponent<TextMeshProUGUI>().text = (damage / 1000).ToString("0.#") + "K";
        }
        else
        {
            text.GetComponent<TextMeshProUGUI>().text = damage.ToString("0");
        }
        text.SetActive(true);
    }

    public void EnterPool(GameObject text)
    {
        var dt = text.GetComponent<DamageText>();
        activeTexts.Remove(dt);
        textStack.Push(text);
        text.SetActive(false);
    }

    public void StopAllTexts()
    {
        foreach(var dt in activeTexts.ToList())
        {
            dt.StopText();
        }
        activeTexts.Clear();
    }

    private void Refill(int Count)
    {
        for (int i = 0; i < Count; i++)
        {
            GameObject text = Instantiate(damageText, transform);
            text.transform.position = transform.position;
            text.GetComponent<DamageText>().textManager = this;
            textStack.Push(text);
        }
    }
}
