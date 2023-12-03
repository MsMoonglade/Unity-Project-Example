using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    void Awake()
    {
       // CheckNewGameVersion();

        DOTween.Init();

        SceneManager.LoadScene(1);
    }

    /*
    void CheckNewGameVersion()
    {
        if (PlayerPrefs.HasKey("Build_1.2.0"))
            return;

        else
        {
            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt("Build_1.2.0", 1);
        }
    }
    */
}