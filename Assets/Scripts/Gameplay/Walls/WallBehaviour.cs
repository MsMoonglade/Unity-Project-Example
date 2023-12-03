using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Lofelt.NiceVibrations;
using UnityEditor;

public class WallBehaviour : MonoBehaviour
{
    [Header("Variables")]
    public int startVariable;
    public int minStartVariable;
    public int maxStartVariable;

    [Header("Base Not Edit Variables")]
    public float nearWallCheck_Range;
    public LayerMask nearWallCheck_Mask;
    public bool isNegative;

    [Header("Base LocalReference")]
    public TMP_Text dynamicText;
    public MeshRenderer[] sideElementMeshRenderer;
    
    public Material positive_Side_Mat;    

    public MeshRenderer centerWallMeshRenderer;
    public Material positive_Center_Mat;

    [Header("Private Local Variables")]

    private List<Collider> nearWall = new List<Collider>();

    private bool activated;

    protected virtual void Awake()
    {
        UpdateUi();

        CheckNearWall();

        activated = false;
    }
    
    public virtual void Take()
    {
        foreach (var c in nearWall)
        {
            c.enabled = false;

            c.transform.gameObject.GetComponent<WallBehaviour>().DisableAnimation();
            c.GetComponent<WallBehaviour>().activated = true;
        }
    }

    public virtual void TakeHit(int i_value)
    {
        
    }

    private void CheckNearWall()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, nearWallCheck_Range , nearWallCheck_Mask);
       
        foreach (var hitCollider in hitColliders)
        {
            nearWall.Add(hitCollider);
        }
    }

    private void DisableAnimation()
    {
        if (!activated)
        {
            transform.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear);

            transform.DOScale(new Vector3(0, 0, 0), 0.2f)
                .SetEase(Ease.InBack);
        }
    }
   
    protected void SetToPositive()
    {
        isNegative = false;

        transform.DOLocalRotate(new Vector3(0, 360 , 0), 0.2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);      
           
        foreach (MeshRenderer rend in sideElementMeshRenderer)
        {
            rend.material = positive_Side_Mat;
        }

        centerWallMeshRenderer.material = positive_Center_Mat;
    }

    protected virtual void SetStartVariable()
    {
        if (!isNegative)
        {
            startVariable = Random.Range(minStartVariable, maxStartVariable);
        }
    }

    protected virtual void UpdateUi()
    {       
        TweenTextScale();
    }

    private void TweenTextScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(dynamicText.transform.DOScale(1.3f, 0.16f));
        mySequence.Append(dynamicText.transform.DOScale(1, 0.16f));
    }

    protected virtual void OnSetDifficulty(object sender)
    {

    }
}