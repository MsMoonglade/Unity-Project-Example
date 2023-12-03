using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameBulletBehaviour : MonoBehaviour
{
    [Header("Variables")]
    public float moveSpeed;

    [Header("Not Edit Variables")]
    [HideInInspector]
    public bool moving;

    private void OnDisable()
    {
        moving = false;
    }

    public void StartMove()
    {
        this.gameObject.SetActive(true);
        moving = true;
    }

    private void Update()
    {
        if (moving)
        {
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider  col)
    {
        if (col.transform.CompareTag("EndGameObstacles"))
        {
            if (col.transform.GetComponent<EndGameSingleObstacle>() != null)
            {
                col.transform.GetComponent<EndGameSingleObstacle>().TakeHit();
            }

            this.gameObject.SetActive(false);
        }
    }
}