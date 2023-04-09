using System.IO;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;
using UnityEngine;

public struct PhysicsObject : IMLSerializable {
    public fp2 curPosition;
    public fp2 velocity;
    public fp2 acceleration;

    public PhysicsObject(fp2 startingPosition) {
        curPosition = startingPosition;
        velocity = fp2.zero;
        acceleration = fp2.zero;
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(curPosition.x);
        bw.Write(curPosition.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(acceleration.x);
        bw.Write(acceleration.y);
    }

    public void Deserialize(BinaryReader br) {
        curPosition.x = br.ReadDecimal();
        curPosition.y = br.ReadDecimal();
        velocity.x = br.ReadDecimal();
        velocity.y = br.ReadDecimal();
        acceleration.x = br.ReadDecimal();
        acceleration.y = br.ReadDecimal();
    }

    public override int GetHashCode() {
        int hashCode = 1628924851;
        hashCode = hashCode * -1466031817 + curPosition.GetHashCode();
        hashCode = hashCode * -1466031817 + velocity.GetHashCode();
        hashCode = hashCode * -1466031817 + acceleration.GetHashCode();
        return hashCode;
    }
}

public class MLPhysics {
    private readonly Rect playAreaExtents;
    private readonly fp moveSpeed = (fp).05f;
    private readonly fp gravity = (fp)(-.06f);
    private readonly fp verticalJumpStrength = (fp).7f;
    private readonly fp horizontalJumpStrength = (fp)1f;
    private readonly fp maxHorizontalVelocity = (fp).1f;
    private readonly fp friction = (fp)(-.01f);

    public MLPhysics(Rect playAreaExtents) {
        this.playAreaExtents = playAreaExtents;
    }

    public fp2 MovePhysicsObject(ref PhysicsObject PO, int2 direction) {
        // Horizontal
        if (IsGrounded(PO.curPosition)) {
            fp newXVelocity = moveSpeed * direction.x + PO.velocity.x;
            if (fpmath.abs(newXVelocity) <= fpmath.abs(friction)) {
                PO.velocity.x = 0;
            }
            else {
                newXVelocity += fpmath.sign(newXVelocity) * friction;
                PO.velocity.x = fpmath.clamp(newXVelocity, -maxHorizontalVelocity, maxHorizontalVelocity);
            }
        }
        fp newX = fpmath.clamp(PO.curPosition.x + PO.velocity.x, playAreaExtents.Left, playAreaExtents.Right);

        // Vertical
        if (IsGrounded(PO.curPosition)) {
            if (direction.y > 0) {
                PO.velocity += new fp2(PO.velocity.x * horizontalJumpStrength, verticalJumpStrength);
            }
            else {
                PO.velocity.y = 0;
            }
        }
        else {
            PO.velocity += new fp2(PO.acceleration.x, PO.acceleration.y + gravity);
        }
        fp newY = fpmath.clamp(PO.curPosition.y + PO.velocity.y, playAreaExtents.Bottom, playAreaExtents.Top);
        
        fp2 newPosition = new fp2(newX, newY);
        return newPosition;
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