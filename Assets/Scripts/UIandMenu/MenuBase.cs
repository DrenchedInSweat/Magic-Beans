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
    [SerializeField] AudioClip startGameSound;

    public void NewGame()
    {
        PlaySoundEffect(startGameSound);
        ChangeScene("Game");
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
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
