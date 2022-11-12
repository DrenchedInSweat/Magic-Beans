using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject optionsScreen;
    bool isPaused;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                pauseScreen.SetActive(true);

            }
            else
            {
                pauseScreen.SetActive(false);
                optionsScreen.SetActive(false);
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        
    }
    public void ResumeGame()
    {

        Time.timeScale = 1;
        pauseScreen.SetActive(false);

    }

    public void Options()
    {
        optionsScreen.SetActive(true);
    }
}
