using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    public static DragManager instance;

    [Header("Not Edit Variables")]
    public LayerMask draggableMask;
    public LayerMask possiblePositionMask;
    public LayerMask inputDetectMask;

    [Header("Private References")]
    //Main Drag References
    private bool dragging = false;
    private Transform objectDrag;
    private Vector3 startPos;
    private Quaternion startRot;

    
    [Header("Private Variables")]
    //When DraggingObject
    Ray rayPos;
    RaycastHit hitObj;
    Transform targetPos = null;
    private bool haveTarget = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!GameManager.instance.IsInGameStatus() && GameManager.instance.inEdit)
        {
            //IF DRAGGING AND DETECT A SLOT lock in it   
            if (dragging)
            {
                MoveCurrentDraggingObject();              
            }

            //Reset Dragging if have Multiple/none touch
            if (Input.touchCount != 1)
            {
                dragging = false;
                return;
            }

            //base touch input
            Touch touch = Input.touches[0];
            Vector3 pos = touch.position;

            //Detect under initial touches
            if (touch.phase == TouchPhase.Began)
            {
                //check if start touch on DRAGGABLE Objects
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Mathf.Infinity, draggableMask);

                if (hit)
                {
                    //SET DRAGGABLE OBJECT
                    if (hitInfo.collider.transform.CompareTag("Draggable"))
                    {
                        objectDrag = hitInfo.collider.transform;
                        objectDrag.GetComponent<Collider>().enabled = false;
                        startPos = objectDrag.position;
                        startRot = objectDrag.rotation;
                        dragging = true;
                    }
                }
            }

            //ACTUAL DRAG
            if (dragging && touch.phase == TouchPhase.Moved)
            {
                //check if dragging on INPUTDETECT Layer
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, inputDetectMask);
                if (hit)
                {
                    objectDrag.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                    objectDrag.transform.rotation = hitInfo.transform.rotation;
                }
            }

            //END DRAG
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                rayPos = Camera.main.ScreenPointToRay(Input.mousePosition);

                //END INPUT IN GRID SLOT AND DONT MERGE
                if (haveTarget && targetPos != null && GridSlotFree() && !CheckCanMerge())
                {
                    objectDrag.transform.parent = InventoryBehaviour.Instance.ReturnMovingElementParent(targetPos.gameObject);

                    //objectDrag.localPosition = targetPos.localPosition;
                    objectDrag.DOLocalMove(targetPos.localPosition, 0.25f)
                        .SetEase(Ease.OutBack);
                    objectDrag.transform.rotation = targetPos.transform.rotation;
                   
                    objectDrag.GetComponent<Collider>().enabled = true;

                    if (objectDrag.transform.parent == InventoryBehaviour.Instance.moneyParent.transform)
                    {
                        if (CharacterBehaviour.instance.playerMoneyObjects.Contains(objectDrag.GetComponent<PlayerMoneyObject>()))
                        {
                            CharacterBehaviour.instance.playerMoneyObjects.Remove(objectDrag.GetComponent<PlayerMoneyObject>());
                        }

                        objectDrag.transform.DOScale(1.25f, 0.25f);
                    }

                    else
                    {
                        if (!CharacterBehaviour.instance.playerMoneyObjects.Contains(objectDrag.GetComponent<PlayerMoneyObject>()))
                        {
                            CharacterBehaviour.instance.playerMoneyObjects.Add(objectDrag.GetComponent<PlayerMoneyObject>());
                        }

                        objectDrag.transform.DOScale(1, 0.25f);
                    }

                    dragging = false;
                }
                
                //END INPUT MERGE
                else if (haveTarget &&
                    targetPos != null &&                    
                    CheckCanMerge())
                {
                    GameObject mergedObj = ReturnNeedToMergeObject();

                    //increase old element value
                    mergedObj.transform.GetComponent<PlayerMoneyObject>().IncreaseValue();

                    //destroy moving one
                    CharacterBehaviour.instance.playerMoneyObjects.Remove(objectDrag.GetComponent<PlayerMoneyObject>());
                    Destroy(objectDrag.gameObject);
                    dragging = false;
                }                
                
                //END INPUT AND RETURN
                else
                {
                    //On BIN
                    if (ReleaseOnBin())
                    {                        
                        CharacterBehaviour.instance.playerMoneyObjects.Remove(objectDrag.GetComponent<PlayerMoneyObject>());
                        Destroy(objectDrag.gameObject);
                        dragging = false;
                    }

                    //Return To ORIGIN
                    else 
                    {
                        objectDrag.position = startPos;
                        objectDrag.rotation = startRot;
                        objectDrag.GetComponent<Collider>().enabled = true;
                        dragging = false;
                    }
                }

                EventManager.TriggerEvent(Events.updateCost);

                StartCoroutine(DelaySave());
            }
        }
    }

    private IEnumerator DelaySave()
    {
        yield return new WaitForSeconds(1);

        CharacterBehaviour.instance.SavePlayerValue();
        InventoryBehaviour.Instance.SaveInvValue();
    }

    private void MoveCurrentDraggingObject()
    {
        rayPos = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayPos, out hitObj, Mathf.Infinity, possiblePositionMask))
        {
            //if dragging object on SLOT
            if (hitObj.collider.transform != null && hitObj.collider.transform.CompareTag("Slot"))
            {
                targetPos = hitObj.collider.transform;
                haveTarget = true;
            }

            //if dragging NOT ON SLOT
            else
            {
                targetPos = null;
                haveTarget = false;
            }
        }

        //if No surface under dragging
        else
        {
            targetPos = null;
            haveTarget = false;
        }
    }

    private bool ReleaseOnBin()
    {
        bool o_OnBin = false;

        rayPos = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayPos, out hitObj, Mathf.Infinity, possiblePositionMask))
        {
            //if dragging object on SLOT
            if (hitObj.collider.transform != null && hitObj.collider.transform.CompareTag("Bin"))
            {
                o_OnBin = true;
            }
        }
     
        return o_OnBin;
    }

    private bool GridSlotFree()
    {
        RaycastHit hitInfo;
        
        bool hit = Physics.Raycast(targetPos.transform.position + new Vector3(0, 2, 0), Vector3.down, out hitInfo, Mathf.Infinity, draggableMask);
        
        if (hit)
        {
            return false;
        }

        return true;
    }
    
    private bool CheckCanMerge()
    {
        bool canMerge = false;

        RaycastHit hitInfo;

        bool hit = Physics.Raycast(targetPos.transform.position + new Vector3(0 , 2 , 0 ) , Vector3.down , out hitInfo, Mathf.Infinity, draggableMask);

        if (hit && hitInfo.collider.transform.GetComponent<PlayerMoneyObject>() != null
            && hitInfo.collider.transform.GetComponent<PlayerMoneyObject>().value == objectDrag.transform.GetComponent<PlayerMoneyObject>().value)
        {
            canMerge = true;
        }

        return canMerge;
    }

    private GameObject ReturnNeedToMergeObject()
    {
        GameObject obj = null;

        RaycastHit hitInfo;

        bool hit = Physics.Raycast(targetPos.transform.position + new Vector3(0, 2, 0), Vector3.down, out hitInfo, Mathf.Infinity, draggableMask);

        if (hit && hitInfo.collider.transform.GetComponent<PlayerMoneyObject>() != null
            && hitInfo.collider.transform.GetComponent<PlayerMoneyObject>().value == objectDrag.transform.GetComponent<PlayerMoneyObject>().value)
        {
            obj = hitInfo.collider.transform.gameObject;
        }

        return obj;
    }   
}
