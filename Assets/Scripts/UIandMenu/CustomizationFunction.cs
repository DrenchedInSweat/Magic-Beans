using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationFunction : MonoBehaviour
{
    [SerializeField] Customization[] customizations;
    [SerializeField] Button[] buttons;
    [SerializeField] Image displayImage;
    [SerializeField] TMP_Text displayText;

    [SerializeField] AudioClip hoversound;
    [SerializeField] AudioClip selectsound;
    [SerializeField] AudioClip declinesound;

    MeshRenderer glove;
    Texture gloveMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i > customizations.Length; i++)
        {
            if (customizations[i].unlocked)
            {
                buttons[i].interactable = true;

                if(customizations[i].image)
                buttons[i].GetComponent<Image>().sprite = customizations[i].image;
            }
            else
            {
                buttons[i].interactable = false;
            }
            buttons[i].onClick.AddListener(() => { ChangeCustom(i);});
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCustom(int i)
    {
        
        //glove.material.SetTexture(customizations[i].image.ToString(), gloveMaterial);
    }

    public void DisplayInfo(int i)
    {
        displayImage.sprite = customizations[i].image;
        displayText.text = customizations[i].description;
    }

}
