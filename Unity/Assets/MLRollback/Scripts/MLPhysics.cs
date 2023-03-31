using Unity.Mathematics.FixedPoint;

public class MLPhysics {
    public class Rect {
        private fp2 topLeft;
        private fp2 bottomRight;

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