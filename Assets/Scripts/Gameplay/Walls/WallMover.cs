using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour
{
    public float moveSpeed;

    void Start()
    {
        if (moveSpeed != 0)
        {
            if (transform.localPosition.x > 0)
            {
                transform.DOLocalMoveX(-transform.localPosition.x, moveSpeed)
                       .SetEase(Ease.InOutSine)
                       .SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                transform.DOLocalMoveX(Mathf.Abs(transform.localPosition.x), moveSpeed)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
    }
}