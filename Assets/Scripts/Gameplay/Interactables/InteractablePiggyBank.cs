using DamageNumbersPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePiggyBank : InteractableObject
{
    public bool forceEditVariable;
    public int totalRewardAmount;
    public int startHp;

    [Header("Variables")]
    public float[] changeMeshPercentage;
    public int[] piecesToBreak;
    public int[] rewardPercentagePerBreak;

    [Header("Don't Edit Variables")]
    public int perRewardMoneyValue;

    [Header("Local References")]
    public ParticleSystem completeParticle;
    public GameObject modelParent;

    [Header("Project References")]
    public GameObject rewardMoneyPref;

    [Header("Private Variables")]
    private int hp;
    private int currentBreakIndex;
    private List<int> usedBreakIndex = new List<int>();
    private int rewardAmountGiven;

    [Header("Private References")]
    public List<GameObject> modelPieces = new List<GameObject>();


    private void OnEnable()
    {
        EventManager.StartListening(Events.setDifficulty, OnSetDifficulty);

        currentBreakIndex = -1;
        rewardAmountGiven = 0;
        usedBreakIndex.Add(currentBreakIndex);

        if (forceEditVariable)
        {
            hp = startHp;
            UpdateUi();
        }

        foreach(Transform t in modelParent.transform.GetChild(0).transform)
        {
            modelPieces.Add(t.gameObject);
        }
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.setDifficulty, OnSetDifficulty);
    }

    public void Update()
    {
        if (transform.position.z <= CharacterBehaviour.instance.transform.position.z)
            Complete(false);
    }

    public override void TakeHit(int i_amount)
    {
        if (!isCompleted)
        {
            base.TakeHit(i_amount);

            hp -= i_amount;

            /*
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(modelParent.transform.DOScale(1.1f, 0.1f));
            mySequence.Append(modelParent.transform.DOScale(1, 0.1f));
            */

            if (hp < 0)
                hp = 0;

            if (hp == 0)
            {
                if (!isCompleted)
                {
                    Complete(true);
                }
            }

            CheckBreakModelPieces();

            if (!usedBreakIndex.Contains(currentBreakIndex))
            {
                usedBreakIndex.Add(currentBreakIndex);
                BreakModelPieces(ModelPiecesToBreak());
                GiveMoneyReward(rewardPercentagePerBreak[currentBreakIndex]);
            }

            UpdateUi();
        }
    }

    private void GiveMoneyReward(float i_rewardPercentage)
    {
        float currentRewardAmount = (totalRewardAmount * i_rewardPercentage)/100;

        int currentRewardMoney = (int)(currentRewardAmount / perRewardMoneyValue);
        
        if (currentRewardMoney == 0)
            currentRewardMoney = 1;

        for(int i = 0; i < currentRewardMoney; i++)
        {
            Vector3 pos = transform.position;
            GameObject money = PoolUtilities.instance.GetPooledItem(rewardMoneyPref);
            money.transform.position = pos;

            money.GetComponent<CollectablesMoneyPile>().value = perRewardMoneyValue;
            money.gameObject.SetActive(true);

            
            //jump animation
            float posOffset = 4f;

            Vector3 randomizedDestination = new Vector3(
               transform.position.x + Random.Range(-posOffset, posOffset),
               0.55f,
               transform.position.z + Random.Range(0, posOffset *2));

            float jumpPower = Random.Range((float)3, (float)5.5f);
            float jumpDuration = Random.Range(1, 2.5f);

            money.transform.DOJump(randomizedDestination, jumpPower, 1 , jumpDuration);

            rewardAmountGiven += perRewardMoneyValue;
        }
    }

    private void GiveMoneyReward(int i_rewardAmount , bool i_ended)
    {
        for (int i = 0; i < i_rewardAmount; i++)
        {
            Vector3 pos = transform.position;
            GameObject money = PoolUtilities.instance.GetPooledItem(rewardMoneyPref);
            money.transform.position = pos;

            money.GetComponent<CollectablesMoneyPile>().value = perRewardMoneyValue;
            money.gameObject.SetActive(true);

            //jump animation
            float posOffset = 4f;

            Vector3 randomizedDestination = new Vector3(
               transform.position.x + Random.Range(-posOffset, posOffset),
               0.55f,
               transform.position.z + Random.Range(0, posOffset * 2));

            float jumpPower = Random.Range((float)3, (float)5.5f);
            float jumpDuration = Random.Range(1, 2.5f);

            money.transform.DOJump(randomizedDestination, jumpPower, 1, jumpDuration);

            rewardAmountGiven += perRewardMoneyValue;
        }
    }

    private void BreakModelPieces(int i_amount)
    {
        for(int i = 0; i < i_amount; i++)
        {
            int randomIndex = Random.Range(0 , modelPieces.Count);

            GameObject localPiece = modelPieces[randomIndex];
            modelPieces.Remove(localPiece);

            float posOffset = 2f;

            Vector3 randomizedDestination = new Vector3(
                transform.position.x + Random.Range(-posOffset, posOffset),
                -1,
                transform.position.z + Random.Range(-posOffset, posOffset));

            float jumpPower = Random.Range((float)2 , (float)6);
            float jumpDuration = Random.Range(1, 2.5f);

            localPiece.transform.DOJump(randomizedDestination, jumpPower, 1, jumpDuration)
                .OnComplete(() => localPiece.SetActive(false));
        }
    }

    private int ModelPiecesToBreak()
    {
        int o_PiecesToBreak = 0;

        o_PiecesToBreak = piecesToBreak[currentBreakIndex];

        return o_PiecesToBreak;
    }

    private void CheckBreakModelPieces()
    {
        float currentHpPercent = (float)hp / (float)startHp;
       
        if(currentHpPercent <= changeMeshPercentage[0] && currentHpPercent >= changeMeshPercentage[1])
        {
            currentBreakIndex = 0;
        }

        else if (currentHpPercent <= changeMeshPercentage[1] && currentHpPercent >= changeMeshPercentage[2])
        {
            currentBreakIndex = 1;
        }

        else if (currentHpPercent <= changeMeshPercentage[2] && currentHpPercent >= changeMeshPercentage[3])
        {
            currentBreakIndex = 2;
        }

        else if (currentHpPercent <= changeMeshPercentage[3] && currentHpPercent >= 0.1f)
        {
            currentBreakIndex = 3;
        }
    }

    protected override void UpdateUi()
    {
        base.UpdateUi();

        dynamicText.text = hp.ToString();
    }

    //Called in Parent (Complete)
    protected override void DisableAnimation()
    {
        base.DisableAnimation();
      
        transform.DOLocalMoveY(-5f, 1)
            .SetEase(Ease.InBack)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    protected override void CompletedAnimation()
    {
        base.CompletedAnimation();

        int leftedReward = totalRewardAmount - rewardAmountGiven;
        GiveMoneyReward(leftedReward , true);

        completeParticle.transform.SetParent(null, true);
        completeParticle.Play();

        transform.DOScale(0, 0.5f);
    }

    protected override void OnSetDifficulty(object sender)
    {
        base.OnSetDifficulty(sender);

        startHp = DifficultyManager.instance.InteractableHp(GameManager.instance.CurrentLevel);
        startHp += (int)(startHp * DifficultyManager.instance.CharacterMultiplyer());

        totalRewardAmount = DifficultyManager.instance.InteractableReward(GameManager.instance.CurrentLevel);

        hp = startHp;
        UpdateUi();
    }
}