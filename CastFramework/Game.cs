using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace CastFramework
{
    [Serializable]
    public struct GameProperties
    {
        [DataMember(Name = "title", IsRequired = false)]
        public string Title;

        [DataMember(Name = "frame_rate", IsRequired = false)]
        public int FrameRate;

        [DataMember(Name = "canvas_width", IsRequired = false)]
        public int CanvasWidth;

        [DataMember(Name = "canvas_height", IsRequired = false)]
        public int CanvasHeight;

        [DataMember(Name = "fullscreen", IsRequired = false)]
        public bool Fullscreen;

        [DataMember(Name = "preload_paks", IsRequired = false)]
        public string[] PreloadResourcePaks;

        public static GameProperties Default()
        {
            return new GameProperties
            {
                CanvasWidth = 800,
                CanvasHeight = 600,
                FrameRate = 60,
                Fullscreen = false,
                Title = "Game"
            };
        }
    }

    public class GameTime
    {
        public TimeSpan TotalGameTime { get; internal set; }

        public TimeSpan ElapsedGameTime { get; internal set; }

        public bool IsRunningSlowly { get; internal set; }

        public GameTime()
        {
            TotalGameTime = TimeSpan.Zero;
            ElapsedGameTime = TimeSpan.Zero;
            IsRunningSlowly = false;
        }

        public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly = false)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
            IsRunningSlowly = isRunningSlowly;
        }

    }

    public class Game : IDisposable
    {
        public string Title { get; }

        public readonly Canvas Canvas;

        public bool IsActive => Platform.IsActive;

        public TimeSpan InactiveSleepTime
        {
            get => inactive_sleep_time;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    value = TimeSpan.FromSeconds(0.02);
                }

                inactive_sleep_time = value;
            }
        }

        public TimeSpan MaxElapsedTime
        {
            get => max_elapsed_time;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    value = TimeSpan.FromMilliseconds(500);
                }

                if (value < target_elapsed_time)
                {
                    value = target_elapsed_time;
                }

                max_elapsed_time = value;
            }
        }

        public TimeSpan TargetElapsedTime
        {
            get => target_elapsed_time;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    value = TimeSpan.FromTicks(166667);
                }

                target_elapsed_time = value;
            }
        }

        public bool IsFixedTimeStep
        {
            get => is_fixed_timestep;
            set => is_fixed_timestep = value;
        }

        public readonly ContentManager ContentManager;

        internal readonly GraphicsContext GraphicsContext;

        internal readonly GamePlatform Platform;

        private bool full_screen;

        private int requested_screen_h;

        private int requested_screen_w;

        private bool screen_resize_requested;

        private bool toggle_fullscreen_requested;

        private readonly string[] paks_to_preload;

        private bool is_fixed_timestep = true;

        private TimeSpan target_elapsed_time = TimeSpan.FromTicks(166667);

        private TimeSpan inactive_sleep_time = TimeSpan.FromSeconds(0.02);

        private TimeSpan max_elapsed_time = TimeSpan.FromMilliseconds(500);

        private TimeSpan accum_elapsed_time;

        private readonly GameTime game_time = new GameTime();

        private Stopwatch game_timer;

        private long previous_ticks;

        private int update_frame_lag;

        private bool initialized;

        private List<GameComponent> game_components;

        /* ========================================================================================================== */

        public Game()
        {
            Instance = this;

            var props = LoadGameProperties();

            var timer = Stopwatch.StartNew();

            Platform = new SDLGamePlatform();
            Platform.OnQuit += OnPlatformQuit;
            Platform.OnWinResized += OnScreenResized;

            Title = props.Title;

            Platform.Init(props.Title, props.CanvasWidth, props.CanvasHeight, props.Fullscreen);

            Console.WriteLine($" > Platform Init took: {timer.Elapsed.TotalSeconds.ToString()}");

            Platform.GetScreenSize(out var screen_w, out var screen_h);

            GraphicsContext = new GraphicsContext(Platform.GetRenderSurfaceHandle(), screen_w, screen_h);

            timer.Restart();

            ContentManager = new ContentManager();

            if (props.PreloadResourcePaks != null)
            {
                paks_to_preload = props.PreloadResourcePaks;
            }

            Platform.LoadContent();

            Console.WriteLine($" > Load Content took: {timer.Elapsed.TotalSeconds.ToString()}");

            Canvas = new Canvas(GraphicsContext, props.CanvasWidth, props.CanvasHeight, 2048);

            Input.Init(Platform);

            //MediaPlayer.Init();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            /* CONVENIENCE GLOBAL REFERENCES */
            Scene.Game = this;
            Scene.Content = ContentManager;
            Scene.Canvas = Canvas;

        }

        public static Game Instance { get; private set; }

        public Scene CurrentScene { get; private set; }

        public bool Running { get; internal set; }

        public bool Fullscreen
        {
            get => full_screen;
            set
            {
                if (full_screen == value) return;

                full_screen = value;

                if (initialized)
                {
                    toggle_fullscreen_requested = true;
                }
                else
                {
                    Platform.SetFullscreen(full_screen); //TODO: Check Necessity of Delaying Screen resize update
                }

            }
        }

        public Size ScreenSize
        {
            get
            {
                Platform.GetScreenSize(out var w, out var h);

                return new Size(w, h);
            }
            set
            {
                Platform.GetScreenSize(out var w, out var h);

                if (value.W == w && value.H == h) return;

                if (initialized)
                {
                    screen_resize_requested = true;

                    requested_screen_w = value.W;
                    requested_screen_h = value.H;
                }
                else
                {
                    Platform.SetScreenSize(value.W, value.H);
                }


            }
        }

        public void Dispose()
        {
            CurrentScene.End();
            CurrentScene.Unload();
            ContentManager.FreeEverything();
            GraphicsContext.Dispose();
            //MediaPlayer.Shutdown();
            Platform.Shutdown();
        }

        public void Start(Scene scene = null)
        {
            if (Running) return;

            if (paks_to_preload != null)
            {
                foreach (var pak in paks_to_preload)
                {
                    ContentManager.LoadContentPack(pak);
                }
            }

            CurrentScene = scene ?? new EmptyScene();

            CurrentScene.Load();

            CurrentScene.Init();

            CurrentScene.Update(game_time);

            if (game_components != null)
            {
                UpdateComponents(game_time);
            }

            Canvas.BeginRendering();

            CurrentScene.Draw(Canvas, game_time);

            if(game_components != null)
            {
                DrawComponents(Canvas);
            }

            Canvas.EndRendering();

            GraphicsContext.SwapBuffers();

            Running = true;

            Platform.ShowScreen(true);

            Tick();

            initialized = true;
        }

        public void Quit()
        {
            Running = false;
        }

        public void ShowCursor(bool show)
        {
            Platform.ShowCursor(show);
        }

        public void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
        }

        public void AddComponent(GameComponent component)
        {
            if(this.game_components == null)
            {
                this.game_components = new List<GameComponent>();
            }

            this.game_components.Add(component);
        }

        public void GoToScene(Scene scene)
        {
            
            CurrentScene.End();

            if(!CurrentScene.KeepContentOnMemory)
            {
                CurrentScene.Unload();
            }

            CurrentScene = scene;

            CurrentScene.Load();

            CurrentScene.Init();

            CurrentScene.Update(game_time);

        }

        internal void ThrowError(string message, params object[] args)
        {
            StringBuilder error = new StringBuilder();

            string msg = string.Format(message, args);

            error.AppendLine("::::::: Cast Engine ERROR Log ::::::: ");

            error.Append("Message: ");

            error.AppendLine(msg);

            var destination_path = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "Cast Engine", Title, "ERRORLOG.txt");

            var file = new FileInfo(destination_path);

            file.Directory?.Create();

            File.WriteAllText(file.FullName, error.ToString());

            throw new Exception(msg);
        }

        private static GameProperties LoadGameProperties()
        {
            GameProperties props;

            try
            {
                props = JsonIO.Load<GameProperties>("config.json");

                if (props.Title == null)
                {
                    props.Title = "Game";
                }

                if (props.FrameRate == 0)
                {
                    props.FrameRate = 60;
                }

                if (props.CanvasWidth == 0)
                {
                    props.CanvasWidth = 800;
                }

                if (props.CanvasHeight == 0)
                {
                    props.CanvasHeight = 600;
                }

            }
            catch (Exception)
            {
                props = GameProperties.Default();
            }

            return props;
        }

        private void OnScreenResized(int w, int h)
        {
            int old_w = Canvas.Width;
            int old_h = Canvas.Height;

            Canvas.OnScreenResized(w, h);

            if (old_w != Canvas.Width || old_h != Canvas.Height)
            {
                CurrentScene?.OnCanvasResize(Canvas.Width, Canvas.Height);
            }
        }

        private void OnPlatformQuit()
        {
            Quit();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateComponents(GameTime time)
        {
            for(var i = 0; i < game_components.Count; i++)
            {
                game_components[i].Update(time);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawComponents(Canvas canvas)
        {
            for (var i = 0; i < game_components.Count; i++)
            {
                game_components[i].Draw(canvas);
            }
        }

        private void Tick()
        {

            game_timer = Stopwatch.StartNew();

            while (Running)
            {

                RetryTick:

                if (!IsActive && inactive_sleep_time.TotalMilliseconds > 1.0)
                {
                    Thread.Sleep((int)inactive_sleep_time.TotalMilliseconds);
                }

                var currentTicks = game_timer.Elapsed.Ticks;

                accum_elapsed_time += TimeSpan.FromTicks(currentTicks - previous_ticks);

                if (accum_elapsed_time > max_elapsed_time)
                {
                    accum_elapsed_time = max_elapsed_time;
                }

                previous_ticks = currentTicks;

                if (is_fixed_timestep && accum_elapsed_time < target_elapsed_time)
                {
                    goto RetryTick;
                }

                if (is_fixed_timestep)
                {
                    game_time.ElapsedGameTime = target_elapsed_time;

                    var stepCount = 0;

                    while (accum_elapsed_time >= target_elapsed_time && Running)
                    {
                        game_time.TotalGameTime += target_elapsed_time;
                        accum_elapsed_time -= target_elapsed_time;
                        ++stepCount;

                        Platform.PollEvents();

                        Input.Update();

                        CurrentScene.Update(game_time);

                        if(game_components != null)
                        {
                            UpdateComponents(game_time);
                        }

                        Input.PostUpdate();

                    }

                    update_frame_lag += Calc.Max(0, stepCount - 1);

                    if (game_time.IsRunningSlowly)
                    {
                        if (update_frame_lag == 0)
                        {
                            game_time.IsRunningSlowly = false;
                        }
                    }
                    else if (update_frame_lag >= 5)
                    {
                        game_time.IsRunningSlowly = true;
                    }

                    if (stepCount == 1 && update_frame_lag > 0)
                    {
                        update_frame_lag--;
                    }

                    game_time.ElapsedGameTime = TimeSpan.FromTicks(target_elapsed_time.Ticks * stepCount);
                }
                else
                {
                    game_time.ElapsedGameTime = accum_elapsed_time;
                    game_time.TotalGameTime += accum_elapsed_time;
                    accum_elapsed_time = TimeSpan.Zero;

                    Platform.PollEvents();

                    Input.Update();

                    CurrentScene.Update(game_time);

                    if(game_components != null)
                    {
                        UpdateComponents(game_time);
                    }

                    Input.PostUpdate();
                }

                Canvas.BeginRendering();

                CurrentScene.Draw(Canvas, game_time);

                if(game_components != null)
                {
                    DrawComponents(Canvas);
                }

                Canvas.EndRendering();

                GraphicsContext.SwapBuffers();

                if (toggle_fullscreen_requested)
                {
                    toggle_fullscreen_requested = false;

                    Platform.SetFullscreen(full_screen);
                }
                else if (screen_resize_requested)
                {
                    screen_resize_requested = false;

                    Platform.SetScreenSize(requested_screen_w, requested_screen_h);
                }

            }

#if DEBUG
            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);

            Console.WriteLine(
                $"Gen-0: {gen0.ToString()} | Gen-1: {gen1.ToString()} | Gen-2: {gen2.ToString()}"
            );
#endif
        }
    }
}
