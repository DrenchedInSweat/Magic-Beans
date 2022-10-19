using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//For my fellow programmers wondering why I chose to make this a SO
//This is the intended use for SOs as they store single instance data that can both be manipulate during runtime and act as single framework objects,
//This means that there are never multiple instances of these objects, but rather pointers to the original data container, that we can manipulate while testing.
//There are some disadvantages, meaning that upgrades are static
[CreateAssetMenu(menuName = "Upgrade", fileName = "Upgrade", order = 1)]
public class UpgradeScriptableObject : ScriptableObject
{
    
}
