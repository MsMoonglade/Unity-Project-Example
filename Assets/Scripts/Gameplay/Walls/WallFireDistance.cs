using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WallFireDistance : WallBehaviour
{
    [Header("Not Edit Variables")]
    public float multiplyDistanceRate;

    private void OnEnable()
    {
        EventManager.StartListening(Events.setDifficulty, OnSetDifficulty);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.setDifficulty, OnSetDifficulty);
    }

    private void Awake()
    {
        base.Awake();
    }

    public override void Take()
    {
        base.Take();

        float localAmountToIncrease = startVariable * multiplyDistanceRate;

        CharacterBehaviour.instance.bulletActiveTime += localAmountToIncrease;

        if (localAmountToIncrease > 0)
            CharacterBehaviour.instance.powerUpParticle.Play();
        else
            CharacterBehaviour.instance.powerDownParticle.Play();
    }

    public override void TakeHit(int i_value)
    {
        startVariable++;

        if (startVariable == 0)
            startVariable = 1;

        UpdateUi();
        
        if (startVariable > 0 && isNegative)
        {
            SetToPositive();
            isNegative = false;
        }
    }

    protected override void UpdateUi()
    {
        base.UpdateUi();

        if (startVariable > -1)
            dynamicText.text = "+" + startVariable.ToString();
        else
            dynamicText.text = startVariable.ToString();
    }

    protected override void OnSetDifficulty(object sender)
    {
        base.OnSetDifficulty(sender);

        if(isNegative)
            startVariable = -DifficultyManager.instance.WallDifficulty(GameManager.instance.CurrentLevel);
        else
            SetStartVariable();

        UpdateUi();
    }
}