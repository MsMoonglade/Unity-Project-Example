using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsModule : MonoBehaviour
{
    [Header("Variables")]
    public float localXPosition;

    [Header("Project References")]
    public GameObject[] possibleWalls;

    private void OnEnable()
    {
        GenerateRandomizedWalls();
    }

    private void GenerateRandomizedWalls()
    {
        GameObject[] localWalls = new GameObject[2];

        for(int i = 0; i < localWalls.Length; i++)
        {
            GameObject o = PickRandomWall(possibleWalls);
            localWalls[i] = o;

            if (i % 2 == 0)
                localWalls[i].gameObject.transform.localPosition = new Vector3(localXPosition, 2.5f, 0);
           else
                localWalls[i].gameObject.transform.localPosition = new Vector3(-localXPosition, 2.5f, 0);
        }
    }

    private GameObject PickRandomWall(GameObject[] possiblePool)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, this.transform);
        return o;
    }
}