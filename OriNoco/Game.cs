using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Logging;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

using Point = System.Drawing.Point;
using rlImGui_cs;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Camera.Cam2D;
using ImGuiNET;
using System.Numerics;
using Raylib_CSharp.Audio;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using Raylib_CSharp.Textures;

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

        private static float _ViewportWidth;
        private static float _ViewportHeight;

        public static float speed = 5f;
        public static float followSpeed = 1.5f;
        public static Vector2 direction = new Vector2(0f, 1f);
        private static Vector2 previousDirection = new Vector2();
        private static Vector2 previousCameraPos;
        private static float rotation;
        public static Camera2D viewport2D;
        public static Music music;
        public static Sound sound;
        private static bool enableDiagonal;
        private static Vector2 previousSizeCV;
        private static bool chartView;

        private static float _OrthographicSize = 5f;
        public static float OrthographicSize
        {
            get => _OrthographicSize;
            set
            {
                _OrthographicSize = value;
                viewport2D.Zoom = _ViewportHeight / _OrthographicSize * 0.5f;
            }
        }

        public static TextureDrawable note;
        public static List<ShapeDrawable> lines = new List<ShapeDrawable>();

        static bool ImGuiDemoOpen = false;
        static bool isStarted = false;
        static bool IsHovering;
        static RenderTexture2D ChartRenderTexture;

        public static void Init()
        {
            Window.Init(_WindowSize.X, _WindowSize.Y, "Orinoco");
            Logger.SetTraceLogLevel(TraceLogLevel.Warning);
            TextureDictionary.Load();

            viewport2D = new Camera2D
            {
                Target = new(0.0f, 0.0f),
                Rotation = 0.0f,
                Zoom = 1.0f
            };

            note = new TextureDrawable(TextureDictionary.note);
            note.position = new Vector2(0f, 0f);
            note.scale = new Vector2(0.2f, 0.2f);

            AudioDevice.Init();
            AudioDevice.SetMasterVolume(1f);

            music = Music.Load("Sounds/NULL APOPHENIA.ogg");
            sound = Sound.Load("Sounds/NULL APOPHENIA.ogg");

            ChartRenderTexture = RenderTexture2D.Load(Window.GetScreenWidth(), Window.GetScreenHeight());

            rlImGui.Setup(true);

            Time.SetTargetFPS(100);

            while(!Window.ShouldClose())
            {
                UpdateViewports();
                Update();
                Draw();
            }

            rlImGui.Shutdown();

            ChartRenderTexture.Unload();
            music.UnloadStream();
            AudioDevice.Close();
        }

        private static void UpdateViewports()
        {
            if (_ViewportWidth == Window.GetScreenWidth() && _ViewportHeight == Window.GetScreenHeight())
                return;

            _ViewportWidth = Window.GetScreenWidth();
            _ViewportHeight = Window.GetScreenHeight();

            viewport2D.Offset = new Vector2(_ViewportWidth / 2, _ViewportHeight / 2);
            viewport2D.Zoom = _ViewportHeight / _OrthographicSize * 0.5f;
        }

        private static void Update()
        {
            if (Input.IsKeyDown(KeyboardKey.Escape))
            {
                Window.Close();
                return;
            }

            if (isStarted)
            {
                music.UpdateStream();

                // Player
                note.position += direction * speed * Time.GetFrameTime();

                // Lines
                var tail = lines[lines.Count - 1];
                tail.position += direction * speed * Time.GetFrameTime() / 2;
                tail.scale += new Vector2(0f, speed * Time.GetFrameTime());

                // Camera
                var target = Vector2.Lerp(previousCameraPos, note.position, followSpeed * Time.GetFrameTime());
                previousCameraPos = target;
                target.Y = -target.Y;
                viewport2D.Target = target;

                if (Input.IsKeyPressed(KeyboardKey.Q))
                {
                    enableDiagonal = !enableDiagonal;
                }

                if (Input.IsKeyDown(KeyboardKey.Down))
                {
                    if (Input.IsKeyDown(KeyboardKey.Right) && enableDiagonal)
                    {
                        Console.WriteLine("Down Right");
                        direction = Vector2.Normalize(new Vector2(1f, -1f));
                        rotation = 135f;
                    }
                    else if (Input.IsKeyDown(KeyboardKey.Left) && enableDiagonal)
                    {
                        Console.WriteLine("Down Left");
                        direction = Vector2.Normalize(new Vector2(-1f, -1f));
                        rotation = 225f;
                    }
                    else
                    {
                        Console.WriteLine("Down");
                        direction = new Vector2(0, -1f);
                        rotation = 180f;
                    }
                }
                else if (Input.IsKeyDown(KeyboardKey.Up))
                {
                    if (Input.IsKeyDown(KeyboardKey.Right) && enableDiagonal)
                    {
                        Console.WriteLine("Up Right");
                        direction = Vector2.Normalize(new Vector2(1f, 1f));
                        rotation = 45f;
                    }
                    else if (Input.IsKeyDown(KeyboardKey.Left) && enableDiagonal)
                    {
                        Console.WriteLine("Up Left");
                        direction = Vector2.Normalize(new Vector2(-1f, 1f));
                        rotation = -45f;
                    }
                    else
                    {
                        Console.WriteLine("Up");
                        direction = new Vector2(0, 1f);
                        rotation = 0f;
                    }
                }
                else if (Input.IsKeyDown(KeyboardKey.Left))
                {
                    Console.WriteLine("Left");
                    direction = Vector2.Normalize(new Vector2(-1f, 0f));
                    rotation = -90f;
                }
                else if (Input.IsKeyDown(KeyboardKey.Right))
                {
                    Console.WriteLine("Right");
                    direction = Vector2.Normalize(new Vector2(1f, 0f));
                    rotation = 90f;
                }

                if (!IsHovering)
                {
                    if (Input.IsMouseButtonDown(MouseButton.Left) || Input.IsMouseButtonDown(MouseButton.Right))
                    {
                        var mousePosition = viewport2D.GetScreenToWorld(Input.GetMousePosition());
                        mousePosition.Y = -mousePosition.Y;

                        float distance = Vector2.Distance(note.position, mousePosition);
                        Vector2 right = note.position + new Vector2(distance, 0);
                        Vector2 left = note.position + new Vector2(-distance, 0);
                        Vector2 up = note.position + new Vector2(0, distance);
                        Vector2 down = note.position + new Vector2(0, -distance);

                        Vector2 upRight = note.position + (Vector2.Normalize(new Vector2(1f, 1f)) * distance);
                        Vector2 upLeft = note.position + (Vector2.Normalize(new Vector2(-1f, 1f)) * distance);
                        Vector2 downRight = note.position + (Vector2.Normalize(new Vector2(1f, -1f)) * distance);
                        Vector2 downLeft = note.position + (Vector2.Normalize(new Vector2(-1f, -1f)) * distance);

                        float currentDistance = 0f;
                        if (currentDistance < Vector2.Distance(mousePosition, right))
                        {
                            currentDistance = Vector2.Distance(mousePosition, right);
                            direction = Vector2.Normalize(new Vector2(-1f, 0f));
                            rotation = 90f;
                        }
                        if (currentDistance < Vector2.Distance(mousePosition, left))
                        {
                            currentDistance = Vector2.Distance(mousePosition, left);
                            direction = Vector2.Normalize(new Vector2(1f, 0f));
                            rotation = -90f;
                        }
                        if (currentDistance < Vector2.Distance(mousePosition, up))
                        {
                            currentDistance = Vector2.Distance(mousePosition, up);
                            direction = Vector2.Normalize(new Vector2(0f, -1f));
                            rotation = 0f;
                        }
                        if (currentDistance < Vector2.Distance(mousePosition, down))
                        {
                            currentDistance = Vector2.Distance(mousePosition, down);
                            direction = Vector2.Normalize(new Vector2(0f, 1f));
                            rotation = 180f;
                        }

                        if (enableDiagonal)
                        {
                            if (currentDistance < Vector2.Distance(mousePosition, upRight))
                            {
                                currentDistance = Vector2.Distance(mousePosition, upRight);
                                direction = Vector2.Normalize(new Vector2(-1f, -1f));
                                rotation = -135f;
                            }
                            if (currentDistance < Vector2.Distance(mousePosition, upLeft))
                            {
                                currentDistance = Vector2.Distance(mousePosition, upLeft);
                                direction = Vector2.Normalize(new Vector2(1f, -1f));
                                rotation = 135f;
                            }
                            if (currentDistance < Vector2.Distance(mousePosition, downRight))
                            {
                                currentDistance = Vector2.Distance(mousePosition, downRight);
                                direction = Vector2.Normalize(new Vector2(-1f, 1f));
                                rotation = -45f;
                            }
                            if (currentDistance < Vector2.Distance(mousePosition, downLeft))
                            {
                                currentDistance = Vector2.Distance(mousePosition, downLeft);
                                direction = Vector2.Normalize(new Vector2(1f, 1f));
                                rotation = 45f;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!IsHovering)
                {
                    if (Input.IsKeyPressed(KeyboardKey.Up) || Input.IsMouseButtonPressed(MouseButton.Left))
                    {
                        Console.WriteLine("Music played!");
                        music.PlayStream();
                        isStarted = true;
                        CreateTail();
                    }
                }
            }

            if (previousDirection != direction)
            {
                CreateTail();
                previousDirection = direction;
            }
        }

        private static void Draw()
        {
            Graphics.BeginDrawing();
            Graphics.ClearBackground(Color.DarkGray);

            Graphics.DrawText("OriNoco", 10, 25, 20, Color.White);
            Graphics.DrawText($"FPS: {Time.GetFPS()}", 10, 50, 20, Color.White);

            viewport2D.Offset = new Vector2(_ViewportWidth / 2, _ViewportHeight / 2);
            viewport2D.Zoom = _ViewportHeight / _OrthographicSize * 0.5f;
            Graphics.BeginMode2D(viewport2D);
            foreach (var shape in lines)
                shape.Draw();

            note.Draw();
            Graphics.EndMode2D();

            Graphics.BeginTextureMode(ChartRenderTexture);
            Graphics.ClearBackground(Color.DarkGray);

            viewport2D.Offset = new Vector2(previousSizeCV.X / 2, previousSizeCV.Y / 2);
            viewport2D.Zoom = previousSizeCV.Y / _OrthographicSize * 0.5f;
            Graphics.BeginMode2D(viewport2D);
            foreach (var shape in lines)
                shape.Draw();

            note.Draw();
            Graphics.EndMode2D();
            Graphics.EndTextureMode();

            rlImGui.Begin();

            IsHovering = ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) || ImGui.IsMouseDown(ImGuiMouseButton.Left);
            DrawMainStrip();

            ImGui.Begin("Test");
            {
                ImGui.LabelText(IsHovering ? "Hovering" : "Not Hovering", "s");
                ImGui.SliderFloat("Orthographic Size", ref _OrthographicSize, 1f, 100f);
                ImGui.SliderFloat("Speed", ref speed, 2f, 20f);
                ImGui.SliderFloat("Follow Speed", ref followSpeed, 0.01f, 10f);
                ImGui.Checkbox("Enable Diagonal", ref enableDiagonal);
                viewport2D.Zoom = _ViewportHeight / _OrthographicSize * 0.5f;

                if (ImGuiDemoOpen)
                    ImGui.ShowDemoWindow(ref ImGuiDemoOpen);
            }
            ImGui.End();

            if (chartView)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
                ImGui.SetNextWindowSizeConstraints(new Vector2(400, 400), new Vector2(Window.GetScreenWidth(), Window.GetScreenHeight()));

                ImGui.Begin("Chart View", ref chartView, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
                {
                    rlImGui.ImageRenderTexture(ChartRenderTexture);

                    if (previousSizeCV != ImGui.GetWindowSize())
                    {
                        previousSizeCV = ImGui.GetWindowSize();
                        ChartRenderTexture.Unload();
                        ChartRenderTexture = RenderTexture2D.Load((int)ImGui.GetWindowWidth(), (int)ImGui.GetWindowHeight());
                        Console.WriteLine("Resizing render texture to: " + (int)ImGui.GetWindowWidth() + " x " + ((int)ImGui.GetWindowHeight()));
                    }
                }
                ImGui.End();
                ImGui.PopStyleVar();
            }

            rlImGui.End();

            Graphics.EndDrawing();
        }

        private static void DrawMainStrip()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Exit"))
                        Window.Close();

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    ImGui.MenuItem("ImGui Demo", string.Empty, ref ImGuiDemoOpen);
                    ImGui.MenuItem("Chart View", string.Empty, ref chartView);

                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private static void CreateTail()
        {
            var center = new ShapeDrawable
            {
                shape = ShapeDrawable.ShapeType.Circle,
                scale = new Vector2(0.1f, 0.1f),
                color = Color.White,
                position = note.position
            };
            lines.Add(center);

            var tail = new ShapeDrawable
            {
                position = note.position,
                scale = new Vector2(0.1f, 0),
                color = Color.White,
                rotation = rotation
            };
            lines.Add(tail);
        }
    }
}