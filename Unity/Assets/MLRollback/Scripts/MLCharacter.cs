using SharedGame;
using System;
using System.IO;
using Rect = MLPhysics.Rect;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;

[Serializable]
public class MLCharacter : IMLSerializable, IMLPhysicsObject {
    public PhysicsObject physicsObject;
    public bool facingRight;
    public MLLag lag;
    public MLAnimationManager animManager;

    // NotRolledBack 
    public int playerIndex;
    
    private MLGameManager GM;

    public MLCharacter(int playerIndex, fp2 startingPosition, MLAnimationData[] animData) {
        this.playerIndex = playerIndex;
        physicsObject = new PhysicsObject(startingPosition);
        animManager = new MLAnimationManager(animData);
        lag = new MLLag();
        GM = GameManager.Instance as MLGameManager;
        physicsObject.OnGrounded += OnGrounded;
        physicsObject.OnAerial += OnAerial;
    }

    private void OnAerial(int frameNumber) {
        animManager.StartAnimation(AnimationTypes.Airborne);
    }

    private void OnGrounded(int frameNumber) {
        lag.ApplyLag(LagTypes.LandingLag, frameNumber);
        animManager.StartAnimation(AnimationTypes.Idle);
    }

    public void UseInput(MLInput.FrameButtons frameButtons, int frameNumber) {
        int2 movementTracking = int2.zero;
        bool dash = false;
        bool grounded = GM.physics.IsGrounded(physicsObject.curPosition);
        foreach (var button in frameButtons.buttons) {
            switch (button) {
                case MLInput.Buttons.Left:
                    if (lag.GetLagType(frameNumber) is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(-1, 0);
                    }
                    break;
                case MLInput.Buttons.Right:
                    if (lag.GetLagType(frameNumber) is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(1, 0);
                    }
                    break;
                case MLInput.Buttons.Up:
                    if (lag.GetLagType(frameNumber) is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(0, 1);
                    }
                    break;
                case MLInput.Buttons.Down:
                    if (lag.GetLagType(frameNumber) is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(0, -1);
                    }
                    break;
                case MLInput.Buttons.Dash:
                    if (lag.GetLagType(frameNumber) == LagTypes.None) {
                        dash = true;
                        animManager.StartAnimation(AnimationTypes.Dash);
                        lag.ApplyLag(LagTypes.Dash, frameNumber);
                    }
                    break;
                case MLInput.Buttons.Light:
                    if (lag.GetLagType(frameNumber) == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Light);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frames);
                    }
                    break;
                case MLInput.Buttons.Medium:
                    if (lag.GetLagType(frameNumber) == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Medium);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frames);
                    }
                    break;
                case MLInput.Buttons.Heavy:
                    if (lag.GetLagType(frameNumber) == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Heavy);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frames);
                    }
                    break;
                
            }
        }
        GM.physics.ProcessPhysicsFromInput(ref physicsObject, movementTracking, dash);
    }

    public void HandleDisconnectedFrame() {
        GM.physics.ProcessPhysicsFromInput(ref physicsObject, int2.zero, false);
    }

    public void Serialize(BinaryWriter bw) {
        physicsObject.Serialize(bw);
        bw.Write(facingRight);
        lag.Serialize(bw);
        animManager.Serialize(bw);
    }

    public void Deserialize(BinaryReader br) {
        physicsObject.Deserialize(br);
        facingRight = br.ReadBoolean();
        lag.Deserialize(br);
        animManager.Deserialize(br);
    }

    public override int GetHashCode() {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + physicsObject.GetHashCode();
        hashCode = hashCode * -1521134295 + facingRight.GetHashCode();
        hashCode = hashCode * -1521134295 + lag.GetHashCode();
        hashCode = hashCode * -1521134295 + animManager.GetHashCode();
        return hashCode;
    }

    public ref PhysicsObject GetPhysicsObject() {
        return ref physicsObject;
    }
}