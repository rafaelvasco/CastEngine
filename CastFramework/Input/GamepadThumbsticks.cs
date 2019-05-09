using System.Numerics;

namespace CastFramework
{
    public struct GamepadThumbsticks
    {
        public Vector2 Left { get; private set; }

        public Vector2 Right { get; private set; }

        public GamepadButton DirectionalState { get; private set; }

        private const float LeftThumbDeadZone = 0.24f;
        private const float RightThumbDeadZone = 0.265f;

        public GamepadThumbsticks(Vector2 left, Vector2 right, GamepadDeadZoneMode deadZoneMode)
        {
            Left = left;
            Right = right;

            DirectionalState = 0;

            ApplyDeadZone(deadZoneMode);

            if (deadZoneMode == GamepadDeadZoneMode.Circular)
            {
                ApplyCircularClamp();
            }
            else
            {
                ApplySquareClamp();
            }

            SetDirectionalState(Left, Right);
        }


        private void ApplyDeadZone(GamepadDeadZoneMode deadZoneMode)
        {
            switch (deadZoneMode)
            {
                case GamepadDeadZoneMode.None:
                    break;
                case GamepadDeadZoneMode.IndependentAxis:
                    Left = ExcludeIndependentAxisDeadZone(Left, LeftThumbDeadZone);
                    Right = ExcludeIndependentAxisDeadZone(Right, RightThumbDeadZone);
                    break;
                case GamepadDeadZoneMode.Circular:
                    Left = ExcludeCircularDeadZone(Left, LeftThumbDeadZone);
                    Right = ExcludeCircularDeadZone(Right, RightThumbDeadZone);
                    break;
            }
        }

        private Vector2 ExcludeCircularDeadZone(Vector2 value, float deadzone)
        {
            var originalLength = value.Length();

            if (originalLength <= deadzone)
            {
                return Vector2.Zero;
            }

            var newLength = (originalLength - deadzone) / (1f - deadzone);
            return value * (newLength / originalLength);
        }

        private Vector2 ExcludeIndependentAxisDeadZone(Vector2 value, float deadzone)
        {
            return new Vector2(ExcludeAxisDeadZone(value.X, deadzone), ExcludeAxisDeadZone(value.Y, deadzone));
        }

        private float ExcludeAxisDeadZone(float value, float deadzone)
        {
            if (value < -deadzone)
            {
                value += deadzone;
            }
            else if (value > deadzone)
            {
                value -= deadzone;
            }
            else
            {
                return 0f;
            }

            return value / (1f - deadzone);
        }


        private void SetDirectionalState(Vector2 left, Vector2 right)
        {
            DirectionalState = 0;

            if (left.X < -LeftThumbDeadZone)
            {
                DirectionalState |= GamepadButton.LeftThumbstickLeft;
            }
            else if (left.X > LeftThumbDeadZone)
            {
                DirectionalState |= GamepadButton.LeftThumbstickRight;
            }

            if (left.Y < -LeftThumbDeadZone)
            {
                DirectionalState |= GamepadButton.LeftThumbstickUp;
            }
            else if (left.Y > LeftThumbDeadZone)
            {
                DirectionalState |= GamepadButton.LeftThumbstickDown;
            }

            if (right.X < -RightThumbDeadZone)
            {
                DirectionalState |= GamepadButton.RightThumbstickLeft;
            }
            else if (right.X > RightThumbDeadZone)
            {
                DirectionalState |= GamepadButton.RightThumbstickRight;
            }

            if (right.Y < -RightThumbDeadZone)
            {
                DirectionalState |= GamepadButton.RightThumbstickUp;
            }
            else if (right.Y > RightThumbDeadZone)
            {
                DirectionalState |= GamepadButton.RightThumbstickDown;
            }
        }

        private void ApplySquareClamp()
        {
            Left = new Vector2(Calc.Clamp(Left.X, -1f, 1f), Calc.Clamp(Left.Y, -1f, 1f));
            Right = new Vector2(Calc.Clamp(Right.X, -1f, 1f), Calc.Clamp(Right.Y, -1f, 1f));
        }

        private void ApplyCircularClamp()
        {
            if (Left.LengthSquared() > 1f)
            {
                Left = Left.Normalized();
            }

            if (Right.LengthSquared() > 1f)
            {
                Right = Right.Normalized();
            }
        }
    }
}
