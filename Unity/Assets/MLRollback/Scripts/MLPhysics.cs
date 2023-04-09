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

    private readonly List<IMLPhysicsObject> registeredPhysicsObjects = new List<IMLPhysicsObject>();

    public MLPhysics(Rect playAreaExtents) {
        this.playAreaExtents = playAreaExtents;
    }

    public void RegisterPhysicsObject(ref IMLPhysicsObject physicsObject) {
        if (!registeredPhysicsObjects.Contains(physicsObject)) {
            registeredPhysicsObjects.Add(physicsObject);
        }
    }

    public void UpdatePhysics(int curFrameNumber) {
        foreach (var physicsObject in registeredPhysicsObjects) {
            bool grounded = IsGrounded(physicsObject.GetPhysicsObject().curPosition);
            MovePhysicsObject(ref physicsObject.GetPhysicsObject());
            bool postMoveGrounded = IsGrounded(physicsObject.GetPhysicsObject().curPosition);
            if (grounded != postMoveGrounded) {
                if (postMoveGrounded) {
                    physicsObject.GetPhysicsObject().TriggerGrounded(curFrameNumber);
                }
                else {
                    physicsObject.GetPhysicsObject().TriggerAerial(curFrameNumber);
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

    private void MovePhysicsObject(ref PhysicsObject physicsObject) {
        fp newX = fpmath.clamp(physicsObject.curPosition.x + physicsObject.velocity.x, playAreaExtents.Left, playAreaExtents.Right);
        fp newY = fpmath.clamp(physicsObject.curPosition.y + physicsObject.velocity.y, playAreaExtents.Bottom, playAreaExtents.Top);
        physicsObject.curPosition = new fp2(newX, newY);
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

        public Rect(fp2 topLeft, fp2 bottomRight) {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public Rect(fp2 center, fp width, fp height) {
            this.topLeft = center + new fp2(-width / 2, height / 2);
            this.bottomRight = center + new fp2(width / 2, -height / 2);
        }
    }
}