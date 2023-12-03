using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class EndGameSingleObstacle : MonoBehaviour
{
    [Header("Variables")]
    public float disableAnimationSpeed;

    [Header("Local References")]
    public TMP_Text hpText;
    public GameObject coinParent;
    public GameObject rewardObject;

    [Header("Private Variables")]
    private bool enable;

    [Header("Private References")]
    private Collider col;

    [HideInInspector]
    public int hp;

    public void SetHp(int i_value)
    {
        hp = i_value;
        UpdateUi();

        col = transform.GetComponent<Collider>();
        enable = true;
    }

    public void TakeHit()
    {
        hp--;

        TweenScale(1.1f , 0.25f);

        CheckHp();  

        UpdateUi();
    }

    private void CheckHp()
    {
        if (hp < 0)
            hp = 0;

        if (hp == 0 && enable)
        {
            Disable(5 , 0.5f);
        }
    }

    private void Disable(float i_yPos , float i_tweenTime)
    {
        enable = false;

        col.enabled = false;

        transform.DOLocalMoveY(-i_yPos, i_tweenTime)    
            .SetEase(Ease.InBack);

        rewardObject.transform.GetComponent<CollectablesEndGameCoin>().SetCanBeTaken(true);

        Sequence mySequence = DOTween.Sequence();
        mySequence.PrependInterval(i_tweenTime * 0.25f);

        mySequence.AppendCallback(() => rewardObject.transform.SetParent(transform.parent.transform.parent, true));

        mySequence.Append(rewardObject.transform.DOMoveY(0.5f, i_tweenTime)
            .SetEase(Ease.InBack));  

            //.OnComplete(() => this.gameObject.SetActive(false));
    }

    private void TweenScale(float i_newSize , float i_tweenTime)
    {
        transform.DOScale(1, 0);
        transform.DOScale(i_newSize, i_tweenTime)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void UpdateUi()
    {       
        hpText.text = hp.ToString();
    }
}