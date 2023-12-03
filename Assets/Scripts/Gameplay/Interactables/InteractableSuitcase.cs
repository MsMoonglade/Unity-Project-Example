using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DamageNumbersPro;

public class InteractableSuitcase : InteractableObject
{
    [Header("Variables")]
    public int gridXSize;
    public int gridZSize;
    public int gridHeight;

    public float gridXSpacing;
    public float gridZSpacing;

    public float takenMoneySize;

    [Header("Scene References")]
    public DamageNumber textDamageNumber;

    [Header("Local References")]
    public GameObject suitcaseTopElement;
    public ParticleSystem completeParticle;

    [Header("Private Variables")]
    public List<Vector3> slotPos;
    public List<bool> slotTaken;
    private int totalSlotAmount;
    private int currentUsedSlot;

    private Vector3 startPos;

    private Coroutine checkCompletecoroutine;

    [Header("Used Anywhere Else")]
    [HideInInspector]
    public int localMoneyValue;

    private void OnEnable()
    {
        startPos = new Vector3(-((gridXSize-1) * gridXSpacing/2) , 0.75f , -((gridZSize - 1) * gridZSpacing / 2));

        totalSlotAmount = 0;    
        currentUsedSlot = 0;

        checkCompletecoroutine = null;

        GenerateGridObj();   

        UpdateUi();
    }

    private void Start()
    {
        textDamageNumber = TextDamageProReferences.instance.interactable_Suitcase_Text;
    }

    private void Update()
    {
        if(CharacterBehaviour.instance.transform.position.z >= transform.position.z)
        {
            Complete(false);
        }
    }

    public override void TakeHit(GameObject i_object)
    {
        if (!isCompleted)
        {
            base.TakeHit(i_object);

            Vector3 moneyDestination = ReturnFirstFreeSlot();
          
            if (moneyDestination != new Vector3(100, 100, 100))
            {
                localMoneyValue += i_object.GetComponent<BulletBehaviour>().value;

                Destroy(i_object.GetComponent<BulletBehaviour>());
                Destroy(i_object.GetComponent<Rigidbody>());
                Destroy(i_object.GetComponent<BoxCollider>());

                i_object.transform.SetParent(transform, true);
                i_object.transform.DOLocalJump(moneyDestination , 3 , 1, 0.5f)
                    .OnComplete(() => CheckComplete());
                i_object.transform.DOScale(new Vector3(takenMoneySize, i_object.transform.localScale.y, takenMoneySize), 0.5f);
                i_object.transform.DORotate(new Vector3(0, 90, 0), 0.5f);

                currentUsedSlot++;
            }

            UpdateUi();
        }
    }

    private void CheckComplete()
    {
        if(checkCompletecoroutine != null)
        {
            StopCoroutine(checkCompletecoroutine);
            checkCompletecoroutine = StartCoroutine(CheckCompleteCoroutine());
        }    

        if(checkCompletecoroutine == null) 
        {
            checkCompletecoroutine = StartCoroutine(CheckCompleteCoroutine());
        }
    }

    private Vector3 ReturnFirstFreeSlot()
    {
        for(int i = 0; i < slotTaken.Count; i++)
        {
            if (!slotTaken[i])
            {
                slotTaken[i] = true;
                return slotPos[i];
            }
        }

        return new Vector3(100 , 100 , 100);
    }

    private void GenerateGridObj()
    {
        for(int x = 0; x < gridXSize; x++)
        {
            for (int z = 0; z < gridZSize; z++)
            {
                bool newBool = false;
                slotTaken.Add(newBool);

                Vector3 newPos = startPos;
                slotPos.Add(newPos);

                startPos += new Vector3(0, 0, gridZSpacing);

                totalSlotAmount++;
            }

            startPos += new Vector3(gridXSpacing, 0, -(gridZSize * gridZSpacing));
        }

        totalSlotAmount *= gridHeight;
    }

    protected override void UpdateUi()
    {
        if (currentUsedSlot <= totalSlotAmount)
        {
            base.UpdateUi();

            dynamicText.text = currentUsedSlot.ToString() + "/" + totalSlotAmount.ToString();
        }
    }

    //Called in Parent (Complete)
    protected override void DisableAnimation()
    {
        base.DisableAnimation();

        dynamicText.transform.parent.DOScale(0, 0.5f);

        Vector3 nearestRollerPos = new Vector3(WorldRollerReferences.instance.ReturnRollerPos().x, transform.localPosition.y, transform.localPosition.z + 5 + Random.Range(1f , 5f));

        Sequence disableSequence = DOTween.Sequence();

        disableSequence.Append(suitcaseTopElement.transform.DOLocalRotate(new Vector3(-90, 90, -90), 0.3f));            
       
        disableSequence.PrependInterval(0.05f);
        
        disableSequence.Append(transform.DOLocalJump(nearestRollerPos, 2 , 1, 0.4f)
            .OnComplete(() => WorldRollerReferences.instance.TakeObject(this.gameObject)));

        Sequence rotSequence = DOTween.Sequence();
        rotSequence.PrependInterval(0.35f);
        rotSequence.Append(transform.DOLocalRotate(new Vector3(0, -90, 0), 0.4f));
    }

    protected override void CompletedAnimation()
    {
        base.CompletedAnimation();

        textDamageNumber.Spawn(transform.position);

        dynamicText.transform.parent.DOScale(0, 0.5f);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(1.2f, 0.25f));
        mySequence.Append(transform.DOScale(1, 0.25f));

        Vector3 nearestRollerPos = new Vector3(WorldRollerReferences.instance.ReturnRollerPos().x, transform.localPosition.y, transform.localPosition.z + 5 + Random.Range(1f, 5f));

        Sequence disableSequence = DOTween.Sequence();

        disableSequence.Append(suitcaseTopElement.transform.DOLocalRotate(new Vector3(-90, 90, -90), 0.3f));

        disableSequence.PrependInterval(0.05f);

        disableSequence.Append(transform.DOLocalJump(nearestRollerPos, 2, 1, 0.4f)
            .OnComplete(() => WorldRollerReferences.instance.TakeObject(this.gameObject)));

        Sequence rotSequence = DOTween.Sequence();
        rotSequence.PrependInterval(0.35f);
        rotSequence.Append(transform.DOLocalRotate(new Vector3(0, -90, 0), 0.4f));
    }

    private IEnumerator CheckCompleteCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        if (currentUsedSlot >= totalSlotAmount)
        {
            currentUsedSlot = totalSlotAmount;            
               
            Complete(true);            
        }

        checkCompletecoroutine = null;
    }
}