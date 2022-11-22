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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
