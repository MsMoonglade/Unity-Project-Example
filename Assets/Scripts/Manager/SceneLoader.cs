using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        int savedLevel = 0;
        //currentLevel set
        if (PlayerPrefs.HasKey("CurrentLevel"))
            savedLevel = PlayerPrefs.GetInt("CurrentLevel") + 1;

        int sceneToLoad = savedLevel ;

        if (savedLevel > SceneCountConverted())
        {
            sceneToLoad = SceneManager.sceneCountInBuildSettings - 1;
        }
        if (sceneToLoad <= 0)
            sceneToLoad = 2;

        StartCoroutine(LoadYourAsyncScene(sceneToLoad));

        //Debug.Log("  saved " + savedLevel + "   scene count  " + SceneCountConverted() + " load "+ sceneToLoad);
    }

    IEnumerator LoadYourAsyncScene(int i_sceneToLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(i_sceneToLoad);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private int SceneCountConverted()
    {
        int totalScene = SceneManager.sceneCountInBuildSettings;
        return totalScene - 2;
    }
}