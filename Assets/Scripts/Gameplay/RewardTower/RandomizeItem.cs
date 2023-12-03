using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DamageNumbersPro;

public class RandomizeItem : MonoBehaviour
{
    public RewardStruct[] possibleReward;

    public float rotationTime;

    [HideInInspector]
    public GameObject currentReward;

    public DamageNumber textDamageNumber;

    private bool rewardGiven;

    public void SpawnRandomReward()
    {
        int randomIndex = Random.Range(0, possibleReward.Length);

        GameObject o = possibleReward[randomIndex].obj_Prefs.gameObject;

        currentReward = Instantiate(o, transform.position, Quaternion.identity, transform);
        currentReward.GetComponent<RewardItem>().value = possibleReward[randomIndex].moneyValue;

        currentReward.transform.DOLocalRotate(new Vector3(0, 360, 0), rotationTime, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        textDamageNumber = TextDamageProReferences.instance.Rewardtower_Item_Text;

        rewardGiven = false;
    }

    public void ReciveHit()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(currentReward.transform.DOScaleX(1.1f, 0.05f));
        mySequence.Append(currentReward.transform.DOScaleX(1, 0.05f));
    }

    public void GiveReward()
    {
        if (!rewardGiven)
        {
            rewardGiven = true;

            currentReward.transform.SetParent(null, true);

            float posOffset = 1f;
            Vector3 landingPos = new Vector3(
                currentReward.transform.position.x + Random.Range(-posOffset, posOffset),
                0,
                currentReward.transform.position.z + Random.Range(0, posOffset *2));

            currentReward.transform.DOJump(landingPos, 2 + Random.Range(-0.5f, 0.5f), 1, 1, false);

            textDamageNumber.Spawn(currentReward.transform.position, currentReward.GetComponent<RewardItem>().value + "$");

            currentReward.GetComponent<RewardItem>().canBeTaken = true;
        }
    }
}

[System.Serializable]
public struct RewardStruct
{
    public RewardItem obj_Prefs;
    public int moneyValue;
}