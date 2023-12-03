using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedElementModule : MonoBehaviour
{
    [Header("Variables")]
    public float rightxPositionRange;
    public float leftxPositionRange;
    public float wallPercentRate;

    [Header("Project References")]
    public GameObject[] possibleWalls;
    public GameObject[] possibleGameplayElement;

    private void OnEnable()
    {
        GenerateRandomizedObject();
    }

    private void GenerateRandomizedObject()
    {
        //RightObject
        float rRandomWallPercent = Random.Range(0.0f, 1.0f);
        
        if (rRandomWallPercent <= wallPercentRate)
        {
            GameObject ro = PickRandomObject(possibleWalls);
            ro.gameObject.transform.localPosition = new Vector3(rightxPositionRange, 2.5f, 0);
        }

        else
        {
            GameObject ro = PickRandomObject(possibleGameplayElement);

            float rrandomX = Random.Range(1.5f, rightxPositionRange);
            ro.transform.localPosition = new Vector3(rrandomX, 0, 0);
        }

        //Leftobject
        //RightObject
        float lRandomWallPercent = Random.Range(0.0f, 1.0f);

        if (lRandomWallPercent <= wallPercentRate)
        {
            GameObject lo = PickRandomObject(possibleWalls);
            lo.gameObject.transform.localPosition = new Vector3(leftxPositionRange, 2.5f, 0);
        }
        else
        {
            GameObject lo = PickRandomObject(possibleGameplayElement);

            float lrandomX = Random.Range(leftxPositionRange, -1.5f);
            lo.transform.localPosition = new Vector3(lrandomX, 0, 0);
        }
    }

    private GameObject PickRandomObject(GameObject[] possiblePool)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, this.transform);
        return o;
    }
}