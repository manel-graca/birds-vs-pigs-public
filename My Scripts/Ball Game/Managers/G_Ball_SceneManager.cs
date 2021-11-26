using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G_Ball_SceneManager : MonoBehaviour
{
    #region Singleton
    public static G_Ball_SceneManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion
    
    public void RestartCurrentScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }

}
