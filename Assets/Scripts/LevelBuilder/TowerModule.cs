using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerModule : MonoBehaviour
{
    [Header("Variables")]
    public float xPositionRange;

    [Header("Project References")]
    public GameObject[] possibleTower;

    private void OnEnable()
    {
        GenerateRandomizedTower();
    }

    private void GenerateRandomizedTower()
    {              
        GameObject o = PickRandomTower(possibleTower);
        float randomX = Random.Range(-xPositionRange, xPositionRange);
        o.transform.localPosition = new Vector3(randomX , 0 , 0);
    }

    private GameObject PickRandomTower(GameObject[] possiblePool)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, this.transform);
        return o;
    }
}
