using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;
using DG.Tweening;
using TMPro;
using UnityEngineInternal;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("Variables")]
    public float timeToExitEditView;

    [Header("Local References")]
    public CanvasGroup mainMenuUi;
    public CanvasGroup gameUi;
    public CanvasGroup editUi;
    public CanvasGroup endGameUi;
    public CanvasGroup retryUi;

    public TMP_Text currentGoldText;
    public GameObject endGameconfetti;

    [Header("InstantiateElement")]
    public GameObject ui_Coin_Prefs;
    public GameObject ui_Coin_Destination;
    public float ui_Coin_AnimSpeed;

    public GameObject uiTempParent;

    private void OnEnable()
    {
        EventManager.StartListening(Events.confirmEdit, OnExitEditView);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.confirmEdit, OnExitEditView);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        retryUi.gameObject.transform.localScale = Vector3.zero;
    }

    public void EnableEditUi()
    {
        editUi.alpha = 1;

        gameUi.alpha = 0;
        endGameUi.alpha = 0;
        retryUi.alpha = 0;
        mainMenuUi.alpha = 0;
    }

    public void EnableMainMenuUi()
    {
        editUi.alpha = 0;
        gameUi.alpha = 0;
        endGameUi.alpha = 0;
        retryUi.alpha = 0;
        mainMenuUi.alpha = 1;
    }

    public void EnableGameUi()
    {
        editUi.alpha = 0;
        endGameUi.alpha = 0;
        retryUi.alpha = 0;
        mainMenuUi.alpha = 0;
        gameUi.alpha = 1;
    }

    public void DisableGameUI()
    {
        gameUi.alpha = 0;
    }

    public void EnableEndGameUi()
    {
        editUi.alpha = 0;
        retryUi.alpha = 0;
        mainMenuUi.alpha = 0;
        gameUi.alpha = 0;

        float alpha = 0;
        DOTween.To(() => alpha, x => alpha = x, 1, 0.5f)
            .OnUpdate(() => endGameUi.alpha = alpha)
            .OnComplete(() => OnEndGameUiComplete());
        
    }

    private void OnEndGameUiComplete()
    {
        endGameUi.alpha = 1;
        endGameconfetti.gameObject.SetActive(true);
    }

    public void EnableRetryUi()
    {        
        mainMenuUi.alpha = 0;
        gameUi.alpha = 0;
        endGameUi.alpha = 0;
        retryUi.alpha = 0;

        retryUi.transform.DOScale(Vector3.one, 0.5f);
    }

    public void UpdateGoldText()
    {
        currentGoldText.text = GameManager.instance.currentGold.ToString();

        currentGoldText.transform.DOScale(1, 0);
        currentGoldText.transform.DOScale(0.5f, 0.15f)
           .SetRelative(true)
           .SetEase(Ease.Linear)
           .SetLoops(2, LoopType.Yoyo);
    }
    
    public void InstantiateCoin(int amount , GameObject localPosToRef)
    {
        for(int i = 0; i < amount; i++)
        {
            Vector3 posToRef = localPosToRef.transform.position;
            
            if(localPosToRef == CharacterBehaviour.instance.gameObject)           
                posToRef += new Vector3(Random.Range(-4.0f, 4.0f),0, Random.Range(-4.0f, 8.0f));
            else
                posToRef += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

            Vector3 pos =  Camera.main.WorldToScreenPoint(posToRef);

            GameObject coin = Instantiate(ui_Coin_Prefs, pos, Quaternion.identity, uiTempParent.transform);
            coin.transform.localScale = Vector3.zero;

            coin.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);

            float animSpeed = Random.Range(ui_Coin_AnimSpeed - 0.2f, ui_Coin_AnimSpeed + 0.2f);
            coin.transform.DOMove(ui_Coin_Destination.transform.position, animSpeed)
                .SetEase(Ease.InBack)             
                .OnComplete(() => Destroy(coin));       
        }
    }

    public void LostCoin(int amount)
    {
        for (int i = 0; i < amount; i++)
        {            
            GameObject coin = Instantiate(ui_Coin_Prefs, ui_Coin_Destination.transform.position, Quaternion.identity, uiTempParent.transform);

            float animSpeed = Random.Range(ui_Coin_AnimSpeed - 0.1f, ui_Coin_AnimSpeed + 0.1f);
            Vector3 destination = new Vector3(coin.transform.position.x - Random.Range(-50, 20), coin.transform.position.y - 3000, 0);

            coin.transform.DOMove(destination, animSpeed)                
                .OnComplete(() => Destroy(coin));
        }
    }

    private void OnExitEditView(object sender)
    {
        editUi.DOFade(0 , timeToExitEditView /2);
        gameUi.DOFade(0, timeToExitEditView/2);
        endGameUi.DOFade(0, timeToExitEditView / 2);
        retryUi.DOFade(0, timeToExitEditView / 2);
        mainMenuUi.DOFade(1, timeToExitEditView);
    }
}