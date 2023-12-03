using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotTween : MonoBehaviour
{
    [Header("Variables")]
    public float rotSpeed;
    public Vector3 rotDir;

    private void Start()
    {
        transform.DOLocalRotate(rotDir, rotSpeed, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(10, LoopType.Restart);       
    }
}