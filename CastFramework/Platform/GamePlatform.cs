using System;

namespace CastFramework
{
    internal abstract class GamePlatform
    {
        public Action OnQuit;
        public Action<int> OnMouseScroll;
        public Action<int, int> OnWinResized;
        public Action OnMouseOver;
        public Action OnMouseLeave;

        public abstract bool IsFullscreen { get; }

        public abstract bool IsActive { get; }

        public abstract void Init(string title, int width, int height, bool fullscreen);

        public abstract void LoadContent();

        public abstract void Shutdown();

        public abstract void PollEvents();

        public abstract void GetScreenSize(out int w, out int h);

        public abstract void SetScreenSize(int w, int h);

        public abstract void SetTitle(string title);

        public abstract void ShowCursor(bool show);

        public abstract void ShowScreen(bool show);

        public abstract void SetFullscreen(bool enabled);

        public abstract IntPtr GetRenderSurfaceHandle();

        public abstract GamepadDeadZoneMode GamepadDeadZoneMode { get; set; }

        public abstract ref readonly KeyState GetKeyboardState();

        public abstract ref readonly MouseState GetMouseState();

        public abstract void GetMousePosition(out Point pos);

        public abstract ref readonly GamepadState GetGamepadState();

        public abstract bool SetGamepadVibration(float left_motor, float right_motor);

    }
}
