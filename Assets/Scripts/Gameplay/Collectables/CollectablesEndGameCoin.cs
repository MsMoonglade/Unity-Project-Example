using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectablesEndGameCoin : Collectable
{
    [Header("Variables")]
    public int coinAmountToAdd;
    //public int uiCoinToSpawn;

    [Header("Private Variables")]
    public bool canBeTaken;

    private void OnEnable()
    {
        canBeTaken = false;
        taken = false;
    }

    private void Update()
    {
        if (transform.position.z <= EndGameCharacterBehaviour.instance.transform.position.z && !taken && canBeTaken)
        {
            Take();
        }
    }

    public void SetCanBeTaken(bool i_canBeTaken)
    {
        canBeTaken = i_canBeTaken;
    }

    public override void Take()
    {
        base.Take();

        transform.DOPause();
        transform.DOKill();

        GameManager.instance.IncreaseGold(coinAmountToAdd , this.gameObject);

        transform.DOScale(0, 0.65f)
            .SetEase(Ease.InBack);
    }
}