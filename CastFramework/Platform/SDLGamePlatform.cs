using System;
using System.Diagnostics;
using static CastFramework.SDL;


namespace CastFramework
{
    internal partial class SDLGamePlatform : GamePlatform
    {
        private bool is_fullscreen;
        private int prev_win_h;

        private int prev_win_w;
        private int screen_h;
        private int screen_w;
        private IntPtr window;
        private bool is_active = true;

        public override bool IsFullscreen => is_fullscreen;
        public override bool IsActive => is_active;

        public override void Init(string title, int width, int height, bool fullscreen)
        {
            prev_win_w = width;
            prev_win_h = height;
            is_fullscreen = fullscreen;

            const uint init_flags = SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER |
                                    SDL_INIT_HAPTIC;

            SDL_SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");

            var sw = Stopwatch.StartNew();

            SDL_Init(init_flags);

            var windowFlags =
                SDL_WindowFlags.SDL_WINDOW_HIDDEN;

            if (CurrentPlatform.RunningOS != OS.Win)
            {
                windowFlags |= SDL_WindowFlags.SDL_WINDOW_OPENGL;

                SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
                SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            }

            if (fullscreen) windowFlags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP;

            window = SDL_CreateWindow(
                title,
                SDL_WINDOWPOS_CENTERED_MASK,
                SDL_WINDOWPOS_CENTERED_MASK,
                width,
                height,
                windowFlags
            );


            if (window == IntPtr.Zero)
            {
                SDL_Quit();
                throw new Exception(SDL_GetError());
            }

            if (fullscreen)
            {
                SDL_GetDisplayMode(0, 0, out var mode);

                screen_w = mode.w;
                screen_h = mode.h;
            }
            else
            {
                screen_w = prev_win_w;
                screen_h = prev_win_h;
            }

            sw.Stop();

            InitKeyboard();
        }

        public override void LoadContent()
        {
            InitGamepad();
        }

        public override IntPtr GetRenderSurfaceHandle()
        {
            var info = new SDL_SysWMinfo();


            SDL_GetWindowWMInfo(window, ref info);

            switch (CurrentPlatform.RunningOS)
            {
                case OS.Win:
                    return info.info.win.window;

                case OS.Linux:
                    return info.info.x11.window;

                case OS.OSX:
                    return info.info.cocoa.window;
            }

            throw new Exception(
                "SDLGamePlatform [GetRenderSurfaceHandle]: " +
                "Invalid OS, could not retrive native renderer surface handle.");
        }



        public override void Shutdown()
        {
            Console.WriteLine(" > Closing GamePlatform");

            SDL_Quit();
        }

        public override void PollEvents()
        {
            while (SDL_PollEvent(out var ev) == 1)
                switch (ev.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        OnQuit?.Invoke();
                        break;

                    case SDL_EventType.SDL_KEYDOWN:
                        AddKey((int)ev.key.keysym.sym);
                        break;

                    case SDL_EventType.SDL_KEYUP:
                        RemoveKey((int)ev.key.keysym.sym);
                        break;

                    case SDL_EventType.SDL_TEXTINPUT:

                        break;

                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        SetMouseButtonState(ev.button.button, true);
                        break;

                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        SetMouseButtonState(ev.button.button, false);
                        break;

                    case SDL_EventType.SDL_MOUSEWHEEL:
                        TriggerMouseScroll(ev.wheel.y * 120);
                        break;

                    case SDL_EventType.SDL_CONTROLLERDEVICEADDED:

                        ProcessGamepadAdd(ev.cdevice.which);

                        break;

                    case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:

                        ProcessGamepadRemove(ev.cdevice.which);

                        break;

                    case SDL_EventType.SDL_WINDOWEVENT:
                        switch (ev.window.windowEvent)
                        {
                            case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                                break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                                OnQuit?.Invoke();
                                break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:

                                var w = ev.window.data1;
                                var h = ev.window.data2;

                                if (screen_w != w || screen_h != h)
                                {
                                }

                                screen_w = w;
                                screen_h = h;

                                OnWinResized?.Invoke(w, h);
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:

                                OnMouseOver?.Invoke();

                                break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:

                                OnMouseLeave?.Invoke();

                                break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:

                                is_active = false;

                                break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:

                                is_active = true;

                                break;

                        }

                        break;
                }
        }

        public override void GetScreenSize(out int w, out int h)
        {
            w = screen_w;
            h = screen_h;
        }

        public override void SetScreenSize(int w, int h)
        {
            if (is_fullscreen) return;

            prev_win_w = w;
            prev_win_h = h;

            SDL_SetWindowSize(window, w, h);
            SDL_SetWindowPosition(window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
        }

        public override void SetTitle(string title)
        {
            SDL_SetWindowTitle(window, title);
        }

        public override void ShowCursor(bool show)
        {
            SDL_ShowCursor(show ? 1 : 0);
        }

        public override void ShowScreen(bool show)
        {
            if (show)
                SDL_ShowWindow(window);
            else
                SDL_HideWindow(window);
        }

        public override void SetFullscreen(bool enabled)
        {
            if (is_fullscreen != enabled)
            {
                SDL_SetWindowFullscreen(window, (uint)(enabled ? SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0));

                is_fullscreen = enabled;

                if (!is_fullscreen) SetScreenSize(prev_win_w, prev_win_h);
            }
        }
    }
}
