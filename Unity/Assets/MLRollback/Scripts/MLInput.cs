using System.Collections.Generic;
using CONST = MLGameConstants;
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

    public static FrameButtons ParseInputs(long input, out string debugString) {
        List<Buttons> frameButtons = new List<Buttons>();
        StringBuilder SB = new StringBuilder("");
        if ((input & CONST.INPUT_LEFT) != 0) {
            frameButtons.Add(Buttons.Left);
            SB.Append(" Left,");
        }
        if ((input & CONST.INPUT_RIGHT) != 0) {
            frameButtons.Add(Buttons.Right);
            SB.Append(" Right,");
        }
        if ((input & CONST.INPUT_UP) != 0) {
            frameButtons.Add(Buttons.Up);
            SB.Append(" Up,");
        }
        if ((input & CONST.INPUT_DOWN) != 0) {
            frameButtons.Add(Buttons.Down);
            SB.Append(" Down,");
        }
        if ((input & CONST.INPUT_DASH) != 0) {
            frameButtons.Add(Buttons.Dash);
            SB.Append(" Dash,");
        }
        if ((input & CONST.INPUT_LIT_ATTACK) != 0) {
            frameButtons.Add(Buttons.Light);
            SB.Append(" Light,");
        }
        if ((input & CONST.INPUT_MED_ATTACK) != 0) {
            frameButtons.Add(Buttons.Medium);
            SB.Append(" Medium,");
        }
        if ((input & CONST.INPUT_HEV_ATTACK) != 0) {
            frameButtons.Add(Buttons.Heavy);
            SB.Append(" Heavy,");
        }
        if ((input & CONST.INPUT_BLOCK) != 0) {
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
                    input |= CONST.INPUT_UP;
                }
                if (Input.GetKey(KeyCode.S)) {
                    input |= CONST.INPUT_DOWN;
                }
                if (Input.GetKey(KeyCode.A)) {
                    input |= CONST.INPUT_LEFT;
                }
                if (Input.GetKey(KeyCode.D)) {
                    input |= CONST.INPUT_RIGHT;
                }
                if (Input.GetKey(KeyCode.LeftShift)) {
                    input |= CONST.INPUT_DASH;
                }
                if (Input.GetKey(KeyCode.E)) {
                    input |= CONST.INPUT_LIT_ATTACK;
                }
                if (Input.GetKey(KeyCode.R)) {
                    input |= CONST.INPUT_MED_ATTACK;
                }
                if (Input.GetKey(KeyCode.T)) {
                    input |= CONST.INPUT_HEV_ATTACK;
                }
                if (Input.GetKey(KeyCode.LeftControl)) {
                    input |= CONST.INPUT_BLOCK;
                }
                break;
            case 1:
                if (Input.GetKey(KeyCode.O)) {
                    input |= CONST.INPUT_UP;
                }
                if (Input.GetKey(KeyCode.L)) {
                    input |= CONST.INPUT_DOWN;
                }
                if (Input.GetKey(KeyCode.K)) {
                    input |= CONST.INPUT_LEFT;
                }
                if (Input.GetKey(KeyCode.Semicolon)) {
                    input |= CONST.INPUT_RIGHT;
                }
                if (Input.GetKey(KeyCode.RightShift)) {
                    input |= CONST.INPUT_DASH;
                }
                if (Input.GetKey(KeyCode.I)) {
                    input |= CONST.INPUT_LIT_ATTACK;
                }
                if (Input.GetKey(KeyCode.U)) {
                    input |= CONST.INPUT_MED_ATTACK;
                }
                if (Input.GetKey(KeyCode.Y)) {
                    input |= CONST.INPUT_HEV_ATTACK;
                }
                if (Input.GetKey(KeyCode.RightControl)) {
                    input |= CONST.INPUT_BLOCK;
                }
                break;
                
        }
        
        return input;
    }
}