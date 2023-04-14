using SharedGame;
using System;
using System.IO;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;

[Serializable]
public class MLCharacter : IMLSerializable, IMLCharacterPhysicsObject {
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
        animManager.StartAnimation(AnimationTypes.Idle, true);
        lag = new MLLag();
        GM = GameManager.Instance as MLGameManager;
        physicsObject.OnGrounded += OnGrounded;
        physicsObject.OnAerial += OnAerial;
        lag.OnLagStarted += OnLagStarted;
        lag.OnLagEnded += OnLagEnded;
    }

    private void OnAerial(int frameNumber) {
        if (lag.GetLagType() != LagTypes.Hit) {
            animManager.StartAnimation(AnimationTypes.Jump, false);
        }
    }

    private void OnGrounded(int frameNumber) {
        if (lag.GetLagType() != LagTypes.Hit) {
            lag.ApplyLag(LagTypes.LandingLag, frameNumber);
            animManager.StartAnimation(AnimationTypes.Idle, true);
        }
    }

    private void OnLagStarted(LagTypes lagType) {
        if (lagType == LagTypes.Hit) {
            bool grounded = GM.physics.IsGrounded(physicsObject.curPosition);
            animManager.StartAnimation(AnimationTypes.Hit, grounded);
        }
    }

    private void OnLagEnded(LagTypes lagType) {
        if (lagType == LagTypes.Hit) {
            bool grounded = GM.physics.IsGrounded(physicsObject.curPosition);
            animManager.ReturnToIdle(grounded);
        }
    }

    public void UseInput(MLInput.FrameButtons frameButtons, int frameNumber) {
        int2 movementTracking = int2.zero;
        bool dash = false;
        bool grounded = GM.physics.IsGrounded(physicsObject.curPosition);
        foreach (var button in frameButtons.buttons) {
            switch (button) {
                case MLInput.Buttons.Left:
                    if (lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(-1, 0);
                    }
                    break;
                case MLInput.Buttons.Right:
                    if (lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(1, 0);
                    }
                    break;
                case MLInput.Buttons.Up:
                    if (lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(0, 1);
                    }
                    break;
                case MLInput.Buttons.Down:
                    if (lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(0, -1);
                    }
                    break;
                case MLInput.Buttons.Dash:
                    if (lag.GetLagType() == LagTypes.None) {
                        dash = true;
                        animManager.StartAnimation(AnimationTypes.Dash, grounded);
                        lag.ApplyLag(LagTypes.Dash, frameNumber);
                    }
                    break;
                case MLInput.Buttons.Light:
                    if (lag.GetLagType() == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Light, grounded);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frameLength);
                    }
                    break;
                case MLInput.Buttons.Medium:
                    if (lag.GetLagType() == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Medium, grounded);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frameLength);
                    }
                    break;
                case MLInput.Buttons.Heavy:
                    if (lag.GetLagType() == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Heavy, grounded);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frameLength);
                    }
                    break;
            }
        }
        
        GM.physics.ProcessPhysicsFromInput(ref physicsObject, movementTracking, dash);
    }

    public ref PhysicsObject GetPhysicsObject() {
        return ref physicsObject;
    }

    public MLPhysics.Rect[] GetHurtBoxes() {
        MLPhysics.Rect hurtbox = facingRight ? animManager.GetCurrentAnimationFrameData().hurtbox : animManager.GetCurrentAnimationFrameData().hurtbox.FlippedRect;
        MLPhysics.Rect collider = new MLPhysics.Rect(physicsObject.curPosition + hurtbox.Center, hurtbox.Width, hurtbox.Height);
        return new []{collider};
    }

    public MLPhysics.Rect[] GetHitboxes() {
        MLPhysics.Rect[] adjustedTriggers = new MLPhysics.Rect[animManager.GetCurrentAnimationFrameData().hitboxes.Length];
        for (int i = 0; i < animManager.GetCurrentAnimationFrameData().hitboxes.Length; i++) {
            var hitbox = animManager.GetCurrentAnimationFrameData().hitboxes[i];
            MLPhysics.Rect box = facingRight ? hitbox : hitbox.FlippedRect;
            MLPhysics.Rect trigger = new MLPhysics.Rect(physicsObject.curPosition + box.Center, box.Width, box.Height);
            adjustedTriggers[i] = trigger;
        }
        return adjustedTriggers;
    }

    public MLCharacter GetCharacter() {
        return this;
    }

    public bool CanUseHitboxes() {
        return !animManager.currentAnimationCombatUsed;
    }

    public void UseHitboxesOn(IMLCharacterPhysicsObject character, int frameNumber) {
        animManager.currentAnimationCombatUsed = true;
        MLAnimationFrameData data = animManager.GetCurrentAnimationFrameData();
        MLCharacter hitCharacter = character.GetCharacter();
        hitCharacter.physicsObject.Launch(facingRight ? data.normalLaunchAngle : new fp2(-1 * data.normalLaunchAngle.x, data.normalLaunchAngle.y));
        hitCharacter.lag.ApplyLag(LagTypes.Hit, frameNumber, data.hitStun);
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
}