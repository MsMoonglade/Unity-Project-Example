using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectablesMoneyPile : Collectable
{
    [Header("Local References")]
    public TMP_Text localValueText;

    private void Update()
    {
        localValueText.text = value.ToString();

        if(transform.position.z <= CharacterBehaviour.instance.transform.position.z && !taken)
        {
            Take();
        }            
    }

    public override void Take()
    {
        base.Take();

        transform.DOPause();
        transform.DOKill();

        Vector3 nearestRollerPos = new Vector3(WorldRollerReferences.instance.ReturnRollerPos().x + Random.Range(-0.5f , 0.5f), 0.85f , transform.position.z + 5 + Random.Range(-1 , 1)) ;
      
        transform.DOLocalJump(nearestRollerPos, 2, 1, Random.Range(0.5f , 0.75f))
            .OnComplete(() => WorldRollerReferences.instance.TakeObject(this.gameObject));
    }
}