using Rect = MLPhysics.Rect;
using SharedGame;
using System.Collections.Generic;
using Unity.Mathematics.FixedPoint;
using UnityGGPO;

public class MLGameManager : GameManager {
    public MLUCharacter unityCharacterInPlay;
    public MLPhysics physics;
    
    public override void StartLocalGame() {
        Setup();
        StartGame(new LocalRunner(new MLGame(2, ExtractAnimData())));
    }

    private MLAnimationData[] ExtractAnimData() {
        var data = new MLAnimationData[unityCharacterInPlay.animations.Count];
        for (int i = 0; i < unityCharacterInPlay.animations.Count; i++) {
            var newData = new MLAnimationData {
                frames = unityCharacterInPlay.animations[i].spritesData.Count,
                loopable = unityCharacterInPlay.animations[i].loopable,
                animationType = unityCharacterInPlay.animations[i].animationType
            };
            data[i] = newData;
        }
        return data;
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex) {
        Setup();
        GGPORunner game = new GGPORunner("mlgame", new MLGame(connections.Count, ExtractAnimData()), perfPanel);
        game.Init(connections, playerIndex);
        StartGame(game);
    }

    private void Setup() {
        physics = new MLPhysics(new Rect(
            new fp2(-MLConsts.PLAY_AREA_WIDTH / 2, MLConsts.PLAY_AREA_HEIGHT),
            new fp2(MLConsts.PLAY_AREA_WIDTH / 2, MLConsts.PLAY_AREA_GROUND)));
    }
}