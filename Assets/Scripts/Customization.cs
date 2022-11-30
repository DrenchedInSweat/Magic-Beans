using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Customization", fileName = "Customization", order = 2)]
public class Customization : ScriptableObject
{
    [SerializeField] public Sprite image;
    [SerializeField] [TextArea(3, 10)] public string description;
    [SerializeField] public bool unlocked = false;
   
    public void unlockCustom()
    {
        unlocked = true;
    }

    public void lockCustom()
    {

    }
}
