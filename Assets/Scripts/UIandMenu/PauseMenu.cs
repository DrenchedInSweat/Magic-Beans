using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject optionsScreen;
    
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip unpausedSound;
    [SerializeField] private AudioClip menuSelect;
    [SerializeField] private AudioClip menuExit;

    
    private void Awake()
    {
        GameManager.Instance.onGamePaused += Pause;
        GameManager.Instance.onGameUnpaused += Unpause;
    }

    private void Pause()
    {
        print("Oops");
        AudioSource.PlayClipAtPoint(pauseSound, transform.position, GameManager.Instance.SfxVolume);
        pauseScreen.SetActive(true);
    }
    
    private void Unpause()
    {
        print("Yay");
        AudioSource.PlayClipAtPoint(unpausedSound, transform.position, GameManager.Instance.SfxVolume);
        optionsScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
    
    //-------------------- Unity Event Functions ----------------------------- //
    
    public void Resume()
    {
        print("..?");
        Unpause(); // Correct EVEN if in upgrade menu.
        GameManager.Instance.TogglePause();
    }
    
    public void Restart()
    {
        GameManager.Instance.ToggleStop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitle()
    {
        GameManager.Instance.ToggleStop();
        SceneManager.LoadScene(0);
    }

    public void OpenMenu(GameObject menu)
    {
        AudioSource.PlayClipAtPoint(menuSelect, transform.position, GameManager.Instance.SfxVolume);
        menu.SetActive(true);
    }
    
    public void CloseMenu(GameObject menu)
    {
        AudioSource.PlayClipAtPoint(menuExit, transform.position, GameManager.Instance.SfxVolume);
        menu.SetActive(false);
    }
    
}
