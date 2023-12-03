using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallSpreadShoot : WallBehaviour
{
    public Image spreadImage;

    private void Awake()
    {
        base.Awake();

        isNegative = false;
    }

    public override void Take()
    {
        base.Take();

        CharacterBehaviour.instance.SetSpreadShoot();
    }

    public override void TakeHit(int i_value)
    {
        UpdateUi();
    }

    protected override void UpdateUi()
    {
        TweenTextScale();
    }

    private void TweenTextScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(spreadImage.transform.DOScale(1.15f, 0.16f));
        mySequence.Append(spreadImage.transform.DOScale(1, 0.16f));
    }
}
