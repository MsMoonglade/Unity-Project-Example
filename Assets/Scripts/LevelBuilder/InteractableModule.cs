using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableModule : MonoBehaviour
{
    [Header("Variables")]
    public float xPositionRange;

    [Header("Project References")]
    public GameObject[] possibleInteractable;

    private void OnEnable()
    {
        GenerateRandomizedObject();
    }

    private void GenerateRandomizedObject()
    {
        GameObject o = PickRandomObject(possibleInteractable);

        float randomX = Random.Range(-xPositionRange, xPositionRange);
        o.transform.localPosition = new Vector3(randomX, 0, 0);
    }

    private GameObject PickRandomObject(GameObject[] possiblePool)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, this.transform);
        return o;
    }
}
