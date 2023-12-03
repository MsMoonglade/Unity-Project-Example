using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;

public class EndGameIndividualCharacter : MonoBehaviour
{
    [Header("Variables")]
    public float fireRate;

    [Header("Project References")]
    public GameObject endGameBulletPrefs;

    [Header("Local References")]
    public GameObject spawnbulletPoint;
    public GameObject model;
    public ParticleSystem particle;

    [Header("Private References")]
    private Coroutine shootCoroutine;

    [Header("Private Variables")]
    [HideInInspector]
    public bool active;

    public void Setup(float i_activationSpeed)
    {
        shootCoroutine = null;
        active = true;

        particle.Play();

        transform.DOLocalRotate(new Vector3(0, 360, 0), i_activationSpeed, RotateMode.FastBeyond360)
               .SetRelative(true)
               .SetEase(Ease.OutBack)
               .OnComplete(() => transform.localRotation = new Quaternion(0 , 0 , 0 , 0));

        transform.DOScale(1, i_activationSpeed)
            .SetEase(Ease.OutBack);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("EndGameObstacles"))
        {
            Fall();
        }
    }

    public void StartShoot()
    {
        if(shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(ShootCoroutine());
        }
    }

    public void Fall()
    {
        transform.SetParent(null, true);
        EndGameCharacterBehaviour.instance.individualCharacters.Remove(this);
        EndGameCharacterBehaviour.instance.LostIndividualCharacters(this);

        active = false;

        transform.DOPause();
        transform.DOKill();

        if(shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        float randomizer = Random.Range(0, 1f);
        float randomizedTime = Random.Range(0.5f, 0.8f);

        if(randomizer < 0.5f)       
            model.transform.DOLocalRotate(new Vector3(0, 0, -90), randomizedTime);
        else
            model.transform.DOLocalRotate(new Vector3(0, 0, 90), randomizedTime);

        model.transform.DOLocalMoveY(-1, randomizedTime)
             .SetEase(Ease.InBack);
    }

    public void Rotate(bool i_right , float i_rotAmount , float i_rotSpeed)
    {
        if (active)
        {
            if (i_right)
                model.transform.DORotate(new Vector3(0, 0, -i_rotAmount), i_rotSpeed);
            else
                model.transform.DORotate(new Vector3(0, 0, i_rotAmount), i_rotSpeed);
        }
    }

    private IEnumerator ShootCoroutine()
    {
        while (active)
        {       
            float posRandomizer = 0.5f;
            Vector3 pos = spawnbulletPoint.transform.position + new Vector3(Random.Range(-posRandomizer , posRandomizer), 0, 0);
            
            GameObject bullet = PoolUtilities.instance.GetPooledItem(endGameBulletPrefs);
            bullet.transform.position = pos;
            bullet.GetComponent<EndGameBulletBehaviour>().StartMove();

            yield return new WaitForSeconds(fireRate);
        }
    }
}
