using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    [HideInInspector]
    public int value;

    [HideInInspector]
    public bool canBeTaken;
    private bool taken;

    private void OnEnable()
    {
        canBeTaken = false;
        taken = false;
    }

    private void Update()
    {
        if (canBeTaken && !taken)
        {
            if ((CharacterBehaviour.instance.transform.position.z + 2) >= transform.position.z)
            {
                DisableAnimation();
            }
        }
    }

    private void DisableAnimation()
    {
        taken = true;

        transform.DOPause();
        transform.DOKill();

        Vector3 nearestRollerPos = new Vector3(WorldRollerReferences.instance.ReturnRollerPos().x, transform.localPosition.y, transform.localPosition.z + 5);

        Sequence disableSequence = DOTween.Sequence();

        disableSequence.Append(transform.DOLocalJump(nearestRollerPos, 2, 1, 0.75f)
            .OnComplete(() => WorldRollerReferences.instance.TakeObject(this.gameObject)));
    }
}
