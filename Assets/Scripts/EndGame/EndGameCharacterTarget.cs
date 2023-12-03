using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EndGameCharacterTarget : MonoBehaviour
{
    public GameObject endGameChar;
    public List<EndGameIndividualCharacter> singleCharacters = new List<EndGameIndividualCharacter>();

    private void FixedUpdate()
    {
        Repositon();
    }

    private void Repositon()
    {
        if (singleCharacters.Count > 0)
        {
            transform.position = Vector3.Lerp(transform.position, FindCenterPoint(), Time.deltaTime * 100);

            //var step = EndGameCharacterBehaviour.instance.moveSpeed;           
            //transform.transla = Vector3.MoveTowards(transform.position, FindCenterPoint(), step);
        }    
    }

    private Vector3 FindCenterPoint()
    {
        Vector3 o_middlePos = Vector3.zero;

        var bounds = new Bounds(singleCharacters[0].transform.position, Vector3.zero);
        for (var i = 1; i < singleCharacters.Count; i++) 
        {
            bounds.Encapsulate(singleCharacters[i].transform.position);
        }

        return bounds.center;
    }
}