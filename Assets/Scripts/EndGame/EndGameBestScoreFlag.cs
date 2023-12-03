using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndGameBestScoreFlag : MonoBehaviour
{
    [Header("Local References")]
    public ParticleSystem bestScoreParticle;

    [Header("Private Variables")]
    private float currentBestScore;
    private bool bestScoreBeated;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("FlagBestScore"))
            currentBestScore = PlayerPrefs.GetFloat("FlagBestScore");
        else
        {
            currentBestScore = 0;
            PlayerPrefs.SetFloat("FlagBestScore", currentBestScore);
        }

        if (currentBestScore <= 10)
            transform.DOScaleY(0, 0);

        else
        {
            transform.localPosition = new Vector3(0, 0, currentBestScore);
        }

        bestScoreBeated = false;
    }

    private void Update()
    {
        if (GameManager.instance.IsInEndGameStatus())
        {
            CheckForPlayerPos();
        }
    }

    private void CheckForPlayerPos()
    {
        if(EndGameCharacterBehaviour.instance.transform.localPosition.z > transform.localPosition.z)
        {
            transform.DOLocalMoveZ (EndGameCharacterBehaviour.instance.transform.localPosition.z , 0.1f);
        }

        SaveNewPos();
    }

    private void SaveNewPos()
    {
        if(transform.localPosition.z > currentBestScore)
        {
            currentBestScore = transform.localPosition.z;
            PlayerPrefs.SetFloat("FlagBestScore", currentBestScore);
            
            if(transform.localScale.y == 0)
            {
                transform.DOScaleY(1, 1f)
                    .SetEase(Ease.OutBack);
            }

            if (!bestScoreBeated)
            {
                if (bestScoreParticle != null)
                {
                    bestScoreParticle.Play();
                }

                bestScoreBeated = true;
            }
        }       
    }
}