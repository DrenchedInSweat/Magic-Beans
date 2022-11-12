using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenFunctions : MonoBehaviour
{
    public Image background;
    public List<Sprite> titleImages = new List<Sprite>();
    public Button continueButton;

    private void Start()
    {
        //find playerpref with node/stage and set background to image -1 


        //if playerpref of playerdata exists and is not null, show continue else hide it
        
    }


    //used mainly for bug testing or if player wants to reset their game
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
