using SharedGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGGPO;

public class MLGameManager : GameManager
{
    public override void StartLocalGame() {
        StartGame(new LocalRunner(new MLGame(2)));
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex) {
        var game = new GGPORunner("vectorwar", new MLGame(connections.Count), perfPanel);
        game.Init(connections, playerIndex);
        StartGame(game);
    }
}
