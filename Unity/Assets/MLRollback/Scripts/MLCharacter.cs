using SharedGame;
using System;
using System.IO;
using Rect = MLPhysics.Rect;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;
using UnityEngine;

[Serializable]
public class MLCharacter : IMLSerializable {
    public fp2 position;
    public bool facingRight;
    public int playerIndex;
    
    private Rect hitCollider;
    private MLGameManager GM;

    public MLCharacter(int playerIndex, fp2 startingPosition) {
        this.playerIndex = playerIndex;
        position = startingPosition;
        GM = GameManager.Instance as MLGameManager;
    }

    public void UseInput(MLInput.FrameButtons frameButtons) {
        int2 movementTracking = int2.zero;
        if (frameButtons.buttons.Count > 0) Debug.LogWarning(playerIndex);
        foreach (var button in frameButtons.buttons) {
            switch (button) {
                case MLInput.Buttons.Left:
                    movementTracking += new int2(-1, 0);
                    break;
                case MLInput.Buttons.Right:
                    movementTracking += new int2(1, 0);
                    break;
                case MLInput.Buttons.Up:
                    movementTracking += new int2(0, 1);
                    break;
                case MLInput.Buttons.Down:
                    movementTracking += new int2(0, -1);
                    break;
            }
        }
        
        position = GM.physics.MovePhysicsObject(position, movementTracking);
    }

    public void HandleDisconnectedFrame() {
        position = GM.physics.MovePhysicsObject(position, int2.zero);
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(facingRight);
    }

    public void Deserialize(BinaryReader br) {
        position.x = br.ReadDecimal();
        position.y = br.ReadDecimal();
        facingRight = br.ReadBoolean();
    }

    public override int GetHashCode() {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + facingRight.GetHashCode();
        return hashCode;
    }
}