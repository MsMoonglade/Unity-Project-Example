using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Check For Enable Level Generation")]
    public bool GENERATE;
    public float wallPercentageSpawnRate;

    [Header("Variables")]
    public float gameplayElement_StartPosOffset;
    public float gameplayElement_DistanceOffset;
    public float endElement_DistanceOffset;
    public AnimationCurve levelLenght;

    [Header("Not Edit Variables")]

    [Header("Local References")]
    public GameObject wallParent;
    public GameObject rewardTowerParent;
    public GameObject interactableParent;
    public GameObject mixedParent;
    public GameObject worldFloorObject;

    [Header("Project References")]
    public GameObject[] possibleWall;
    public GameObject[] possibleRewardTower;
    public GameObject[] possibleInteractableElement;
    public GameObject[] possibleMixedElement;

    [Header("Private Variables")]
    private Vector3 elementPosition;

    [Header("Private References")]
    private DifficultyManager difficultyManager;

    private void OnEnable()
    {
        EventManager.StartListening(Events.confirmEdit, ShowLevel);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.confirmEdit, ShowLevel);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        difficultyManager = GetComponent<DifficultyManager>();

        if (GENERATE)
        {
            GenerateLevel();
          
            HideLevel();
            SetLevelDifficulty();

            SpecialWallCheck();
        }
    }

    private void GenerateLevel()
    {
        float levelLenght = EvaluateLevelLenght(GameManager.instance.CurrentLevel);
        int levelPieces = (int)(levelLenght / gameplayElement_DistanceOffset);

        elementPosition = Vector3.zero;
        elementPosition += new Vector3(0, 0, gameplayElement_StartPosOffset);

        for (int i = 0; i < levelPieces; i++)
        {
            //GENERATE WALL
            float generateWall = Random.Range(0f, 1f);
            if(generateWall <= wallPercentageSpawnRate)
            {
                GameObject element = GenerateRandomElement(possibleWall, wallParent);
                element.transform.position = elementPosition;
            }

            //GENERATE OTHER GAMEPLAY ELEMENT
            else
            {
                float randomizer = Random.Range(0.0f, 1.0f);

                if (randomizer >=0 && randomizer < 0.35f)
                {
                    GameObject element = GenerateRandomElement(possibleRewardTower, rewardTowerParent);
                    element.transform.position = elementPosition;
                }

                else if (randomizer >= 0.35f && randomizer <= 0.7f)
                {
                    GameObject element = GenerateRandomElement(possibleInteractableElement, interactableParent);
                    element.transform.position = elementPosition;
                }

                else if (randomizer >= 0.7f && randomizer <= 1)
                {
                    GameObject element = GenerateRandomElement(possibleMixedElement, mixedParent);
                    element.transform.position = elementPosition;
                }
            }            

            elementPosition += new Vector3(0, 0, gameplayElement_DistanceOffset);
        }

        elementPosition += new Vector3(0, 0, endElement_DistanceOffset);

        worldFloorObject.transform.position = elementPosition;        
    }

    private void SpecialWallCheck()
    {
        //Have max of one spreadshot in scene
        var spreadShotInScene = FindObjectsOfType<WallSpreadShoot>();

        int wallToMantain = Random.Range(0, spreadShotInScene.Length);

        for(int i = 0; i < spreadShotInScene.Length; i++)
        {
            if (i != wallToMantain)
            {
                Destroy(spreadShotInScene[i].gameObject);
            }
        }
    }

    private GameObject GenerateRandomElement(GameObject[] possiblePool , GameObject parent)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, parent.transform);
        return o;
    }


    private void SetLevelDifficulty()
    {
        EventManager.TriggerEvent(Events.setDifficulty);

        /*
        float levelDifficulty = (float)difficultyManager.currentDifficulty;

        
       RewardTowerElement[] tower = GetComponentsInChildren<RewardTowerElement>();
        
        if (tower.Length != 0 )
        {
            foreach (RewardTowerElement t in tower)
            {
                t.value = (int)(levelDifficulty + Random.Range(0, levelDifficulty / 2f));
                
                if (t.value < 2)
                    t.value = 2;                

                float randomizer = Random.Range(0.0f, 1.0f);

                if (randomizer <= 0.075f)
                    t.value *= 5;

                else if (randomizer > 0.075f && randomizer <= 0.135f)
                    t.value = (int)(t.value * 3f);

                else if (randomizer > 0.135f && randomizer <= 0.2f)
                    t.value = (int)(t.value * 1.5f);

                levelDifficulty *= 1.05f;
            }
        }
        */
    }


    private void HideLevel()
    {
        wallParent.transform.DOScaleY(0, 0);
        rewardTowerParent.transform.DOScaleY(0, 0);
        interactableParent.transform.DOScaleY(0, 0);
        mixedParent.transform.DOScaleY(0, 0);
    }

    public void ShowLevel(object sender)
    {
        wallParent.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBack);
        rewardTowerParent.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBack);
        interactableParent.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBack);
        mixedParent.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBack);
    }

    public float EvaluateLevelLenght(int currentLevel)
    {
        int fixedLevel = 0;

        if (currentLevel < 25)
        {
            fixedLevel = currentLevel;
        }

        else
            fixedLevel = 25;

        float lenght = levelLenght.Evaluate(fixedLevel);
        return lenght;
    }
}