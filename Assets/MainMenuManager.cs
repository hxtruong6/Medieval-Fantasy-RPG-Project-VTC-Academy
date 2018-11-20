﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public GameObject loadingScreen;

    public void Play()
    {
        loadingScreen.gameObject.SetActive(true);
        SceneManager.LoadScene("LevelDemo");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
