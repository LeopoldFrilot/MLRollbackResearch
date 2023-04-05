using SharedGame;
using Rect = MLPhysics.Rect;
using Unity.Mathematics;
using Unity.Mathematics.FixedPoint;

public class MLCharacter {
    public fp2 position;
    public bool facingRight;
    
    private Rect hitCollider;
    private MLGameManager GM;

    public MLCharacter(fp2 startingPosition) {
        position = startingPosition;
        GM = GameManager.Instance as MLGameManager;
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
        
        position = GM.physics.MoveCharacter(position, movementTracking);
    }
}