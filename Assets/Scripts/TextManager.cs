using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void EnterPool(GameObject text)
    {

    }
}
