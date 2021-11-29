using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameController : GameController
{
    public override bool CanPerformMove()
    {
        throw new System.NotImplementedException();
    }

    public override void TryToStartCurrentGame()
    {
        throw new System.NotImplementedException();
    }

    protected override void SetGameState(GameState state)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetMultiplayerDependencies(NetworkManager networkManager)
    {
        throw new NotImplementedException();
    }
}
