using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MenuBase
{
    PlayerControls controls;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject topPauseButton;
    bool isPaused = false;


    [SerializeField] AudioClip pauseSound;
    [SerializeField] AudioClip unpausedSound;

    protected override void Awake()
    {
        base.Awake();
        controls = new PlayerControls();
        controls.InGame.Enable();
        controls.InGame.EscapeMenu.performed += x => CheckPause();
    }


    void CheckPause()
    {
        if (isPaused)
        {
            ResumeGame();
            SetButton(null);
        }
        else
        {

            PauseGame();
            SetButton(topPauseButton);
        }
        
    }

    public void Restart()
    {
        controls.UI.Disable();
        controls.InGame.Enable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        controls.InGame.Disable();
        controls.UI.Enable();

        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }
    public void ResumeGame()
    {
        controls.UI.Disable();
        controls.InGame.Enable();
        if (optionsScreen)
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
