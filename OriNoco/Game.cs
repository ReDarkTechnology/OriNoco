using System.Collections.Generic;

using Point = System.Drawing.Point;
using rlImGui_cs;

using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Images;
using Raylib_CSharp.Logging;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;
using OriNoco.Tweening;
using System.Numerics;
using OriNoco.UI;

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

        private static Point _PreviousSize = new Point(1280, 720);

        public static Vector2 WindowSizeF
        {
            get => new(Window.GetScreenWidth(), Window.GetScreenHeight());
            set { Window.SetSize((int)value.X, (int)value.Y); }
        }

        public static bool StopRunning;
        public static List<Scene> Scenes { get; private set; } = new List<Scene>();

        public static void Start()
        {
            Logger.SetTraceLogLevel(TraceLogLevel.Warning);
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            Window.Init(_WindowSize.X, _WindowSize.Y, "OriNoco - None");
            {
                Window.SetMinSize(640, 360);
                Window.SetIcon(Image.LoadFromMemory(".png", BuiltResources.OriNoco));
                Time.SetTargetFPS(100);

                TextureDictionary.Init();
                FontsDictionary.Init();

                rlImGui.SetupUserFonts += val => FontsDictionary.InitImGui();

                AudioDevice.Init();
                {
                    AudioDevice.SetMasterVolume(1f);
                    rlImGui.Setup(true);
                    {
                        Core.Init();
                        foreach (var scene in Scenes)
                            scene.Init();

                        while (!Window.ShouldClose() && !StopRunning)
                        {
                            if (_PreviousSize != WindowSize)
                            {
                                _PreviousSize = WindowSize;
                                _WindowSize = new Point(Window.GetScreenWidth(), Window.GetScreenHeight());

                                foreach (var scene in Scenes)
                                    scene.OnWindowResized();
                            }

                            GUI.StartUpdate();
                            ETween.Update();
                            DelayUtil.Update();

                            foreach (var scene in Scenes) scene.Update();

                            Graphics.BeginDrawing();
                            foreach (var scene in Scenes) scene.Draw();

                            GUI.Begin();
                            MenuBar.Draw();
                            foreach (var scene in Scenes) scene.DrawGUI();
                            ProjectInfoWindow.Draw();
                            NewProjectWindow.Draw();
                            ProjectsWindow.Draw();
                            GUI.End();

                            foreach (var scene in Scenes) scene.PostRender();
                            Graphics.EndDrawing();
                            GUI.EndUpdate();
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