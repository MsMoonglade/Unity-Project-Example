using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMoney : WallBehaviour
{
    private void Awake()
    {
        base.Awake();
    }

    public override void Take()
    {
        base.Take();

        GameManager.instance.IncreaseGold(startVariable);
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
}