using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class GameOverMenu : MenuBase
{
    Player player;

    // Start is called before the first frame update
    private void Awake()
    {
        player = FindObjectOfType<Player>().GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
