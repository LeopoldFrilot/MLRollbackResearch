using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MLInput {
    public enum Buttons {
        Left,
        Right,
        Up,
        Down,
        Dash,
        Light,
        Medium,
        Heavy,
        Block
    }

   public struct FrameButtons {
        public List<Buttons> buttons;

        public FrameButtons(List<Buttons> buttons) {
            this.buttons = buttons;
        }
    }

    public static FrameButtons ParseInputs(long input, out string debugString, bool shouldFlipInputs = false) {
        List<Buttons> frameButtons = new List<Buttons>();
        StringBuilder SB = new StringBuilder("");
        if ((input & MLConsts.INPUT_LEFT) != 0) {
            frameButtons.Add(shouldFlipInputs ? Buttons.Right : Buttons.Left);
            SB.Append(shouldFlipInputs ? " Right," : " Left,");
        }
        if ((input & MLConsts.INPUT_RIGHT) != 0) {
            frameButtons.Add(shouldFlipInputs ? Buttons.Left : Buttons.Right);
            SB.Append(shouldFlipInputs ? " Left," : " Right,");
        }
        if ((input & MLConsts.INPUT_UP) != 0) {
            frameButtons.Add(Buttons.Up);
            SB.Append(" Up,");
        }
        if ((input & MLConsts.INPUT_DOWN) != 0) {
            frameButtons.Add(Buttons.Down);
            SB.Append(" Down,");
        }
        if ((input & MLConsts.INPUT_DASH) != 0) {
            frameButtons.Add(Buttons.Dash);
            SB.Append(" Dash,");
        }
        if ((input & MLConsts.INPUT_LIT_ATTACK) != 0) {
            frameButtons.Add(Buttons.Light);
            SB.Append(" Light,");
        }
        if ((input & MLConsts.INPUT_MED_ATTACK) != 0) {
            frameButtons.Add(Buttons.Medium);
            SB.Append(" Medium,");
        }
        if ((input & MLConsts.INPUT_HEV_ATTACK) != 0) {
            frameButtons.Add(Buttons.Heavy);
            SB.Append(" Heavy,");
        }
        if ((input & MLConsts.INPUT_BLOCK) != 0) {
            frameButtons.Add(Buttons.Block);
            SB.Append(" Block,");
        }

        debugString = SB.ToString().Trim();
        return new FrameButtons(frameButtons);
    }

    public static long SerializeInputs(int controllerId) {
        long input = 0;

        switch (controllerId) {
            case 0:
                if (Input.GetKey(KeyCode.W)) {
                    input |= MLConsts.INPUT_UP;
                }
                if (Input.GetKey(KeyCode.S)) {
                    input |= MLConsts.INPUT_DOWN;
                }
                if (Input.GetKey(KeyCode.A)) {
                    input |= MLConsts.INPUT_LEFT;
                }
                if (Input.GetKey(KeyCode.D)) {
                    input |= MLConsts.INPUT_RIGHT;
                }
                if (Input.GetKey(KeyCode.LeftShift)) {
                    input |= MLConsts.INPUT_DASH;
                }
                if (Input.GetKey(KeyCode.I)) {
                    input |= MLConsts.INPUT_LIT_ATTACK;
                }
                if (Input.GetKey(KeyCode.O)) {
                    input |= MLConsts.INPUT_MED_ATTACK;
                }
                if (Input.GetKey(KeyCode.P)) {
                    input |= MLConsts.INPUT_HEV_ATTACK;
                }
                if (Input.GetKey(KeyCode.LeftControl)) {
                    input |= MLConsts.INPUT_BLOCK;
                }
                break;
            case 1:
                if (Input.GetKey(KeyCode.O)) {
                    input |= MLConsts.INPUT_UP;
                }
                if (Input.GetKey(KeyCode.L)) {
                    input |= MLConsts.INPUT_DOWN;
                }
                if (Input.GetKey(KeyCode.K)) {
                    input |= MLConsts.INPUT_LEFT;
                }
                if (Input.GetKey(KeyCode.Semicolon)) {
                    input |= MLConsts.INPUT_RIGHT;
                }
                if (Input.GetKey(KeyCode.RightShift)) {
                    input |= MLConsts.INPUT_DASH;
                }
                if (Input.GetKey(KeyCode.I)) {
                    input |= MLConsts.INPUT_LIT_ATTACK;
                }
                if (Input.GetKey(KeyCode.U)) {
                    input |= MLConsts.INPUT_MED_ATTACK;
                }
                if (Input.GetKey(KeyCode.Y)) {
                    input |= MLConsts.INPUT_HEV_ATTACK;
                }
                if (Input.GetKey(KeyCode.RightControl)) {
                    input |= MLConsts.INPUT_BLOCK;
                }
                break;
                
        }
        
        return input;
    }
}