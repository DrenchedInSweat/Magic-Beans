using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuBase : MonoBehaviour
{
    
    //[SerializeField] AudioClip menyToggle;
    //[SerializeField] AudioClip menuBuzzer;
   // [SerializeField] AudioClip startGameSound;

    
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }

   

    
/*
    public void AsyncChangeScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    } */
}
