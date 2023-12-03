using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SurpriseBox : MonoBehaviour
{
    [Header("Variables")]
    public float rewardRandomizer;

    [Header ("Project References")]
    public GameObject[] m_possibleReward;

    [Header ("Local References")]
    public GameObject topWall;   
    public ParticleSystem m_rewardParticle;

    [Header ("Private Variables")]
    private bool m_isInAnim;
    private bool m_animRight;

    [Header ("Private References")]
    private Animator anim;
    [HideInInspector]
    public GameObject localReward;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        m_isInAnim = false;
        m_animRight = true;   
    }

    public void Open(int i_rewardAmount)
    {
        topWall.transform.DOScale(Vector3.zero, 0.5f);

        anim.SetTrigger("Anim");
        RandomizeReward(i_rewardAmount);
    }

    public void ReciveHit()
    {
        if (!m_isInAnim)
        {
            Anim();
        }
    }

    private void Anim()
    {
        Vector3 rotDir = Vector3.zero;
        Vector3 moveDir = Vector3.zero;

        if (m_animRight)
        {
            rotDir = new Vector3(0, 0, -8);
            moveDir = new Vector3(0.1f, 0.15f, 0);
        }
        else
        {
            rotDir = new Vector3(0, 0, 8);
            moveDir = new Vector3(-0.1f, 0.15f, 0);
        }

        Sequence rotSequence = DOTween.Sequence();
        rotSequence.Append(transform.DORotate(rotDir, 0.1f)
            .SetEase(Ease.InOutBounce));
        rotSequence.Append(transform.DORotate(new Vector3(0, 0, 0), 0.1f)
            .SetEase(Ease.InOutBounce));

        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(transform.DOLocalMove(moveDir, 0.1f)
            .SetEase(Ease.InOutBounce));
        moveSequence.Append(transform.DOLocalMove(new Vector3(0, 0, 0), 0.1f)
            .SetEase(Ease.InOutBounce));

        m_animRight = !m_animRight;

        if(this.gameObject.activeInHierarchy)        
            StartCoroutine(ResetAnim());
    }

    private IEnumerator ResetAnim()
    {
        m_isInAnim = true;

        yield return new WaitForSeconds(0.2f);

        m_isInAnim = false;
    }

    private void RandomizeReward(int i_rewardAmount)
    {
        //define local Reward
        int rewardIndex = Random.Range(0, m_possibleReward.Length);
        localReward = m_possibleReward[rewardIndex];    

        if (m_rewardParticle != null)
            m_rewardParticle.Play();

        if (localReward.transform.GetComponent<CollectablesMoneyPile>() != null)
        {
            for (int i = 0; i < i_rewardAmount; i++)
            {
                Vector3 pos = transform.position;
                pos = new Vector3(pos.x, 0, pos.z);
                GameObject money = PoolUtilities.instance.GetPooledItem(localReward);
                money.transform.position = pos;
                money.transform.DOScale(0, 0);

                money.GetComponent<CollectablesMoneyPile>().value = 10;
                money.gameObject.SetActive(true);
                money.transform.DOScale(1, 0.25f);

                //jump animation
                float posOffset = 4f;

                Vector3 randomizedDestination = new Vector3(
                   transform.position.x + Random.Range(-posOffset, posOffset),
                   0.55f,
                   transform.position.z + Random.Range(0, posOffset * 2));

                float jumpPower = Random.Range((float)3, (float)5.5f);
                float jumpDuration = Random.Range(1, 2.5f);

                money.transform.DOJump(randomizedDestination, jumpPower, 1, jumpDuration);
            }
        }
        else
        {
            Debug.Log("WrongReward");
        }
    }
}