using Rect = MLPhysics.Rect;
using SharedGame;
using System.Collections.Generic;
using Unity.Mathematics.FixedPoint;
using UnityGGPO;

public class MLGameManager : GameManager {
    public MLInput input;
    public MLPhysics physics;
    
    public override void StartLocalGame() {
        Setup();
        StartGame(new LocalRunner(new MLGame(2)));
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex) {
        Setup();
        GGPORunner game = new GGPORunner("mlgame", new MLGame(connections.Count), perfPanel);
        game.Init(connections, playerIndex);
        StartGame(game);
    }

    private void Setup() {
        input = new MLInput();
        physics = new MLPhysics(new Rect(
            new fp2(-MLConsts.PLAY_AREA_WIDTH / 2, MLConsts.PLAY_AREA_HEIGHT),
            new fp2(MLConsts.PLAY_AREA_WIDTH / 2, MLConsts.PLAY_AREA_GROUND)));
    }
}