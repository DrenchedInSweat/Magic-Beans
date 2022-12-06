using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleSwitch : MonoBehaviour
{
    [SerializeField] private bool state;
    [SerializeField] private UnityEvent myEvent;
    public void Activate()
    {
        if(!state)
            myEvent?.Invoke();
        state = true;
    }

    public void GiveRandomUpgrade()
    {
        
    }
}
