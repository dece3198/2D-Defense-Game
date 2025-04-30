using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject damageText;
    private Stack<GameObject> textStack = new Stack<GameObject>();
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

    public void ExitPool(float damage)
    {
        float rnadX = Random.Range(-(rect.rect.width / 4), rect.rect.width / 4);
        float randY = Random.Range(0, 0.5f);
        Vector2 pos = new Vector2(rnadX, randY);

        if(textStack.Count == 0)
        {
            Refill(10);
        }

        GameObject text = textStack.Pop();

        text.GetComponent<RectTransform>().localPosition = pos;
        if(damage >= 1000000)
        {
            text.GetComponent<TextMeshProUGUI>().text = (damage / 1000000).ToString("N0") + "M";
        }
        else if(damage >= 1000)
        {
            text.GetComponent<TextMeshProUGUI>().text = (damage / 1000).ToString("N0") + "K";
        }
        else
        {
            text.GetComponent<TextMeshProUGUI>().text = damage.ToString("N0");
        }
        text.SetActive(true);
    }

    public void EnterPool(GameObject text)
    {
        textStack.Push(text);
        text.SetActive(false);
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
