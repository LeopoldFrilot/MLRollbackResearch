using CONST = MLGameConstants;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;
using Rect = MLPhysics.Rect;

public class MLCharacter {
    public fp2 position;
    
    private Rect hitCollider;
    private bool facingRight;
    private MLPhysics physics;

    public MLCharacter(fp2 startingPosition) {
        position = startingPosition;
        physics = new MLPhysics(new Rect(
            new fp2(-CONST.PLAY_AREA_WIDTH / 2, CONST.PLAY_AREA_HEIGHT),
            new fp2(CONST.PLAY_AREA_WIDTH / 2, CONST.PLAY_AREA_GROUND)));
    }

    public void UseInput(MLInput.FrameButtons frameButtons) {
        int2 movementTracking = int2.zero;
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
        position = physics.MoveCharacter(position, movementTracking);
    }
}