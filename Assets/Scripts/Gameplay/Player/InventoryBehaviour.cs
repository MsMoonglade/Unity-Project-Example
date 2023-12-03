using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBehaviour : MonoBehaviour
{  
    public static InventoryBehaviour Instance;

    [Header("Variables")]
    public float timeToDisableAfterEdit;

    [Header("Project References")]
    [Header("Private Variables")]

    [Header("Not Edit Variables")]
    public LayerMask moneyLayerMask;

    [Header("Local References")]
    public GameObject moneyParent;
    public Image editUiBg;
    public GameObject model;
    public GameObject binObject;

    [Header("Private References")]
    private InventoryGridGenerator grid;
    
    [HideInInspector]
    public List<int> gridElementValue = new List<int>();
    [HideInInspector]
    public List<Vector3> gridElementPos = new List<Vector3>();

    private void OnEnable()
    {
        EventManager.StartListening(Events.confirmEdit, OnExitEditView);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.confirmEdit, OnExitEditView);
    }

    private void Awake()
    {
        Instance = this;

        grid = GetComponent<InventoryGridGenerator>();
    }

    private void Start()
    {
        LoadInvValue();
        SpawnCorrectMoneyElement();
    }

    //Used in Drag manager for check if money is lefted in inv or player
    public Transform ReturnMovingElementParent(GameObject slot)
    {
        foreach (GameObject g in grid.currentGridElement)
        {
            if(slot.gameObject == g)
                return moneyParent.transform;
        }        

        return CharacterBehaviour.instance.editObjectParent.transform;
    }

    public bool HaveFreeSlot()
    {
        int gridElementQuantity = grid.gridXSize * grid.gridYSize;

        if (moneyParent.transform.childCount < gridElementQuantity)
            return true;
        else 
            return false;
    }

    public GameObject ReturnFirstFreePos()
    {
        foreach(GameObject slot in grid.currentGridElement)
        {
            RaycastHit hit;
            Vector3 pos = slot.transform.position + slot.transform.TransformDirection(Vector3.up) * 3;

            if (!Physics.Raycast(pos, slot.transform.TransformDirection(Vector3.down), out hit, 5, moneyLayerMask))
            {
                return slot;
            }
        }

        return null;
    }

    public void GenerateMoneyInInv(int i_newMoneyValue)
    {
        if (HaveFreeSlot())
        {
            Vector3 pos = ReturnFirstFreePos().transform.localPosition;

            int myValue = i_newMoneyValue;

            gridElementPos.Add(pos);
            gridElementValue.Add(myValue);

            GameObject edit = Instantiate(CharacterBehaviour.instance.editObject, pos, new Quaternion(0,0,0,0) , moneyParent.transform);
            edit.transform.localPosition = pos;

            edit.transform.DOScale(0, 0);
            edit.transform.DOScale(1.25f, 0.25f)
                .SetEase(Ease.OutBack);           

            edit.GetComponent<PlayerMoneyObject>().Setup(myValue);
        
            SaveInvValue();
        }
    }

    private void OnExitEditView(object sender)
    {
        editUiBg.DOFade(0, timeToDisableAfterEdit);

        moneyParent.transform.DOScale(0, timeToDisableAfterEdit);
        model.transform.DOScale(0, timeToDisableAfterEdit);
        binObject.transform.DOScale(0, timeToDisableAfterEdit)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    public void SaveInvValue()
    {
        int[] valueToSave = new int[moneyParent.transform.childCount];
        Vector3[] posToSave = new Vector3[moneyParent.transform.childCount];

        for (int i = 0; i < moneyParent.transform.childCount; i++)
        {
            valueToSave[i] = moneyParent.transform.GetChild(i).GetComponent<PlayerMoneyObject>().value;
            posToSave[i] = moneyParent.transform.GetChild(i).transform.localPosition;
        }

        PlayerPrefs.DeleteKey("SavedInvValue");
        PlayerPrefs.SetString("SavedInvValue", string.Join("###", valueToSave));

        string posToSaveString = SerializeVector3Array(posToSave);
        PlayerPrefs.DeleteKey("SavedInvPos");
        PlayerPrefs.SetString("SavedInvPos", posToSaveString);
    }

    public void LoadInvValue()
    {
        if (PlayerPrefs.HasKey("SavedInvValue") && PlayerPrefs.HasKey("SavedInvPos"))
        {
            //LoadValue
            string[] tempValue = PlayerPrefs.GetString("SavedInvValue").Split(new[] { "###" }, StringSplitOptions.None);

            if (tempValue[0] != "")
            {
                if (tempValue.Length >= 1)
                    for (int i = 0; i < tempValue.Length; i++)
                    {
                        gridElementValue.Add(int.Parse(tempValue[i]));
                    }

                //LoadPos
                string posStringNotSplitted = PlayerPrefs.GetString("SavedInvPos");
                Vector3[] allPosSplitted = DeserializeVector3Array(posStringNotSplitted);
                if (allPosSplitted.Length >= 1)
                {
                    for (int i = 0; i < allPosSplitted.Length; i++)
                    {
                        gridElementPos.Add(allPosSplitted[i]);
                    }
                }
            }
        }
    }

    private void SpawnCorrectMoneyElement()
    {
        //Take position saved and load relative Money and load it with relative value
        foreach (GameObject o in grid.currentGridElement)
        {
            int myValue = 0;

            for (int i = 0; i < gridElementPos.Count; i++)
            {
                if (o.transform.localPosition == gridElementPos[i])
                {
                    myValue = gridElementValue[i];
                    break;
                }
            }

            GameObject edit = Instantiate(CharacterBehaviour.instance.editObject, o.transform.position, o.transform.rotation, moneyParent.transform);
            edit.transform.localPosition = o.transform.localPosition;
            //edit.transform.DOScale(1.25f, 0f);
            edit.GetComponent<PlayerMoneyObject>().Setup(myValue);

            /*
            //BG
            GameObject bg = Instantiate(CharacterBehaviour.instance.editBG, o.transform.localPosition, Quaternion.identity, inventoryBgParent.transform);
            bg.transform.localPosition = o.transform.localPosition + new Vector3(0, -0.03f, 0);
            bg.transform.rotation = new Quaternion(0, 0, 0, 0);
            */
        }
    }

    public static string SerializeVector3Array(Vector3[] aVectors)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 v in aVectors)
        {
            sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
        }
        if (sb.Length > 0) // remove last "|"
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    public static Vector3[] DeserializeVector3Array(string aData)
    {
        string[] vectors = aData.Split('|');
        Vector3[] result = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(' ');
            if (values.Length != 3)
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            result[i] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        return result;
    }    
}