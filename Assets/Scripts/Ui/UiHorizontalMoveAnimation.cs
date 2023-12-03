using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiHorizontalMoveAnimation : MonoBehaviour
{
    public float animationTime;

    private void Awake()
    {
        if (transform.localPosition.x > 0)
        {
            transform.DOLocalMoveX(-transform.localPosition.x, animationTime)
                   .SetEase(Ease.InOutSine)
                   .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.DOLocalMoveX(Mathf.Abs(transform.localPosition.x), animationTime)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
