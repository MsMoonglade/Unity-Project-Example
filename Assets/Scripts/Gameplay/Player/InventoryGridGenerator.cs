using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGridGenerator : MonoBehaviour
{
    [Header("Variables")]
    public int gridXSize;
    public int gridYSize;
    public float gridXSpacing;
    public float gridYSpacing;

    [Header("Local References")]
    public GameObject gridParent;

    [Header("Project References")]
    public GameObject invGridSlotPrefs;

    [Header("Private Variables")]
    [HideInInspector]
    public List<GameObject> currentGridElement = new List<GameObject>();

    private void Awake()
    {
        GenerateSlot();
    }
    
    private void GenerateSlot()
    {              
        Vector3 startPos = Vector3.zero;
        ;
        for (int i = 0; i < gridXSize; i++)
        {
            for (int j = 0; j < gridYSize; j++)
            {
                GameObject slot = Instantiate(invGridSlotPrefs, startPos, Quaternion.identity , gridParent.transform);
                slot.transform.localPosition = startPos;
                slot.transform.rotation = new Quaternion(0, 0, 0, 0);

                currentGridElement.Add(slot);

                startPos += new Vector3(0, 0, CharacterBehaviour.instance.moneyDecalScaleY + gridYSpacing);
            }

            startPos += new Vector3(CharacterBehaviour.instance.moneyDecalScaleX + gridXSpacing, 0, (-gridYSize * (CharacterBehaviour.instance.moneyDecalScaleY + gridYSpacing )));
        }
    }    
}
