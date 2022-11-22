using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Customization", fileName = "Customization", order = 2)]
public class Customization : ScriptableObject
{
    [SerializeField] Sprite image;
    [SerializeField] string description;
    [SerializeField] bool unlocked = false;
   
    public void unlockCustom()
    {
        unlocked = true;
    }
}
