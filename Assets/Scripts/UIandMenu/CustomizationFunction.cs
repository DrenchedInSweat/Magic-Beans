using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationFunction : MonoBehaviour
{
    [SerializeField] Customization[] customizations;
    [SerializeField] Button[] buttons;

    
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i > customizations.Length; i++)
        {
            if (customizations[i].unlocked)
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
