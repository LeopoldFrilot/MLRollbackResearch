using SharedGame;
using System.Collections.Generic;
using UnityGGPO;

public class MLGameManager : GameManager {
    public MLInput input;
    
    public override void StartLocalGame() {
        Setup();
        StartGame(new LocalRunner(new MLGame(2)));
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex) {
        Setup();
        GGPORunner game = new GGPORunner("vectorwar", new MLGame(connections.Count), perfPanel);
        game.Init(connections, playerIndex);
        StartGame(game);
    }

    private void Setup() {
        input = new MLInput();
    }
}