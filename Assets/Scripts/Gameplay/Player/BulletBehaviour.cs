using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletBehaviour : MonoBehaviour
{
    [Header ("Variables")]
    public float moveSpeed;

    [Header("Project References")]
    public GameObject disable_Particle;
    
    [Header("Local References")]
    public ParticleSystem print_Particle;

    [HideInInspector]
    public int value;

    [Header("Private References")]
    private MeshRenderer meshRenderer;
    private Collider col;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Wall"))
        {
            col.transform.GetComponent<WallBehaviour>().TakeHit(value);
            Disable();
        }

        if (col.transform.CompareTag("Tower"))
        {
            col.transform.GetComponent<RewardTower>().TakeHit(value);   
            Disable();
        }

        if (col.transform.CompareTag("Interactable"))
        {
            col.transform.GetComponent<InteractableObject>().TakeHit(this.gameObject);
            col.transform.GetComponent<InteractableObject>().TakeHit(value);

            if (col.transform.GetComponent<InteractableObject>().disableBullet)
            {
                Disable();
            }
        }

        if (col.transform.CompareTag("EndMiniGame"))
        {
            StopAllCoroutines();
            transform.DOPause();
            transform.DOKill(); 

            EndGameBehaviour.instance.IncreaseEndGameScore(value , this.gameObject);             
        }
    }

    //called when shooted
    public void Enable()
    {
        this.gameObject.SetActive(true);

        transform.localScale = Vector3.one;

        col.enabled = true;

        StartCoroutine(DisableOnEnable());
        StartCoroutine(MoveForward());
    }

    public void SetValue(int newValue)
    {
        value = newValue;

        //Color the money model
        //position 0 is for side of the model
        //position 1 is for the face of the model
        Material[] mats = new Material[2];
        mats[0] = ColorUtilities.instance.GetIndexSideMaterial(value);
        mats[1] = ColorUtilities.instance.GetEditObjectMaterial(value);

        meshRenderer.materials = mats;

        //Color the print particle
        Color newColor = ColorUtilities.instance.GetIndexColor(value);
        print_Particle.GetComponent<ParticleSystemRenderer>().material.SetColor("_BaseColor", newColor);

        Enable();
    }

    public void Disable()
    {
        col.enabled = false;

        GameObject p = PoolUtilities.instance.GetPooledItem(disable_Particle);
        p.transform.position = transform.position;
        p.GetComponent<ParticleSystemRenderer>().material = ColorUtilities.instance.GetIndexParticleMat(value);
        p.SetActive(true);

        ActualDisable();
    }

    private IEnumerator DisableOnEnable()
    {
        yield return new WaitForSeconds(CharacterBehaviour.instance.bulletActiveTime);  
        
        DisableAnimation();
    }

    //Scale the bullet to 0
    private void DisableAnimation()
    {
        col.enabled = false;

        transform.DOScaleY(0, 0.2f);
        transform.DOScaleX(0, 0.2f)
            .OnComplete(() => ActualDisable());
    }

    //Real Gameobject Disabler
    private void ActualDisable()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        this.gameObject.SetActive(false);
    }

    //Translate object forward
    private IEnumerator MoveForward()
    {
        yield return new WaitForSeconds(0.035f);

        while (this.gameObject.activeInHierarchy)
        {
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}