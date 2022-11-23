using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MenuBase
{
    PlayerControls controls;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;
    bool isPaused = false;


    [SerializeField] AudioClip pauseSound;
    [SerializeField] AudioClip unpausedSound;

    void Awake()
    {
        controls = new PlayerControls();
        controls.InGame.Enable();
        controls.InGame.EscapeMenu.performed += x => CheckPause();
    }


    void CheckPause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {

            PauseGame();
        }
        
    }

    public void PauseGame()
    {
        
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }
    public void ResumeGame()
    {
        if(optionsScreen)
        optionsScreen.SetActive(false);
        
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    public void Options()
    {
        optionsScreen.SetActive(true);
    }

}
