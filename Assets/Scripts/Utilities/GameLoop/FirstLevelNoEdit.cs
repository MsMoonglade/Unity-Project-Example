using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstLevelNoEdit : MonoBehaviour
{
    public Image obscurer;

    private void Start()
    {
        EventManager.TriggerEvent(Events.confirmEdit);

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.6f);
        seq.Append(obscurer.DOFade(0, 1.25f)
            .OnComplete(() => Destroy(this.gameObject)));
    }
}