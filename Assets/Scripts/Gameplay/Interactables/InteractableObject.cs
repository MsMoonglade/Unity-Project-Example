using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using DG.Tweening;

public class InteractableObject : MonoBehaviour
{
    public bool disableBullet;

    [Header("Base LocalReference")]
    public TMP_Text dynamicText;

    [Header("Private Local Variables")]
    protected bool isCompleted;

    protected virtual void Awake()
    {
        UpdateUi();

        isCompleted = false;
    }

    public virtual void Take()
    {
      
    }

    public virtual void TakeHit(int i_value)
    {

    }

    public virtual void TakeHit(GameObject i_object)
    {

    }
    protected virtual void Complete(bool i_completed)
    {
        if (!isCompleted)
        {
            isCompleted = true;

            transform.GetComponent<BoxCollider>().enabled = false;

            if (i_completed)
                CompletedAnimation();

            else
                DisableAnimation();
        }
    }

    protected virtual void DisableAnimation()
    {

    }  

    protected virtual void CompletedAnimation()
    {

    }

    protected virtual void UpdateUi()
    {
        TweenTextScale();
    }

    private void TweenTextScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(dynamicText.transform.DOScale(1.3f, 0.16f));
        mySequence.Append(dynamicText.transform.DOScale(1, 0.16f));
    }

    protected virtual void OnSetDifficulty(object sender)
    {
    }
}
