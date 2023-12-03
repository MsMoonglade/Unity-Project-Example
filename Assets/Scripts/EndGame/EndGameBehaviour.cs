using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameBehaviour : MonoBehaviour
{
    public static EndGameBehaviour instance;

    [Header("Variables")]
    public AnimationCurve costPerAditionalCharacter;
    public AnimationCurve endLevelCoinReward;

    [Header("Not Edit Variables")]
    public int maxEndGameCharacters;

    [Header("Local References")]
    public Canvas localCanvas;
    public Slider localScoreSlider;
    public TMP_Text localScoreText;
    public GameObject spawnFakeMoneyPosition;

    [Header("Project References")]
    public GameObject endGameFakeMoneyPrefs;

    [Header("Private Variables")]
    private int nextCharNecessaryScore;
    private int currentScore;
    private int buyedCharacters;

    [Header("Private References")]
    private CinemachineVirtualCamera endGameCamera;    
    private Coroutine startEndGameCoroutine;

    private void Awake()
    {
        instance = this;

        startEndGameCoroutine = null;

        endGameCamera = transform.GetComponentInChildren<CinemachineVirtualCamera>();

        currentScore = 0;
        buyedCharacters = 0;

        nextCharNecessaryScore = (int)costPerAditionalCharacter.Evaluate(buyedCharacters);
        
        UpdateUi();
    }

    public void IncreaseEndGameScore(int i_amount)
    {
        if (buyedCharacters < maxEndGameCharacters)
        {
            currentScore += i_amount;

            CheckCurentLevel();
            UpdateUi();
        }
        else
        {
            localScoreText.text = "MAX";
            localScoreSlider.transform.parent.DOScale(0, 0.25f);
        }
    }

    public void IncreaseEndGameScore(int i_amount , GameObject i_moneyObject)
    {
        Destroy(i_moneyObject.GetComponent<BulletBehaviour>());
        Destroy(i_moneyObject.GetComponent<Rigidbody>());
        Destroy(i_moneyObject.GetComponent<BoxCollider>());

        i_moneyObject.transform.SetParent(transform, true);
      
        i_moneyObject.transform.DOJump(EndGameCharacterBehaviour.instance.transform.position, 3, 1, 0.5f)           
            .OnComplete(() => IncreaseEndGameScore(i_amount));
        
        i_moneyObject.transform.DORotate(new Vector3(0, 0 , -90), 0.55f)                
            .OnComplete(() => Destroy(i_moneyObject.gameObject));
    }

    public void TakeMoneyFromStore(int i_moneyAmount , int i_moneyValue)
    {
        StartCoroutine(TakeMoneyFromStorecoroutine(i_moneyAmount,i_moneyValue));
    }

    private IEnumerator TakeMoneyFromStorecoroutine(int i_moneyAmount, int i_moneyValue)
    {
        for (int i = 0; i < i_moneyAmount; i++)
        {
            Vector3 pos = spawnFakeMoneyPosition.transform.position;
            GameObject money = PoolUtilities.instance.GetPooledItem(endGameFakeMoneyPrefs);
            money.transform.position = pos;
            money.gameObject.SetActive(true);

            money.transform.DOLocalRotate(new Vector3(0, 0, -90), 0.76f)
                .OnComplete(() => money.gameObject.SetActive(false));
            money.transform.DOLocalJump(EndGameCharacterBehaviour.instance.transform.position, Random.Range(2, 4), 1, 0.75f, false)
                .OnComplete(() => IncreaseEndGameScore(i_moneyValue));

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TakeObjectFromRoller(GameObject i_object, int i_value)
    {
        i_object.transform.SetParent(transform, true);

        i_object.transform.DOLocalRotate(new Vector3(0, 0, -90), 0.75f);
        i_object.transform.DOJump(EndGameCharacterBehaviour.instance.transform.position, 2, 1, 0.75f, false)
         .OnComplete(() => OnCompleteTakeObjectFromRoller(i_object , i_value));
    }

    private void OnCompleteTakeObjectFromRoller(GameObject i_object , int i_value)
    {
        IncreaseEndGameScore(i_value);
        i_object.SetActive(false);
    }

    //Called in player
    public void StartEndGame()
    {
        if (startEndGameCoroutine == null)
        {
            startEndGameCoroutine = StartCoroutine(StartEndGameCoroutine());
        }
    }

    private IEnumerator StartEndGameCoroutine()
    {
        GameManager.instance.SetEndGame();

        //switch CurrentActive Camera
        CinemachineVirtualCameraSwitcher.instance.NoPlayerCameraPrio();
        CinemachineVirtualCameraSwitcher.instance.NoEditCameraPrio();

        endGameCamera.Priority = 10;

        yield return new WaitForSeconds(0.5f);

        localCanvas.transform.DOScale(Vector3.zero, 0.5f);

        yield return new WaitForSeconds(0.25f);

        EndGameCharacterBehaviour.instance.StartRun();
    }


    private void CheckCurentLevel()
    {
        if (currentScore >= nextCharNecessaryScore)
        {
            EndGameCharacterBehaviour.instance.AddIndividualCharacter(false);
            buyedCharacters++;

            nextCharNecessaryScore = (int)costPerAditionalCharacter.Evaluate(buyedCharacters);
            currentScore = 0;
        }
    }

    private void UpdateUi()
    {
        float levelCompletitionSlider = (float)currentScore / (float)nextCharNecessaryScore;

        if (levelCompletitionSlider < 0.1f)
            levelCompletitionSlider = 0.1f;

        localScoreSlider.value = levelCompletitionSlider;

        localScoreText.text =  (((float)nextCharNecessaryScore - (float)currentScore)).ToString() + " $";
    }

    public int EndGameCoinRewardAmount()
    {
        int o_coinAmount = 0;

        o_coinAmount = (int)endLevelCoinReward.Evaluate(GameManager.instance.CurrentLevel);

        return o_coinAmount;
    }

    public void GiveEndGameCoinReward()
    {        
        GameManager.instance.IncreaseGold(EndGameCoinRewardAmount(), EndGameCharacterBehaviour.instance.gameObject);
    }
}