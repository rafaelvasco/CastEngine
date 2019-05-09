using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CastFramework
{
    internal class SDL2GamepadDevice
    {
        public IntPtr Device;
        public int DeviceId;
        public IntPtr HapticDevice;
        public int HapticType;
        public string Name;

        public GamepadState State;

        public SDL2GamepadDevice()
        {
            State = new GamepadState
            {
                Thumbsticks = new GamepadThumbsticks(),
                Triggers = new GamePadTriggers()
            };
        }
    }

    internal partial class SDLGamePlatform
    {
        private static SDL.SDL_HapticEffect _haptic_left_right = new SDL.SDL_HapticEffect
        {
            type = SDL.SDL_HAPTIC_LEFTRIGHT,
            leftright = new SDL.SDL_HapticLeftRight
            {
                type = SDL.SDL_HAPTIC_LEFTRIGHT,
                length = SDL.SDL_HAPTIC_INFINITY,
                large_magnitude = ushort.MaxValue,
                small_magnitude = ushort.MaxValue
            }
        };

        private SDL2GamepadDevice gamepad_device;

        private GamepadState gamepad_state;

        public bool GamepadPresent => gamepad_device != null;

        public string GamepadName => gamepad_device != null ? gamepad_device.Name : "Null";

        public override GamepadDeadZoneMode GamepadDeadZoneMode { get; set; } = GamepadDeadZoneMode.IndependentAxis;

        private void InitGamepad()
        {
            var gamepad_db_file = Game.Instance.ContentManager.Get<TextFile>("gamecontrollerdb");

            foreach (var line in gamepad_db_file.Text)
                if (!line.StartsWith("#"))
                    SDL.SDL_GameControllerAddMapping(line);
        }

        public override ref readonly GamepadState GetGamepadState()
        {
            if (gamepad_device == null) return ref GamepadState.Default;

            var device = gamepad_device.Device;

            var thumbsticks_state = new GamepadThumbsticks(
                new Vector2(
                    NormalizeAxisValue(SDL.SDL_GameControllerGetAxis(device,
                        SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX)),
                    NormalizeAxisValue(SDL.SDL_GameControllerGetAxis(device,
                        SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY))
                ),
                new Vector2(
                    NormalizeAxisValue(SDL.SDL_GameControllerGetAxis(device,
                        SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX)),
                    NormalizeAxisValue(SDL.SDL_GameControllerGetAxis(device,
                        SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY))
                ),
                GamepadDeadZoneMode
            );

            var triggers_state = new GamePadTriggers(
                NormalizeAxisValue(SDL.SDL_GameControllerGetAxis(device,
                    SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT)),
                NormalizeAxisValue(SDL.SDL_GameControllerGetAxis(device,
                    SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT))
            );

            var buttons_state = gamepad_device.State.ButtonsState =
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) == 1
                    ? GamepadButton.A
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B) == 1
                    ? GamepadButton.B
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK) == 1
                    ? GamepadButton.Back
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE) == 1
                    ? GamepadButton.BigButton
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER) == 1
                    ? GamepadButton.LeftShoulder
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER) == 1
                    ? GamepadButton.RightShoulder
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK) ==
                 1
                    ? GamepadButton.LeftStick
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK) ==
                 1
                    ? GamepadButton.RightStick
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START) == 1
                    ? GamepadButton.Start
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X) == 1
                    ? GamepadButton.X
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y) == 1
                    ? GamepadButton.Y
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP) ==
                 1
                    ? GamepadButton.DPadUp
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN) == 1
                    ? GamepadButton.DPadDown
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT) == 1
                    ? GamepadButton.DPadLeft
                    : 0) |
                (SDL.SDL_GameControllerGetButton(device,
                     SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT) == 1
                    ? GamepadButton.DPadRight
                    : 0) |
                (triggers_state.Left > 0f ? GamepadButton.LeftTrigger : 0) |
                (triggers_state.Right > 0f ? GamepadButton.RightTrigger : 0);


            gamepad_state.ButtonsState = buttons_state;
            gamepad_state.Thumbsticks = thumbsticks_state;
            gamepad_state.Triggers = triggers_state;

            return ref gamepad_state;
        }

        public override bool SetGamepadVibration(float left_motor, float right_motor)
        {
            if (gamepad_device == null) return false;

            var gamepad = gamepad_device;

            if (gamepad.HapticType == 0) return false;

            if (left_motor <= 0.0f && right_motor <= 0.0f)
            {
                SDL.SDL_HapticStopAll(gamepad.HapticDevice);
            }
            else if (gamepad.HapticType == 1)
            {
                _haptic_left_right.leftright.large_magnitude = (ushort)(65535f * left_motor);
                _haptic_left_right.leftright.small_magnitude = (ushort)(65535f * right_motor);

                SDL.SDL_HapticUpdateEffect(gamepad.HapticDevice, 0, ref _haptic_left_right);
                SDL.SDL_HapticRunEffect(gamepad.HapticDevice, 0, 1);
            }
            else if (gamepad.HapticType == 2)
            {
                SDL.SDL_HapticRumblePlay(gamepad.HapticDevice, Math.Max(left_motor, right_motor),
                    SDL.SDL_HAPTIC_INFINITY);
            }

            return true;
        }

        private void ProcessGamepadAdd(int device_id)
        {
            if (gamepad_device != null) return;

            var gamepad = new SDL2GamepadDevice
            {
                DeviceId = device_id,
                Device = SDL.SDL_GameControllerOpen(device_id),
                HapticDevice = SDL.SDL_HapticOpen(device_id)
            };

            if (gamepad.Device == IntPtr.Zero) return;

            gamepad_device = gamepad;

            if (gamepad_device.HapticDevice == IntPtr.Zero) return;

            try
            {
                if (SDL.SDL_HapticEffectSupported(gamepad_device.HapticDevice, ref _haptic_left_right) == 1)
                {
                    SDL.SDL_HapticNewEffect(gamepad_device.HapticDevice, ref _haptic_left_right);
                    gamepad_device.HapticType = 1;
                }
                else if (SDL.SDL_HapticRumbleSupported(gamepad_device.HapticDevice) == 1)
                {
                    SDL.SDL_HapticRumbleInit(gamepad_device.HapticDevice);
                    gamepad_device.HapticType = 2;
                }
                else
                {
                    SDL.SDL_HapticClose(gamepad_device.HapticDevice);
                    gamepad_device.HapticDevice = IntPtr.Zero;
                }

                gamepad_device.Name = SDL.SDL_GameControllerName(gamepad.Device);
            }
            catch
            {
                SDL.SDL_HapticClose(gamepad_device.HapticDevice);
                gamepad_device.HapticDevice = IntPtr.Zero;
                SDL.SDL_ClearError();
            }
        }

        private void ProcessGamepadRemove(int device_id)
        {
            if (gamepad_device.DeviceId == device_id) DisposeGamepadDevice();
        }

        private void DisposeGamepadDevice()
        {
            if (gamepad_device.HapticType > 0) SDL.SDL_HapticClose(gamepad_device.HapticDevice);

            SDL.SDL_GameControllerClose(gamepad_device.Device);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeAxisValue(int axis)
        {
            if (axis < 0) return axis / 32768f;

            return axis / 32767f;
        }
    }
}
