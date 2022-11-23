using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBase : MonoBehaviour
{
    float currentVol;
    [SerializeField] AudioClip menuSelect;
    [SerializeField] AudioClip menyToggle;
    [SerializeField] AudioClip menuExit;
    [SerializeField] AudioClip menuBuzzer;

    public void NewGame()
    {
        ChangeScene("Game");
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void AsyncChangeScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }
}
