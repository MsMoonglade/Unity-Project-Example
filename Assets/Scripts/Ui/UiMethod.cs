using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMethod : MonoBehaviour
{
    public static UiMethod instance;
       
    //PRITER SCALE
    private Vector3 printerScale;
    public int printerScaleIndex;

    //JUMP SPEED
    public  float jumpSpeedAddPerLevel;
    public int jumpSpeedIndex;

    //MOVE SPEED
    public float moveSpeedAddPerLevel;
    public int moveSpeedIndex;

    private void Awake()
    {
        instance = this;

        EventManager.StartListening(Events.characterLevelUp, OnIncreasePrinterSize);

        if (PlayerPrefs.HasKey("PrinterScaleIndex"))
            printerScaleIndex = PlayerPrefs.GetInt("PrinterScaleIndex");
        else
        {
            printerScaleIndex = 1;
            PlayerPrefs.SetInt("PrinterScaleIndex", printerScaleIndex);
        }

        if (PlayerPrefs.HasKey("JumpSpeedIndex"))
            jumpSpeedIndex = PlayerPrefs.GetInt("JumpSpeedIndex");
        else
        {
            jumpSpeedIndex = 0;
            PlayerPrefs.SetInt("JumpSpeedIndex", jumpSpeedIndex);
        }

        if (PlayerPrefs.HasKey("MoveSpeedIndex"))
            moveSpeedIndex = PlayerPrefs.GetInt("MoveSpeedIndex");
        else
        {
            moveSpeedIndex = 0;
            PlayerPrefs.SetInt("MoveSpeedIndex", moveSpeedIndex);
        }
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.characterLevelUp, OnIncreasePrinterSize);
    }

    private void Start()
    {
        printerScale = CharacterBehaviour.instance.printerObject.transform.localScale;  
    }

    public void GoToNextLevel()
    {
        GameManager.instance.ReloadLevel();
    }

    public void BuyMoney()
    {
        if (GameManager.instance.HaveGold(ShopCostHelper.instance.currentMoneyShopCost))
        {
            if (InventoryBehaviour.Instance.HaveFreeSlot())
            {                
                GameManager.instance.DecreaseGold(ShopCostHelper.instance.currentMoneyShopCost);

                InventoryBehaviour.Instance.GenerateMoneyInInv(ShopCostHelper.instance.newMoneyValue);
                ShopCostHelper.instance.MoneyIsBuyed();
            }
        }
    }

    public void SaveCurrentMoneyStatus()
    {
        CharacterBehaviour.instance.SavePlayerValue();
        InventoryBehaviour.Instance.SaveInvValue();
    }

    public void ExitFromEdit()
    {
        EventManager.TriggerEvent(Events.confirmEdit);
    }

    public void RestartLevel()
    {
        GameManager.instance.ReloadLevel();
    }

    public void StartGame()
    {
        GameManager.instance.DetectStartGameButton();
    }

    public void GM_ClearAllSave()
    {
        GameManager.instance.ClearPlayerPrefs();    
    }
    public void GM_1kGold()
    {
        GameManager.instance.IncreaseGold(1000);
    }


    public void OnIncreasePrinterSize(object sender)
    {
        if (printerScaleIndex < 3)
        {
            Vector3 newScale = Vector3.zero;

            if (printerScaleIndex == 1)
            {
                newScale = printerScale += new Vector3(1.5f, 0, 0);
                CharacterBehaviour.instance.ApplyPrinterScale(newScale, true);

            }
            else
            {
                newScale = printerScale += new Vector3(0, 0, 0.75f);
                CharacterBehaviour.instance.ApplyPrinterScale(newScale, false);
            }

            printerScaleIndex++;
            PlayerPrefs.SetInt("PrinterScaleIndex", printerScaleIndex);
        }
    }    

    /*
    public void IncreaseFireRate()
    {
        if (GameManager.instance.HaveGold(ShopCostHelper.instance.actualJumpSpeedCost))
        {
            GameManager.instance.DecreaseGold(ShopCostHelper.instance.actualJumpSpeedCost);

            float amount = jumpSpeedAddPerLevel;
            CharacterBehaviour.instance.IncreaseJumpSpeed(amount);

            jumpSpeedIndex++;
            PlayerPrefs.SetInt("JumpSpeedIndex", jumpSpeedIndex);

            ShopCostHelper.instance.UpdateCost();
        }
    }
    */

    /*
    public void IncreaseMoveSpeed()
    {
        if (GameManager.instance.HaveGold(ShopCostHelper.instance.actualMoveSpeedCost))
        {
            GameManager.instance.DecreaseGold(ShopCostHelper.instance.actualMoveSpeedCost);

            float amount = moveSpeedAddPerLevel;
            CharacterBehaviour.instance.IncreaseMoveSpeed(amount);

            moveSpeedIndex++;
            PlayerPrefs.SetInt("MoveSpeedIndex", moveSpeedIndex);

            ShopCostHelper.instance.UpdateCost();
        }
    }
    */

    /*
    public void IncreaseGoldPerHour()
    {        
        PassiveIncome.instance.IncreaseGoldPerHour();        
    }
    */
}