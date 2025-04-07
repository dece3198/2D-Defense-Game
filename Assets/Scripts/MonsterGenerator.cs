using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters;



    private IEnumerator MonsterCo()
    {
        int count = 0;

        while(count >= 40)
        {
            yield return new WaitForSeconds(3);
            count++;
        }
    }
}
