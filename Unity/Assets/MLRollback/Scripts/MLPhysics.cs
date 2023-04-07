using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;

public class MLPhysics {
    private readonly Rect playAreaExtents;
    private readonly fp moveSpeed = (fp).05f;

    public MLPhysics(Rect playAreaExtents) {
        this.playAreaExtents = playAreaExtents;
    }

    public fp2 MovePhysicsObject(fp2 curPosition, int2 direction) {
        fp newX = fpmath.clamp(curPosition.x + moveSpeed * direction.x, playAreaExtents.Left, playAreaExtents.Right);
        fp newY = fpmath.clamp(curPosition.y + direction.y, playAreaExtents.Bottom, playAreaExtents.Top);
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