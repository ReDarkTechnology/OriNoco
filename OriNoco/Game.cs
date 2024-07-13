using System.Collections.Generic;

using Point = System.Drawing.Point;
using rlImGui_cs;

using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Images;
using Raylib_CSharp.Logging;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace OriNoco
{
    public static class Game
    {
        private static Point _WindowSize = new Point(1280, 720);
        public static Point WindowSize {
            get => new (Window.GetScreenWidth(), Window.GetScreenHeight());
            set
            {
                _WindowSize = value;
                Window.SetSize(_WindowSize.X, _WindowSize.Y);
            }
        }

        public static List<Scene> Scenes { get; private set; } = new List<Scene>();

        public static void Start()
        {
            Raylib.SetConfigFlags(ConfigFlags.Msaa4XHint | ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            Window.Init(_WindowSize.X, _WindowSize.Y, "Orinoco");
            {
                Window.SetMinSize(640, 360);
                Window.SetIcon(Image.LoadFromMemory(".png", BuiltResources.OriNoco));
                Logger.SetTraceLogLevel(TraceLogLevel.Warning);
                Time.SetTargetFPS(100);
                AudioDevice.Init();
                {
                    AudioDevice.SetMasterVolume(1f);
                    rlImGui.Setup(true);
                    {
                        foreach (var scene in Scenes)
                            scene.Init();

                        while (!Window.ShouldClose())
                        {
                            foreach (var scene in Scenes)
                                scene.Update();

                            Graphics.BeginDrawing();
                            foreach (var scene in Scenes)
                                scene.Draw();
                            Graphics.EndDrawing();
                        }

                        foreach (var scene in Scenes)
                            scene.Shutdown();
                    }
                    rlImGui.Shutdown();
                }
                AudioDevice.Close();
            }
            Window.Close();
        }

        public static void RegisterScene(Scene scene) => Scenes.Add(scene);
    }
}