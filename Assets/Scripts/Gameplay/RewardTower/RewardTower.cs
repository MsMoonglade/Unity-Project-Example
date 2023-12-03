using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lofelt.NiceVibrations;
using static UnityEngine.Rendering.DebugUI;

public class RewardTower : MonoBehaviour
{
    [Header("Variables")]
    public bool customValue;
    public int hp;
    public int coinRewardAmount;
    public int moneyPileSurpriseRewardAmount;

    [Header("Fixed Variables")]
    public bool fixedValue;
    public int hpPerElement;

    [Header("Not Edit Variables")]
    public RewardType rewardType;
    public float floorLevelingOffset;
    public float elementRotOffset;
    public float elementYOffset;
    public float rewardYOffset;

    [Header("Project References")]

    [Header("Local References")]
    public GameObject towerParent;
    public GameObject rewardParent;
    public GameObject towerElementPrefs;
    public TMP_Text valueText;
    public GameObject saleTiketObject;
    public ParticleSystem completeParticle;

    [Header("Private Variables")]
    private float playerHitTaken;
    private float movedOffset;
    private int startHp;

    [Header("Private References")]
    private Collider col;
    private SurpriseBox surpriseBox;
    private RandomizeItem randomizedItem;

    private void OnEnable()
    {
        EventManager.StartListening(Events.setDifficulty, OnSetDifficulty);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Events.setDifficulty, OnSetDifficulty);
    }

    private void Start()
    { 
        col = GetComponent<Collider>();

        if (customValue)
        {
            movedOffset = 0;
            playerHitTaken = 0;

            startHp = hp;

            GenerateTower();
            SetupReward();
            valueText.text = hp.ToString() + "$";
        }

        transform.localPosition += new Vector3(0, floorLevelingOffset, 0);
    }

    private void Update()
    {
        if(col.enabled)
        {
            if (transform.position.z < CharacterBehaviour.instance.transform.position.z)
                Disable();
        }
    }

    public void TakeHit(int i_amount)
    {
        hp -= i_amount;

        if(hp < 0)
            hp = 0;

        valueText.text = hp.ToString() + "$";

        TweenTowerScale();
        CheckHp();

        if (rewardType == RewardType.SurpriseBox)
            surpriseBox.ReciveHit();


        if (rewardType == RewardType.Item)
            randomizedItem.ReciveHit();
    }

    public void HitPlayer()
    {
        if(playerHitTaken == 0)
        {
            playerHitTaken++;
            ShakeAllElement(0.3f ,1f , 50 , 85);
        }

        else
        {
            ShakeAllElementAndDisable();
        }
    }

    private void ShakeAllElement(float i_time , float i_strenght , int i_vibrato , float i_randomness )
    {
        foreach(Transform t in towerParent.transform)
        {
            t.DOShakePosition(i_time, i_strenght, i_vibrato , i_randomness , false , true , ShakeRandomnessMode.Harmonic);
            t.DOShakeRotation(i_time , i_strenght , i_vibrato , i_randomness , true , ShakeRandomnessMode.Harmonic);
        }

        rewardParent.transform.DOShakePosition(i_time, i_strenght/2, i_vibrato, i_randomness, false, true, ShakeRandomnessMode.Harmonic);
    }

    private void ShakeAllElementAndDisable()
    {
        col.enabled = false;
       
        ShakeAllElement(0.3f, 1f, 50, 85);
      
        transform.DOLocalMoveY(-10f, 1)
            .SetEase(Ease.InBack)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    private void Disable()
    {
        col.enabled = false;

        transform.DOLocalMoveY(-10f, 1)
          .SetEase(Ease.InBack)
          .OnComplete(() => this.gameObject.SetActive(false));
    }

    private void CheckHp()
    {
        float actualY = (movedOffset * hp) / startHp;
        actualY -= movedOffset;

        towerParent.transform.DOLocalMoveY(actualY, 0.3f);
        rewardParent.transform.DOLocalMoveY(actualY + movedOffset , 0.3f);

        if (hp <= 0)
        {
            StartCoroutine(CompleteTower());    
        }
    }

    private IEnumerator CompleteTower()
    {
        col.enabled = false;

        saleTiketObject.transform.DOScale(0, 0.2f);
        valueText.transform.parent.DOScale(Vector3.zero, 0.2f);

        completeParticle.Play();
        hp = 0;

        GiveReward();

        yield return new WaitForSeconds(0.15f);

        towerParent.transform.DOScale(0, 0.6f)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    private void GiveReward()
    {
        if (rewardType == RewardType.Coin)
            GameManager.instance.IncreaseGold((int)coinRewardAmount);

        if(rewardType == RewardType.SurpriseBox && surpriseBox != null)
        {
            //surpriseBox.Open();

            Vector3 newPos = transform.position + new Vector3(0, 0.1f, 0);
            SurpriseBox newBox = Instantiate(surpriseBox , newPos , Quaternion.identity , transform.parent).GetComponent<SurpriseBox>();
            newBox.Open(moneyPileSurpriseRewardAmount);

            surpriseBox.gameObject.SetActive(false);
        }

        if (rewardType == RewardType.Item && randomizedItem != null)
        {
           randomizedItem.GiveReward(); 
        }
    }

    private void GenerateTower()
    {
        int valuePerElement = 1;

        if (hp <= 30)
            valuePerElement = 2;
        else if (hp > 30 && hp <= 80)
            valuePerElement = 5;
        else if (hp > 80 && hp <= 150)
            valuePerElement = 6;
        else
            valuePerElement = 10;

        if (fixedValue)
            valuePerElement = hpPerElement;

        int necesaryElement = (int)(hp / valuePerElement);
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < necesaryElement; i++)
        {
            GameObject element = Instantiate(towerElementPrefs, pos, Quaternion.identity, towerParent.transform);
            element.transform.localPosition = pos;
            element.transform.localRotation *= Quaternion.Euler(0, (elementRotOffset * i), 0);

            pos += new Vector3(0, elementYOffset, 0);
            movedOffset += elementYOffset;
        }

        rewardParent.transform.localPosition = new Vector3(rewardParent.transform.localPosition.x, pos.y, rewardParent.transform.localPosition.z);
    }

    private void SetupReward()
    {
        if (rewardType == RewardType.SurpriseBox)
        {
            surpriseBox = rewardParent.GetComponentInChildren<SurpriseBox>();
        }

        if(rewardType == RewardType.Item)
        {
            randomizedItem = rewardParent.GetComponentInChildren<RandomizeItem>();
            randomizedItem.SpawnRandomReward();
        }
    }

    private void TweenTowerScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScaleX(1.1f, 0.05f));
        mySequence.Append(transform.DOScaleX(1, 0.05f));

        Sequence mySequence2 = DOTween.Sequence();
        mySequence2.Append(transform.DOScaleZ(1.1f, 0.05f));
        mySequence2.Append(transform.DOScaleZ(1, 0.05f));
      
        Sequence mySequence3 = DOTween.Sequence();
        mySequence3.Append(valueText.transform.DOScale(1.1f, 0.05f));
        mySequence3.Append(valueText.transform.DOScale(1f, 0.05f));
    }

    private void OnSetDifficulty(object sender)
    {
        movedOffset = 0;
        playerHitTaken = 0;

        startHp = DifficultyManager.instance.TowerHp(GameManager.instance.CurrentLevel);
        startHp += (int)(startHp * DifficultyManager.instance.CharacterMultiplyer());

        hp = startHp;

        GenerateTower();
        SetupReward();
        valueText.text = hp.ToString() + "$";

        moneyPileSurpriseRewardAmount = DifficultyManager.instance.TowerReward(GameManager.instance.CurrentLevel);
    }
}

public enum RewardType
{
    Coin,
    SurpriseBox,
    Item
}