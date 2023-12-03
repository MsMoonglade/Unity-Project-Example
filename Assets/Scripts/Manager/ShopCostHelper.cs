using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopCostHelper : MonoBehaviour
{
    public static ShopCostHelper instance;

    [Header ("Variables")]
    public int initialMoneyShopCost;
    public int moneyPerLevelCostDelta;
    public int[] moneyToBuyTreshold;
    [HideInInspector]
    public int currentMoneyShopCost;
    public int[] characterLevelTreshold;

    [Header ("Private Variables")]
    private int moneyUnlockedIndex;
    private int oldMoneyUnlockedIndex;
    private int totalBuyedMoney;
    private int currentCharacterExp;
    private int currentcharacterLevel;
    
    [HideInInspector]
    public int newMoneyValue;

    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("TotalBuyedMoney"))
            totalBuyedMoney = PlayerPrefs.GetInt("TotalBuyedMoney");
        else
        {
            totalBuyedMoney = 0;
            PlayerPrefs.SetInt("TotalBuyedMoney", totalBuyedMoney);
        }

        if (PlayerPrefs.HasKey("CurrentCharacterExp"))
            currentCharacterExp = PlayerPrefs.GetInt("CurrentCharacterExp");
        else
        {
            currentCharacterExp = 0;
            PlayerPrefs.SetInt("CurrentCharacterExp", currentCharacterExp);
        }

        if (PlayerPrefs.HasKey("CurrentCharacterLevel"))
            currentcharacterLevel = PlayerPrefs.GetInt("CurrentCharacterLevel");
        else
        {
            currentcharacterLevel = 1;
            PlayerPrefs.SetInt("CurrentCharacterLevel", currentcharacterLevel);
        }

        StartMoneytreshold  ();
    }

    private void Start()
    {
        EventManager.TriggerEvent(Events.updateCost);
    }

    public void MoneyIsBuyed()
    {
        totalBuyedMoney++;
        currentCharacterExp++;

        PlayerPrefs.SetInt("TotalBuyedMoney", totalBuyedMoney);
        
        if(ReturnCharacterLevel() != 3)        
            CheckCharacterLevel();

        CheckMoneyTreshold();
    }

    public int ReturnTotalMoneyBuyed()
    {
        return totalBuyedMoney;
    }

    public int ReturnCurentMoneytreshHoldValue()
    {
        int o_value = 0;
      
        if (totalBuyedMoney < moneyToBuyTreshold[0])
        {
            o_value = moneyToBuyTreshold[0];
        }
        else if (totalBuyedMoney < moneyToBuyTreshold[1] && totalBuyedMoney >= moneyToBuyTreshold[0])
        {
            o_value = moneyToBuyTreshold[1];
        }
        else
        {
            o_value = moneyToBuyTreshold[2];
        }

        return o_value;            
    }

    public int ReturnCurentMoneytreshHoldIndex()
    {
        int o_index = 0;

        if (totalBuyedMoney < moneyToBuyTreshold[0])
        {
            o_index = 0;
        }
        else if (totalBuyedMoney < moneyToBuyTreshold[1] && totalBuyedMoney >= moneyToBuyTreshold[0])
        {
            o_index = 1;
        }
        else
        {
            o_index = 2;
        }

        return o_index;
    }

    public float ReturnCharacterExpProgression()
    {
        float o_Progression = 0;

        int nextLevelCharExp = characterLevelTreshold[currentcharacterLevel - 1];
          
        o_Progression = (float)currentCharacterExp / (float)nextLevelCharExp;       

        return o_Progression;
    }

    public int ReturnCharacterLevel()
    {
        int o_level = currentcharacterLevel;
        return o_level;
    }

    private void CheckMoneyTreshold()
    {
        if (totalBuyedMoney < moneyToBuyTreshold[0])
        {
            moneyUnlockedIndex = 0;
            oldMoneyUnlockedIndex = 0;
        }

        else if (totalBuyedMoney < moneyToBuyTreshold[1] && totalBuyedMoney >= moneyToBuyTreshold[0])
        {
            moneyUnlockedIndex = 1;

            if(oldMoneyUnlockedIndex == 0)
            {
                ButtonsValueRefresh.Instance.AnimateBuyButton();
            }

            oldMoneyUnlockedIndex = 1;
        }
        else 
        {
            moneyUnlockedIndex = 2;     
            
            if(oldMoneyUnlockedIndex == 1)
            {
                ButtonsValueRefresh.Instance.AnimateBuyButton();
            }

            oldMoneyUnlockedIndex = 2;
        }

        newMoneyValue = moneyUnlockedIndex + 1;
        
        SetMoneyCost();
    }
    private void StartMoneytreshold()
    {
        if (totalBuyedMoney < moneyToBuyTreshold[0])
        {
            moneyUnlockedIndex = 0;
        }

        else if (totalBuyedMoney < moneyToBuyTreshold[1] && totalBuyedMoney >= moneyToBuyTreshold[0])
        {
            moneyUnlockedIndex = 1;
        }
        else
        {
            moneyUnlockedIndex = 2;           
        }

        oldMoneyUnlockedIndex = moneyUnlockedIndex;
        newMoneyValue = moneyUnlockedIndex + 1;
        SetMoneyCost() ;    
    }

    private void CheckCharacterLevel()
    {        
        int nextLevelCharExp = characterLevelTreshold[currentcharacterLevel-1];

        if(currentCharacterExp >= nextLevelCharExp)
        {
            currentcharacterLevel ++;
            PlayerPrefs.SetInt("CurrentCharacterLevel", currentcharacterLevel);

            currentCharacterExp = 0;
            PlayerPrefs.SetInt("CurrentCharacterExp", currentCharacterExp);

            EventManager.TriggerEvent(Events.characterLevelUp);
        }

        EventManager.TriggerEvent(Events.updateCost);
    }


    private void SetMoneyCost()
    {
        currentMoneyShopCost = initialMoneyShopCost;
        
        for (int i = 0; i < moneyUnlockedIndex; i++) 
        {
            currentMoneyShopCost +=  moneyPerLevelCostDelta;                
        }

        EventManager.TriggerEvent(Events.updateCost);
    }
}