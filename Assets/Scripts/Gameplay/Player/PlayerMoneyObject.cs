using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerMoneyObject : MonoBehaviour
{
    [Header("Variables")]
    public float spreadShootRotOffset;

    [Header("Local References")]
    public MeshRenderer renderer;

    [Header("Private Variables")]
    [HideInInspector]
    public int value;

    private void Awake()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Setup(int val)
    {        
        if(val == 0)
        {
            CharacterBehaviour.instance.playerMoneyObjects.Remove(this.GetComponent<PlayerMoneyObject>());
            Destroy(this.gameObject);
        }       

        value = val;
        SetMaterial(value);
    }

    public void Shoot(bool i_spread)
    {
        //Normal one Shoot
        if (!i_spread)
        {
            Vector3 pos = new Vector3(transform.position.x, 0.5f, transform.position.z);
            GameObject bullet = PoolUtilities.instance.GetPooledItem(CharacterBehaviour.instance.bullet_Pref);
            bullet.transform.position = pos;

            bullet.GetComponent<BulletBehaviour>().SetValue(value);
        }

        //Spreaded Shoot
        else
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 pos = new Vector3(transform.position.x, 0.5f, transform.position.z);
                Vector3 rot = Vector3.zero;

                if (i % 2 == 0)
                    rot = new Vector3(0, spreadShootRotOffset, 0);
                else
                    rot = new Vector3(0, -spreadShootRotOffset, 0);


                GameObject bullet = PoolUtilities.instance.GetPooledItem(CharacterBehaviour.instance.bullet_Pref);
                
                if (bullet != null)
                {
                    bullet.transform.position = pos;
                    bullet.transform.DORotate(rot, 0);

                }

                bullet.GetComponent<BulletBehaviour>().SetValue(value);
            }
        }
    }

    public void IncreaseValue()
    {
        value++;
        
        if(value > 10)
            value = 10;

        TweenScale(1.2f , 0.5f);

        SetMaterial(value);
    }

    public void DecreaseValue()
    {
        value--;

        TweenScale(0.8f, 0.5f);

        SetMaterial(value);
    }

    private void TweenScale(float i_scaleTarget , float i_scaleSpeed)
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(renderer.transform.DOScale(i_scaleTarget, i_scaleSpeed))
            .SetEase(Ease.OutBack);
        mySequence.Append(renderer.transform.DOScale(1, i_scaleSpeed))
            .SetEase(Ease.OutBack);
    }

    public void SetMaterial(int val)
    {
        Material[] mats = new Material[2];

        mats[0] = ColorUtilities.instance.GetIndexSideMaterial(val);
        mats[1] = ColorUtilities.instance.GetEditObjectMaterial(val);

        renderer.materials = mats;
    }
}
