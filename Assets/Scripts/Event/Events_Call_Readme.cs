using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events_Call_Readme : MonoBehaviour
{
    // EVENTS : endGame     -->     EndGameCharacterBehaviour().Die()
    //          - GameManager


    // EVENTS : updateCost     -->     ShopCostHelper.MoneyIsBuyed()
    //          - ButtonsCostRefresh

    // EVENTS : confirmEdit     -->     UiMethod.ExitFromEdit()
    //          - GameManager
    //          - InventoryBehaviour
    //          - UiManager
    //          - LevelManager

    // EVENTS : characterLevelUp    -->  ShopCostHelper.CheckCharacterLevel()
    //          - UiMethod

    // EVENTS : setDifficulty    -->  LevelManager.Start().SetLevelDifficulty()
    //          - InteractableObject : - InteractablePiggyBank
    //          - RewardTower
    //          - WallBehaviour      : - WallFireDistance
    //                                 - WallFireRate
}