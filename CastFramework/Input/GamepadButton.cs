using System;

namespace CastFramework
{
    [Flags]
    public enum GamepadButton
    {
        DPadUp = 1 << 0,
        DPadDown = 1 << 1,
        DPadLeft = 1 << 2,
        DPadRight = 1 << 3,

        Start = 1 << 4,
        Back = 1 << 5,

        LeftStick = 1 << 6,
        RightStick = 1 << 7,

        LeftShoulder = 1 << 8,
        RightShoulder = 1 << 9,

        BigButton = 1 << 10,

        A = 1 << 11,
        B = 1 << 12,
        X = 1 << 13,
        Y = 1 << 14,

        LeftThumbstickLeft = 1 << 15,
        LeftThumbstickRight = 1 << 16,
        LeftThumbstickUp = 1 << 17,
        LeftThumbstickDown = 1 << 18,

        RightThumbstickLeft = 1 << 19,
        RightThumbstickRight = 1 << 20,
        RightThumbstickUp = 1 << 21,
        RightThumbstickDown = 1 << 22,

        LeftTrigger = 1 << 23,
        RightTrigger = 1 << 24
    }
}
