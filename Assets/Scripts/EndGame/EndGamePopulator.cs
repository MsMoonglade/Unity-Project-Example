using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePopulator : MonoBehaviour
{
    [Header("Variables")]
    public AnimationCurve obstaclesHp;
    public int obstaclesQuantity;
    public float obstaclesStartDistance;
    public float obstaclesDistance;

    [Header("Local References")]
    public GameObject obstaclesParent;

    [Header("Project References")]
    public GameObject endGameObstaclesPref;

    [Header("Private Variables")]
    private Vector3 localobstaclesPos;

    private void OnEnable()
    {
        Generateobstacles();
        SetObstaclesHp();
    }

    private void Generateobstacles()
    {
        localobstaclesPos = Vector3.zero;
        localobstaclesPos += new Vector3(0, 0, obstaclesStartDistance);

        for(int i = 0; i < obstaclesQuantity; i++)
        {
            GameObject currentObstacles = Instantiate(endGameObstaclesPref , localobstaclesPos , Quaternion.identity , obstaclesParent.transform);
            currentObstacles.transform.localPosition = localobstaclesPos;

            localobstaclesPos += new Vector3(0, 0, obstaclesDistance);
        }
    }

    private void SetObstaclesHp()
    {
        for(int i = 0; i < obstaclesParent.transform.childCount; i++)
        {
            int o_thisObstacleHp = (int)obstaclesHp.Evaluate(i);

            obstaclesParent.transform.GetChild(i).GetComponent<EndGameObstacles>().SetObstaclesHp(o_thisObstacleHp);
        }
    }
}