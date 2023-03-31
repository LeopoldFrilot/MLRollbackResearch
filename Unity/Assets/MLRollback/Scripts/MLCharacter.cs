using Unity.Mathematics.FixedPoint;
using Rect = MLPhysics.Rect;

public class MLCharacter {
    private Rect hitCollider;
    private fp2 position;
    private bool facingRight;

    public MLCharacter(fp2 startingPosition) {
        position = startingPosition;
    }
}