using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityScreen : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    
    
    
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (Button button in buttons)
        {
            DisplayInfo(button);
            button.onClick.AddListener(AbilitySelect);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    
    void DisplayInfo(Button button)
    {
        
    }

    void AbilitySelect()
    {

    }

}
