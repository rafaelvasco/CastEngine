using System.Numerics;

namespace CastFramework
{
    public static class Input
    {
        public static bool AnyKeyDown => kb_current_state.PressedCount > 0;

        public static bool IsMouseOver { get; internal set; }

        public static int MouseWheel => mouse_wheel;

        private static GamepadState gp_current_state;
        private static GamepadState gp_prev_state;

        private static KeyState kb_current_state;
        private static KeyState kb_prev_state;

        private static MouseState ms_current_state;
        private static MouseState ms_prev_state;

        private static int mouse_wheel;

        internal static Point mouse_position = new Point(-1, -1);

        private static GamePlatform platform;

        internal static void Init(GamePlatform game_platform)
        {
            platform = game_platform;
            platform.OnMouseScroll += OnPlatformMouseScroll;
            platform.OnMouseOver += OnPlatformMouseOver;
            platform.OnMouseLeave += OnPlatformMouseLeave;
        }

        private static void OnPlatformMouseLeave()
        {
            IsMouseOver = false;
        }

        private static void OnPlatformMouseOver()
        {
            IsMouseOver = true;
        }

        public static GamepadDeadZoneMode GamepadDeadZoneMode
        {
            get => platform.GamepadDeadZoneMode;
            set => platform.GamepadDeadZoneMode = value;
        }

        public static Vector2 LeftThumbstickAxis => gp_current_state.Thumbsticks.Left;

        public static Vector2 RightThumbstickAxis => gp_current_state.Thumbsticks.Right;

        public static float LeftTriggerValue => gp_current_state.Triggers.Left;

        public static float RightTriggerValue => gp_current_state.Triggers.Right;

        private static void OnPlatformMouseScroll(int value)
        {
            mouse_wheel += value;
        }

        internal static void Update()
        {
            kb_prev_state = kb_current_state;
            kb_current_state = platform.GetKeyboardState();

            ms_prev_state = ms_current_state;
            ms_current_state = platform.GetMouseState();

            platform.GetMousePosition(out mouse_position);

            gp_prev_state = gp_current_state;
            gp_current_state = platform.GetGamepadState();
        }

        internal static void PostUpdate()
        {
            mouse_wheel = 0;
        }

        public static bool KeyDown(Key key)
        {
            return kb_current_state[key];
        }

        public static bool KeyPressed(Key key)
        {
            return kb_current_state[key] && !kb_prev_state[key];
        }

        public static bool KeyReleased(Key key)
        {
            return !kb_current_state[key] && kb_prev_state[key];
        }

        public static bool MouseDown(MouseButton button)
        {
            return ms_current_state[button];
        }

        public static bool MousePressed(MouseButton button)
        {
            return ms_current_state[button] && !ms_prev_state[button];
        }

        public static bool MouseReleased(MouseButton button)
        {
            return !ms_current_state[button] && ms_prev_state[button];
        }

        public static bool ButtonDown(GamepadButton button)
        {
            return gp_current_state[button];
        }

        public static bool ButtonPressed(GamepadButton button)
        {
            return gp_current_state[button] && !gp_prev_state[button];
        }

        public static bool ButtonReleased(GamepadButton button)
        {
            return !gp_current_state[button] && gp_prev_state[button];
        }
    }
}
