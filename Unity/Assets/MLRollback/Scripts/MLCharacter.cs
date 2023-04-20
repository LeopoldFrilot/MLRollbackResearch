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
    public fp currentHealth;
    public fp currentBlock;

    private bool isBlocking;

    // NotRolledBack 
    public int playerIndex;
    public string name;
    
    private MLGameManager GM;

    public MLCharacter(int playerIndex, string name, fp2 startingPosition, MLAnimationData[] animData) {
        this.name = name;
        currentHealth = MLConsts.MAX_HEALTH;
        currentBlock = MLConsts.MAX_BLOCK;
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
        bool blockedThisFrame = false;
        foreach (var button in frameButtons.buttons) {
            switch (button) {
                case MLInput.Buttons.Left:
                    if (!IsBlocking() && lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(-1, 0);
                    }
                    break;
                case MLInput.Buttons.Right:
                    if (!IsBlocking() && lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(1, 0);
                    }
                    break;
                case MLInput.Buttons.Up:
                    if (!IsBlocking() && lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(0, 1);
                    }
                    break;
                case MLInput.Buttons.Down:
                    if (!IsBlocking() && lag.GetLagType() is LagTypes.None or LagTypes.JumpStart) {
                        movementTracking += new int2(0, -1);
                    }
                    break;
                case MLInput.Buttons.Dash:
                    if (!IsBlocking() && lag.GetLagType() == LagTypes.None) {
                        dash = true;
                        animManager.StartAnimation(AnimationTypes.Dash, grounded);
                        lag.ApplyLag(LagTypes.Dash, frameNumber);
                    }
                    break;
                case MLInput.Buttons.Light:
                    if (!IsBlocking() && lag.GetLagType() == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Light, grounded);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frameLength);
                    }
                    break;
                case MLInput.Buttons.Medium:
                    if (!IsBlocking() && lag.GetLagType() == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Medium, grounded);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frameLength);
                    }
                    break;
                case MLInput.Buttons.Heavy:
                    if (!IsBlocking() && lag.GetLagType() == LagTypes.None && grounded) {
                        animManager.StartAnimation(AnimationTypes.Heavy, grounded);
                        lag.ApplyLag(LagTypes.Attack, frameNumber, animManager.GetCurrentAnimationData().frameLength);
                    }
                    break;
                case MLInput.Buttons.Block:
                    if ((lag.GetLagType() == LagTypes.None || lag.GetLagType() == LagTypes.Block) && grounded) {
                        blockedThisFrame = true;
                        animManager.StartAnimation(AnimationTypes.Block, grounded);
                    }
                    break;
            }
        }
        isBlocking = blockedThisFrame;
        GM.physics.ProcessPhysicsFromInput(ref physicsObject, movementTracking, dash);
    }

    public bool IsBlocking() {
        return isBlocking;
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
        if (hitCharacter.IsBlocking()) {
            hitCharacter.physicsObject.Launch(facingRight ? data.blockLaunchAngle * (fp).25 : new fp2(-1 * data.blockLaunchAngle.x * (fp).25, 0));
            physicsObject.Launch(!facingRight ? data.blockLaunchAngle * (fp).75 : new fp2(-1 * data.blockLaunchAngle.x * (fp).75, 0));
            hitCharacter.lag.ApplyLag(LagTypes.Block, frameNumber, data.blockStun);
            hitCharacter.ChangeBlock(-data.damage);
        }
        else {
            hitCharacter.physicsObject.Launch(facingRight ? data.normalLaunchAngle : new fp2(-1 * data.normalLaunchAngle.x, data.normalLaunchAngle.y));
            hitCharacter.lag.ApplyLag(LagTypes.Hit, frameNumber, data.hitStun);
            hitCharacter.ChangeHealth(-data.damage);
        }
    }

    public bool IsDead() {
        return currentHealth == 0;
    }

    public void ChangeHealth(fp delta) {
        currentHealth = fpmath.clamp(currentHealth + delta, 0, MLConsts.MAX_HEALTH);
    }
    
    public void ChangeBlock(fp delta) {
        fp newBlock = currentBlock + delta;
        currentBlock = fpmath.clamp(newBlock, 0, MLConsts.MAX_BLOCK);
        if (newBlock < 0) {
            ChangeHealth(newBlock);
        }
    }

    public void HandleDisconnectedFrame() {
        GM.physics.ProcessPhysicsFromInput(ref physicsObject, int2.zero, false);
    }

    public fp GetHealthPercentage() {
        return currentHealth / MLConsts.MAX_HEALTH;
    }

    public fp GetBlockPercentage() {
        return currentBlock / MLConsts.MAX_BLOCK;
    }

    public void Serialize(BinaryWriter bw) {
        physicsObject.Serialize(bw);
        bw.Write(facingRight);
        lag.Serialize(bw);
        animManager.Serialize(bw);
        bw.Write(currentHealth);
        bw.Write(currentBlock);
        bw.Write(isBlocking);
    }

    public void Deserialize(BinaryReader br) {
        physicsObject.Deserialize(br);
        facingRight = br.ReadBoolean();
        lag.Deserialize(br);
        animManager.Deserialize(br);
        currentHealth = br.ReadDecimal();
        currentBlock = br.ReadDecimal();
        isBlocking = br.ReadBoolean();
    }

    public override int GetHashCode() {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + physicsObject.GetHashCode();
        hashCode = hashCode * -1521134295 + facingRight.GetHashCode();
        hashCode = hashCode * -1521134295 + lag.GetHashCode();
        hashCode = hashCode * -1521134295 + animManager.GetHashCode();
        hashCode = hashCode * -1521134295 + currentHealth.GetHashCode();
        hashCode = hashCode * -1521134295 + currentBlock.GetHashCode();
        hashCode = hashCode * -1521134295 + isBlocking.GetHashCode();
        return hashCode;
    }
}