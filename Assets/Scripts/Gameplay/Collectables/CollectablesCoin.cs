using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesCoin : Collectable
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Player") || col.transform.CompareTag("Untagged"))
        {
            if (!taken)
            {
                taken = true;
                Take();
            }
        }
    }

    public override void Take()
    {
        base.Take();

        transform.DOScale(Vector3.zero, 0.25f)
    .SetEase(Ease.InBack)
    .OnComplete(() => this.gameObject.SetActive(false));

        GameManager.instance.IncreaseGold((int)value);
    }
}