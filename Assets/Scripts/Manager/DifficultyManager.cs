using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    [Header("Tower Variables")]
    public AnimationCurve towerHp;
    public AnimationCurve towerReward;
    public float towerMinRandomizer;
    public float towerMaxRandomizer;

    [Header("Interactable Variables")]
    public AnimationCurve interactableHp;
    public AnimationCurve interactableReward;
    public float interactableMinRandomizer;
    public float interactableMaxRandomizer;

    [Header("Wall Variables")]
    public AnimationCurve wallDifficulty;
    public float wallMinRandomizer;
    public float wallMaxRandomizer;

    [Header("Wall Variables")]
    public float characterMultiplyer;

    [Header("Local Level Progression(TEMP DISABLED)")]
    public float levelProgressionMultiplyer;

    private void Start()
    {
        instance = this;
    }

    public int TowerHp(int i_level)
    {
        return ReturnValue(towerHp, towerMinRandomizer, towerMaxRandomizer , i_level);
    }

    public int TowerReward(int i_level)
    {
        return ReturnValue(towerReward, towerMinRandomizer, towerMaxRandomizer, i_level);
    }

    public int InteractableHp(int i_level)
    {
        return ReturnValue(interactableHp, interactableMinRandomizer, interactableMaxRandomizer, i_level);
    }

    public int InteractableReward(int i_level)
    {
        return ReturnValue(interactableReward, interactableMinRandomizer, interactableMaxRandomizer, i_level);
    }

    public int WallDifficulty(int i_level)
    {
        return ReturnValue(wallDifficulty, wallMinRandomizer, wallMaxRandomizer, i_level);
    }

    public float CharacterMultiplyer()
    {
        return (float)(CharacterBehaviour.instance.TotalCharacterValue()) * characterMultiplyer;
    }

    public int ReturnValue(AnimationCurve i_curve , float i_minRandomizer , float i_maxRandomizer , int i_level)
    {
        int normalizedLevel = 10;

        if (i_level < 25)
        {
            normalizedLevel = i_level;
        }

        else
            normalizedLevel = 25;

        float o_value = i_curve.Evaluate(normalizedLevel);
        
        float valueRandomizer = Random.Range(i_minRandomizer, i_maxRandomizer);

        o_value *= valueRandomizer;

        return (int)o_value;
    }
}