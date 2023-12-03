using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class TextDamageProReferences : MonoBehaviour
{
    public static TextDamageProReferences instance;

    [Header("Local References")]
    public DamageNumber interactable_Suitcase_Text;
    public DamageNumber Rewardtower_Item_Text;

    private void Awake()
    {
        instance = this;    
    }
}
