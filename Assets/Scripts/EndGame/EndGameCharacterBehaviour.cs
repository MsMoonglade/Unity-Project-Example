using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EndGameCharacterBehaviour : MonoBehaviour
{
    public static EndGameCharacterBehaviour instance;

    [Header("Variables")]
    public float moveSpeed;
    public float horMoveSpeed;
    public float moveXLimit;
    public float horMoveRotAmount;
    public float individualCharactersActivationSpeed;

    [Header("Not Edit Variables")]   

    [Header("Local References")]
    public GameObject individualCharacterParent;
    public EndGameCharacterTarget charTarger;

    [Header("Project References")]
    public GameObject individualCharacterPref;

    [Header("Private Variables")]
    private bool died;
    private bool moving;
    [HideInInspector]
    public List<EndGameIndividualCharacter> individualCharacters = new List<EndGameIndividualCharacter>();

    private void Awake()
    {
        instance = this;
        moving = false;
        died = false;

        AddIndividualCharacter(true);
    }

    private void Update()
    {
        if (moving)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);


        if(moving && individualCharacters.Count == 0)
        {
            moving = false;

            if (!died)
                Die();
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -moveXLimit, moveXLimit), transform.position.y, transform.position.z);
    }

    public void StartRun()
    {
        moving = true;

        foreach (EndGameIndividualCharacter c in individualCharacters)
        {
            c.StartShoot();
        }
    }

    public void Move(Vector3 direction)
    {
        if (GameManager.instance.IsInEndGameStatus() && moving)
        {
            if (direction.x > 0)
            {
                foreach (EndGameIndividualCharacter c in individualCharacters)
                {
                    c.Rotate(true , horMoveRotAmount , 0.25f);
                }
            }

            else if (direction.x < 0)
            {
                foreach (EndGameIndividualCharacter c in individualCharacters)
                {
                    c.Rotate(false, horMoveRotAmount, 0.25f);
                }
            }

            else
            {
                foreach (EndGameIndividualCharacter c in individualCharacters)
                {
                    c.Rotate(true, 0, 0.25f);
                }
            }

            transform.Translate(direction * Time.deltaTime * horMoveSpeed);
        }
    }

    public void ResetRotAnimation()
    {
        foreach (EndGameIndividualCharacter c in individualCharacters)
        {
            c.Rotate(true, 0, 0.25f);
        }
    }

    public void AddIndividualCharacter(bool i_first)
    {
        float posOffsetX = 2.5f;
        float posOffsetZ = 1.0f;

        float randomizedPosZ = Random.Range(-posOffsetZ, posOffsetZ);
        
        if(Mathf.Abs(randomizedPosZ) < 0.25f)
        {
            randomizedPosZ = 0.25f;

            if(Random.Range(0f , 1f) <= 0.5f)
            {
                randomizedPosZ = -randomizedPosZ;
            }
        }

        Vector3 pos = new Vector3(Random.Range(-posOffsetX, posOffsetX), 0, randomizedPosZ);

        if (i_first)
            pos = Vector3.zero;

        GameObject currentObj = Instantiate(individualCharacterPref, pos, Quaternion.identity, individualCharacterParent.transform);
        currentObj.transform.DOScale(0, 0);
        currentObj.transform.localPosition = pos;
        currentObj.transform.GetComponent<EndGameIndividualCharacter>().Setup(individualCharactersActivationSpeed);
        individualCharacters.Add(currentObj.transform.GetComponent<EndGameIndividualCharacter>());

        charTarger.singleCharacters.Add(currentObj.GetComponent<EndGameIndividualCharacter>());
    }

    public void LostIndividualCharacters(EndGameIndividualCharacter i_character)
    {
        charTarger.singleCharacters.Remove(i_character);
    }

    private void Die()
    {
        died = true;

        EndGameBehaviour.instance.GiveEndGameCoinReward();

        EventManager.TriggerEvent(Events.endGame);
    }
}