using DG.Tweening;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameStatus gameStatus;

    [HideInInspector]
    public int currentGold;

    private int currentLevel;

    [HideInInspector]
    public bool inEdit;

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }

        set
        {
            currentLevel = value;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.endGame, OnEndGame);
        EventManager.StartListening(Events.die, OnCharacterDie);
        EventManager.StartListening(Events.confirmEdit, OnExitEditview);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.endGame, OnEndGame);
        EventManager.StopListening(Events.die, OnCharacterDie);
        EventManager.StopListening(Events.confirmEdit, OnExitEditview);
    }

    private void Awake()
    {
        instance = this;

        SetInMenu();

        EnterEditView();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        DOTween.SetTweensCapacity(500, 250);           
    }

    private void Start()
    {
        //currentLevel set
        if (PlayerPrefs.HasKey("CurrentLevel"))
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
        else
        {
            CurrentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        }

        if (PlayerPrefs.HasKey("GoldCurrency"))
            currentGold = PlayerPrefs.GetInt("GoldCurrency");
        else
        {
            currentGold = 0;
            PlayerPrefs.SetInt("GoldCurrency", currentGold);
        }

        UiManager.instance.UpdateGoldText();    
    }

    public void IncreaseGold(int i_amount)
    {
        currentGold += i_amount;

        UiManager.instance.UpdateGoldText();
        
        UiManager.instance.InstantiateCoin(i_amount, CharacterBehaviour.instance.gameObject);

        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }

    public void IncreaseGold(int i_amount , GameObject i_objectReference)
    {
        currentGold += i_amount;

        UiManager.instance.UpdateGoldText();

        UiManager.instance.InstantiateCoin(i_amount, i_objectReference);

        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }

    public void DecreaseGold(int i_amount)
    {
        currentGold -= i_amount;

        if (currentGold < 0)
            currentGold = 0;

        if (currentGold <= 0)
            currentGold = 0;

        UiManager.instance.UpdateGoldText();

        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }

    public bool HaveGold(int i_amount)
    {
        if (currentGold >= i_amount)
            return true;

        else return false;
    }

    public bool LevelPassed()
    {    
        return false;
    }

    public void ResetScore()
    {
    }

    public void StartGame()
    {
        SetInGame();

        UiManager.instance.EnableGameUi();
        EventManager.TriggerEvent(Events.playGame);

        //LevelManager.instance.GenerateLevel(CurrentLevel);
    }

    public void OnEndGame(object sender)
    {        
        EventManager.TriggerEvent(Events.saveValue);
        CurrentLevel++;
        SetInMenu();

        StartCoroutine(DelayEndGameUi());

        //EventManager.TriggerEvent(Events.endGame);
    }

    private IEnumerator DelayEndGameUi()
    {
        yield return new WaitForSeconds(0.5f);
        UiManager.instance.EnableEndGameUi();
    }

    public void OnCharacterDie(object sender)
    {
        gameStatus = GameStatus.InRestart;
        UiManager.instance.EnableRetryUi();
    }

    public void ReloadLevel()
    {        
        SceneManager.LoadScene(1);
    }

    public void SetInGame()
    {
        gameStatus = GameStatus.InGame;
    }

    public void SetInMenu()
    {
        gameStatus = GameStatus.InMenu;
    }
    public void SetEndGame()
    {
        gameStatus = GameStatus.InEndGame;
    }    

    public bool IsInGameStatus()
    {
        if (gameStatus == GameStatus.InGame)
            return true;

        else
            return false;
    }

    public bool IsInEndGameStatus()
    {
        if (gameStatus == GameStatus.InEndGame)
            return true;

        else
            return false;
    }

    public void OnApplicationQuit()
    {
    }

    public void EnterEditView()
    {
        UiManager.instance.EnableEditUi();

        CharacterBehaviour.instance.SetInEditPos();
        CinemachineVirtualCameraSwitcher.instance.NoPlayerCameraPrio();
        CinemachineVirtualCameraSwitcher.instance.EditCameraPrio();
      
        inEdit = true;        
    }

    public void OnExitEditview(object sender)
    {       
        CinemachineVirtualCameraSwitcher.instance.PlayerCameraPrio();
        CinemachineVirtualCameraSwitcher.instance.NoEditCameraPrio();

        CharacterBehaviour.instance.ConfirmEdit();

        inEdit = false;
    }

    public void DetectStartGameButton()
    {
        StartGame();
        InputManager.instance.FirstInput();
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        ReloadLevel();
    }
}

public enum GameStatus
{
    InGame,
    InMenu,
    InRestart,
    InEndGame
}