using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;
    public Player Player => player;

    bool showSlimePath = false;

    public bool ShowSlimePath => showSlimePath;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
    }
}
