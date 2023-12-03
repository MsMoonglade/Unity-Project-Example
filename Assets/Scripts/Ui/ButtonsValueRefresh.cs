using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonsValueRefresh : MonoBehaviour
{
    public static ButtonsValueRefresh Instance;

    [Header("_COST_InSceneButtonReferences")]
    public TMP_Text buy_AditionalMoney_Cost_Text;

    [Header("_VALUE_InSceneButtonReferences")]
    public TMP_Text currentBuyedMoney_EditCanvas;
    public TMP_Text currentCharacterValue_Character;
    public TMP_Text characterMaxText_Character;
    public TMP_Text currentBuyedMoneyValue_EditCanvas;

    [Header("_SPRITE_InSceneButtonReferences")]
    public Image buyMoneyButtonImage;
    public Sprite haveGoldImage;
    public Sprite dontHaveGoldimage;

    [Header("_SLIDER_InSceneImageReferences")]
    public Image currentCharacterLevelSlider_Character;

    [Header("_ANIMATION_InSceneObjReferences")]
    public GameObject buyMoneyButton;


    private void OnEnable()
    {
        EventManager.StartListening(Events.updateCost, OnUpdateValueText);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.updateCost , OnUpdateValueText);
    }

    private void Awake()
    {
        Instance = this;

        characterMaxText_Character.DOFade(0, 0);
    }

    private void Start()
    {
        EventManager.TriggerEvent(Events.updateCost);
    }

    private void OnUpdateValueText(object sender) 
    {
        //SHOP COST
        if(buy_AditionalMoney_Cost_Text != null)        
            buy_AditionalMoney_Cost_Text.text = ShopCostHelper.instance.currentMoneyShopCost.ToString();

        //CHARACTER PANEL VALUE
        currentCharacterValue_Character.text = CharacterBehaviour.instance.TotalCharacterValue().ToString() + " $";

        if (ShopCostHelper.instance.ReturnCharacterLevel() != 3)
        {
            float characterExpSliderFillAmount = ShopCostHelper.instance.ReturnCharacterExpProgression();
            currentCharacterLevelSlider_Character.fillAmount = characterExpSliderFillAmount;
        }
        else
        {
            currentCharacterLevelSlider_Character.transform.parent.DOScale(0 , 0);
            characterMaxText_Character.DOFade(1, 0);
        }

        //EDIT PANEL 
        if (ShopCostHelper.instance.ReturnCurentMoneytreshHoldIndex() < 3)
        {
            currentBuyedMoney_EditCanvas.text = ShopCostHelper.instance.ReturnTotalMoneyBuyed().ToString()
                + " / "
                + ShopCostHelper.instance.ReturnCurentMoneytreshHoldValue().ToString();
        }
        else
        {
            currentBuyedMoney_EditCanvas.text = "MAX";
        }

        currentBuyedMoneyValue_EditCanvas.text = ShopCostHelper.instance.newMoneyValue.ToString();

        if (GameManager.instance.HaveGold(ShopCostHelper.instance.currentMoneyShopCost))
            buyMoneyButtonImage.sprite = haveGoldImage;
        else
            buyMoneyButtonImage.sprite = dontHaveGoldimage;

    }

    public void AnimateBuyButton()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(buyMoneyButton.transform.DOScale(0, 0.15f));
        mySequence.Append(buyMoneyButton.transform.DOScale(1, 0.15f));
    }
}