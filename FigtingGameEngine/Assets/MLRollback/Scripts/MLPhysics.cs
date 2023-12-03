using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;

public struct PhysicsObject : IMLSerializable {
    public fp2 curPosition;
    public fp2 velocity;

    public Action<int> OnGrounded;
    public void TriggerGrounded(int curFrameNumber) {
        OnGrounded?.Invoke(curFrameNumber);
    }
    
    public Action<int> OnAerial;
    public void TriggerAerial(int curFrameNumber) {
        OnAerial?.Invoke(curFrameNumber);
    }

    public PhysicsObject(fp2 startingPosition) {
        curPosition = startingPosition;
        velocity = fp2.zero;
        OnGrounded = null;
        OnAerial = null;
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(curPosition.x);
        bw.Write(curPosition.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
    }

    public void Deserialize(BinaryReader br) {
        curPosition.x = br.ReadDecimal();
        curPosition.y = br.ReadDecimal();
        velocity.x = br.ReadDecimal();
        velocity.y = br.ReadDecimal();
    }

    public override int GetHashCode() {
        int hashCode = 1628924851;
        hashCode = hashCode * -1466031817 + curPosition.GetHashCode();
        hashCode = hashCode * -1466031817 + velocity.GetHashCode();
        return hashCode;
    }

    public void Launch(fp2 launchAngle) {
        velocity += launchAngle;
    }
}

public class MLPhysics {
    private readonly Rect playAreaExtents;
    private readonly fp moveSpeed = (fp).03f;
    private readonly fp gravity = (fp)(-.025f);
    private readonly fp verticalJumpStrength = (fp).4f;
    private readonly fp horizontalJumpStrength = (fp).1f;
    private readonly fp maxHorizontalVelocity = (fp).1f;
    private readonly fp friction = (fp)(-.01f);
    private readonly fp2 groundedDashStrength = new fp2((fp)1.5f, 0);
    private readonly fp2 aerialDashStrength = new fp2((fp).1f, (fp).3f);

    private readonly List<IMLCharacterPhysicsObject> registeredCharacterObjects = new List<IMLCharacterPhysicsObject>();
    public MLPhysics(Rect playAreaExtents) {
        this.playAreaExtents = playAreaExtents;
    }

    public void RegisterCharacterObject(ref IMLCharacterPhysicsObject physicsObject) {
        if (!registeredCharacterObjects.Contains(physicsObject)) {
            registeredCharacterObjects.Add(physicsObject);
        }
    }

    public void UpdatePhysics(int curFrameNumber) {
        bool[] parallelPrevGroundedArray = new bool[registeredCharacterObjects.Count];
        for (int i = 0; i < registeredCharacterObjects.Count; i++) {
            var characterObject = registeredCharacterObjects[i];
            parallelPrevGroundedArray[i] = IsGrounded(characterObject.GetPhysicsObject().curPosition);
            UseHitboxes(characterObject, curFrameNumber);
            MovePhysicsObject(characterObject);
        }
        
        for (int i = 0; i < registeredCharacterObjects.Count; i++) {
            var characterObject = registeredCharacterObjects[i];
            bool postMoveGrounded = IsGrounded(characterObject.GetPhysicsObject().curPosition);
            if (parallelPrevGroundedArray[i] != postMoveGrounded) {
                if (postMoveGrounded) {
                    characterObject.GetPhysicsObject().TriggerGrounded(curFrameNumber);
                }
                else {
                    characterObject.GetPhysicsObject().TriggerAerial(curFrameNumber);
                }
            }
        }
    }

    private void UseHitboxes(IMLCharacterPhysicsObject instigator, int frameNumber) {
        if (instigator.CanUseHitboxes()) {
            foreach (var Hitbox in instigator.GetHitboxes()) {
                foreach (IMLCharacterPhysicsObject registeredCharacterObject in registeredCharacterObjects) {
                    if (instigator != registeredCharacterObject) {
                        foreach (Rect collider in registeredCharacterObject.GetHurtBoxes()) {
                            if (Hitbox.IsOverlapped(collider)) {
                                instigator.UseHitboxesOn(registeredCharacterObject, frameNumber);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ProcessPhysicsFromInput(ref PhysicsObject PO, int2 direction, bool dash) {
        bool grounded = IsGrounded(PO.curPosition);
        
        // Horizontal
        if (grounded) {
            fp newXVelocity = moveSpeed * direction.x + PO.velocity.x;
            if (fpmath.abs(newXVelocity) <= fpmath.abs(friction)) {
                PO.velocity.x = 0;
            }
            else {
                newXVelocity += fpmath.sign(newXVelocity) * friction;
                PO.velocity.x = fpmath.clamp(newXVelocity, -maxHorizontalVelocity, maxHorizontalVelocity);
            }
        }

        // Vertical
        if (grounded) {
            if (direction.y > 0) {
                PO.velocity += new fp2(PO.velocity.x * horizontalJumpStrength, verticalJumpStrength);
            }
            else {
                PO.velocity.y = 0;
            }
        }
        else {
            PO.velocity += new fp2(0, gravity);
        }

        if (dash && direction.x != 0) {
            if (grounded) {
                PO.velocity += new fp2(groundedDashStrength.x * direction.x, groundedDashStrength.y);
            }
            else {
                PO.velocity += new fp2(aerialDashStrength.x * direction.x, aerialDashStrength.y);
            }
        }
    }

    private void MovePhysicsObject(IMLPhysicsObject po) {
        fp2 prevPosition = po.GetPhysicsObject().curPosition;
        ClampedMove(po, prevPosition.x + po.GetPhysicsObject().velocity.x, prevPosition.y + po.GetPhysicsObject().velocity.y);
    }

    private void ClampedMove(IMLPhysicsObject po, fp newX, fp newY) {
        fp x = fpmath.clamp(newX, playAreaExtents.Left, playAreaExtents.Right);
        fp y = fpmath.clamp(newY, playAreaExtents.Bottom, playAreaExtents.Top);
        po.GetPhysicsObject().curPosition = new fp2(x, y);
    }

    public bool IsGrounded(fp2 characterPosition) {
        return characterPosition.y == 0;
    }
    
    public class Rect {
        private readonly fp2 topLeft;
        private readonly fp2 bottomRight;
        public fp Left => topLeft.x;
        public fp Right => bottomRight.x;
        public fp Top => topLeft.y;
        public fp Bottom => bottomRight.y;
        public fp2 Center { get; }
        public Rect FlippedRect => new Rect(new fp2(Center.x * -1, Center.y), Width, Height);
        public fp Width => Right - Left;
        public fp Height => Top - Bottom;

        public Rect(fp2 topLeft, fp2 bottomRight) {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
            this.Center = new fp2(Left + Right / 2, Top + Bottom / 2);
        }

        public Rect(fp2 center, fp width, fp height) {
            this.topLeft = center + new fp2(-width / 2, height / 2);
            this.bottomRight = center + new fp2(width / 2, -height / 2);
            this.Center = center;
        }

        public fp2 GetOverlap(Rect otherCollider) {
            fp left = fpmath.max(Left, otherCollider.Left);
            fp right = fpmath.min(Right, otherCollider.Right);
            fp bottom = fpmath.max(Bottom, otherCollider.Bottom);
            fp top = fpmath.min(Top, otherCollider.Top);

            if (right < left || top < bottom) {
                return fp2.zero;
            }

            return new fp2(right - Left, top - Bottom);
        }

        public bool IsOverlapped(Rect otherCollider) {
            return GetOverlap(otherCollider).x != fp.zero || GetOverlap(otherCollider).y != fp.zero;
        }
    }
}