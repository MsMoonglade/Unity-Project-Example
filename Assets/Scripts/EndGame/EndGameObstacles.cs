using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameObstacles : MonoBehaviour
{
    public void SetObstaclesHp(int i_value)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.GetComponent<EndGameSingleObstacle>().SetHp(i_value);
        }
    }
}
