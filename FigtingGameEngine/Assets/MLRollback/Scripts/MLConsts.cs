﻿using Unity.Mathematics.FixedPoint;

public static class MLConsts {
    public const int MAX_BLOCK = 50;
    public const int MAX_HEALTH = 100;
    public const int PLAY_AREA_WIDTH = 16;
    public const int PLAY_AREA_HEIGHT = 99;
    public const int PLAY_AREA_GROUND = 0;
    public const int MAX_PLAYERS = 2;

    public const int INPUT_LEFT = 1 << 0;
    public const int INPUT_RIGHT = 1 << 1;
    public const int INPUT_UP = 1 << 2;
    public const int INPUT_DOWN = 1 << 3;
    public const int INPUT_DASH = 1 << 4;
    public const int INPUT_LIT_ATTACK = 1 << 5;
    public const int INPUT_MED_ATTACK = 1 << 6;
    public const int INPUT_HEV_ATTACK = 1 << 7;
    public const int INPUT_BLOCK = 1 << 8;

    public const int STARTING_POSITION_X = 4;
    public const int MAX_ROUND_TIME = 99;
    public const int END_GAME_DELAY = 5;
    public const int FPS = 60;

    public static int GetLagAmount(LagTypes type) {
        switch (type) {
            case LagTypes.JumpStart:
                return 3;
            case LagTypes.Dash:
                return 20;
            case LagTypes.LandingLag:
                return 5;
            default:
                return 0;
        }
    }
}