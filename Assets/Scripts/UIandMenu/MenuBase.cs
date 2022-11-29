using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuBase : MonoBehaviour
{
    float currentVol;
    [SerializeField] AudioClip menuSelect;
    [SerializeField] AudioClip menyToggle;
    [SerializeField] AudioClip menuExit;
    [SerializeField] AudioClip menuBuzzer;
    [SerializeField] AudioClip startGameSound;

    PlayerControls menuControls;

    protected virtual void Awake()
    {
        menuControls = new PlayerControls();
        menuControls.UI.Enable();

    }

    public void SetButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }


    public void NewGame()
    {
        PlaySoundEffect(startGameSound);
        ChangeScene("Game");
        Time.timeScale = 1;
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }

    public void OpenMenu(GameObject menu)
    {
        PlaySoundEffect(menuSelect);
        menu.SetActive(true);
    }

    public void CloseMenu(GameObject menu)
    {
        PlaySoundEffect(menuExit);
        menu.SetActive(false);
    }

    public void AsyncChangeScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    void PlaySoundEffect(AudioClip sound)
    {
        if (sound)
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }
}
