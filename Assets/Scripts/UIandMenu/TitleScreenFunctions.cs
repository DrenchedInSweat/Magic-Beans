using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenFunctions : MonoBehaviour
{
    [Header("Background Changes")]
    [SerializeField] Image background;
    [SerializeField] List<Sprite> titleImages = new List<Sprite>();

    [Header("Visibility Togglable Buttons")]
    [SerializeField] Button continueButton;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject saveMenu;



    private void Start()
    {
        //find playerpref with node/stage and set background to image -1 


        //if playerpref of playerdata exists and is not null, show continue else hide it
        
    }

    public void NewGame()
    {
        
    }



    public void DeleteSaveDate()
    {

    }

    //used mainly for bug testing or if player wants to reset their game
    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
