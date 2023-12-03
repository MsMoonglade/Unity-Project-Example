using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRollerReferences : MonoBehaviour
{
    public static WorldRollerReferences instance;

    [Header("Variables")]
    public float objectInRollerMovespeed;

    [Header("Don't Edit Variables")]
    public float jumpToStorePower;
    public float jumpToStoreTime;

    [Header("Local References")]
    public GameObject rollerDestinationTarger;
    public GameObject gameplayElementParent;
    public GameObject model;
    public GameObject inStoreObjectPosition;
    public GameObject shopModelObject;

    [Header("Private References")]
    private List<GameObject> objectInRoller = new List<GameObject>();

    private void Awake()
    {
        instance = this;    
    }

    public Vector3 ReturnRollerPos()
    {
        return model.transform.position;
    }

    public void TakeObject(GameObject i_newObject)
    {
        i_newObject.transform.SetParent(gameplayElementParent.transform, true);

        float zDistance = rollerDestinationTarger.transform.position.z - i_newObject.transform.position.z;
        float reachEndNecessaryTime = zDistance / objectInRollerMovespeed;

        i_newObject.transform.DOMoveZ(rollerDestinationTarger.transform.position.z, reachEndNecessaryTime)
            .SetEase(Ease.Linear)
            .OnComplete(() => MoveToStore(i_newObject));
    }

    private void MoveToStore(GameObject i_object)
    {
        if (i_object.transform.GetComponent<CollectablesMoneyPile>() != null)
        {
            MoveToEndGameCharacter(i_object);
            return;
        }

        int currentObjectValue = 0;

        if(i_object.transform.GetComponent<InteractableSuitcase>() != null)
        {
            currentObjectValue = i_object.transform.GetComponent<InteractableSuitcase>().localMoneyValue;
        }

        if (i_object.transform.GetComponent<RewardItem>() != null)
        {
            currentObjectValue = i_object.transform.GetComponent<RewardItem>().value / 2;
        }

        i_object.transform.DOJump(inStoreObjectPosition.transform.position, jumpToStorePower, 1, jumpToStoreTime, false)
            .OnComplete(() => AnimateShop(currentObjectValue));  
    }

    private void MoveToEndGameCharacter(GameObject i_object)
    {
        int o_objectValue = i_object.transform.GetComponent<CollectablesMoneyPile>().value;

        EndGameBehaviour.instance.TakeObjectFromRoller(i_object, o_objectValue);
    }

    private void GiveMoneyToEndGame(int i_totalValue)
    {
        int o_moneyAmount = 1;
        int o_moneyValue = 10;

        if (i_totalValue >= 500)
            o_moneyValue = 25;

        if (i_totalValue >= 1000)
            o_moneyValue = 50;

        o_moneyAmount = (int)(i_totalValue / o_moneyValue);

        if (o_moneyAmount == 0)
            o_moneyAmount = 1;

        EndGameBehaviour.instance.TakeMoneyFromStore(o_moneyAmount, o_moneyValue);
    }


    private void AnimateShop(int i_totalValue)
    {
        float animSpeed = 0.5f;

        shopModelObject.transform.DOScale(1, 0);

        shopModelObject.transform.DOScaleX(0.6f, animSpeed)
            .SetEase(Ease.InBack);

        shopModelObject.transform.DOScaleZ(0.6f, animSpeed)
            .SetEase(Ease.InBack);

        shopModelObject.transform.DOScaleY(1.4f, animSpeed)    
            .SetEase(Ease.InBack)
            .OnComplete(() => GiveMoneyToEndGame(i_totalValue));


        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(animSpeed);
        seq.Append(shopModelObject.transform.DOScale(1 , animSpeed));
    }
}