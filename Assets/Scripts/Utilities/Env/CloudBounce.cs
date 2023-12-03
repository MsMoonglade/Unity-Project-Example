using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudBounce : MonoBehaviour
{
    private void Start()
    {
        transform.DOLocalMoveY(transform.position.y + Random.Range(-5, 5f) , Random.Range(1f ,3f))
            .SetLoops(-1, LoopType.Yoyo);
    }
}