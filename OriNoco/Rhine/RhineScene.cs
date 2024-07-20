using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json.Serialization;
using OriNoco.Serializer;
using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace OriNoco.Rhine
{
    public class RhineScene : Scene
    {
        public bool showWindow = true;
        private Color backgroundColor = Color.Black;
        private RhinePlayer player;
        private Viewport2D viewport;
        private TextureDrawable noteDrawable;

        public PredictableLane lane = new PredictableLane();

        public Music music;
        public float fontSize = 20;
        public float followSpeed = 1.5f;
        public float time = 0f;
        public float bpm = 120f;

        public Font mainFont;

        public List<RhineNote> notes = new List<RhineNote>();

        public RhineScene()
        {
            viewport = new Viewport2D(this);
            player = new RhinePlayer(false)
            {
                freeplay = true,
                createNotes = true,
                showTail = false,
            };
            noteDrawable = new TextureDrawable(default);
        }

        public override void Init()
        {
            player.LoadTexture();
            noteDrawable.Texture = TextureDictionary.note;
            noteDrawable.Scale = new Vector2(0.2f);
            music = Music.Load("Sounds/NULL APOPHENIA.ogg");
            mainFont = FontsDictionary.GeoSansLight;
        }

        public override void Update()
        {
            if (player.IsStarted)
            {
                time += Time.GetFrameTime();
                music.UpdateStream();
            }
            player.Update();

            viewport.Position = Vector2.Lerp(viewport.Position, player.drawable.Position, followSpeed * Time.GetFrameTime());
            viewport.Update();
        }

        public override void Draw()
        {
            Graphics.BeginScissorMode(0, 0, Window.GetScreenWidth() - 300, Window.GetScreenHeight());
            Graphics.ClearBackground(backgroundColor);
            
            Graphics.DrawTextPro(mainFont, "OriNoco", new Vector2(10, 10), new Vector2(0, 0), 0, fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"FPS: {Time.GetFPS()}", new Vector2(10, 30), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Notes: {notes.Count}", new Vector2(10, 50), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Music: N² - NULL APOPHENIA", new Vector2(10, 70), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Time: {time}", new Vector2(10, 90), fontSize, 5, Color.White);

            float distance = 1f;

            viewport.Begin();

            for (int i = 0; i < 12; i++)
            {
                noteDrawable.Draw(player.drawable.Position + (distance * player.direction.ToDirection() * i), new ColorF(1f, 1f, 1f, (150 - (i * 8)) / 255f));
            }

            foreach (var note in notes)
                note.Draw();

            player.Draw();
            viewport.End();
            Graphics.EndScissorMode();
        }

        public void DrawPoint(Vector2 point, float thickness, Color color)
        {
            Graphics.DrawLineEx(new Vector2(0, point.Y), new Vector2(Window.GetScreenWidth(), point.Y), thickness, color);
            Graphics.DrawLineEx(new Vector2(point.X, 0), new Vector2(point.X, Window.GetScreenHeight()), thickness, color);
        }

        public override void DrawGUI()
        {
            if (showWindow)
            {
                GUI.BeginWindow("Rhine Settings", ref showWindow);
                {
                    GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Player");
                    GUI.Text($"Position: {player.drawable.Position}");
                    GUI.Text($"Direction: {player.direction}");
                    player.mode = GUI.ComboBox("Create Mode", player.mode);
                    player.speed = GUI.Slider("Speed", player.speed, 1f, 20f);

                    GUI.Separator();

                    GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Camera");
                    GUI.Text($"Position: {viewport.Position}");
                    viewport.OrthographicSize = GUI.Slider("Size", viewport.OrthographicSize, 1f, 25f);
                    followSpeed = GUI.Slider("Follow Speed", followSpeed, 0f, 10f);

                    GUI.Separator();

                    GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "UI");
                    fontSize = GUI.Slider("Font Size", fontSize, 10f, 50f);
                    GUI.Text($"Hovering GUI: " + GUI.IsOverAnyElement);

                    GUI.Separator();

                    GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Chart");
                    if (GUI.Button("Save"))
                    {
                        var serializables = new List<NoteSerializable>();
                        foreach (var note in notes)
                            serializables.Add(new NoteSerializable(note));
                        File.WriteAllText("notes.json", MainSerializer.Serialize(serializables, true));
                    }
                }
                GUI.EndWindow();
            }
        }

        public override void Shutdown()
        {
            notes.Clear();
        }

        public override Vector2 GetViewportSize()
        {
            var size = base.GetViewportSize();
            size.X = size.X - 300;
            return size;
        }

        [System.Serializable]
        public class NoteSerializable
        {
            [JsonPropertyName("position")]
            public Vector2 Position { get; set; }
            [JsonPropertyName("time")]
            public float Time { get; set; }
            [JsonPropertyName("direction")]
            public Direction Direction { get; set; }
            [JsonPropertyName("type")]
            public NoteType Type { get; set; }

            public NoteSerializable() { }
            public NoteSerializable(RhineNote note)
            {
                Position = note.note.Position;
                Time = note.time;
                Direction = note.direction;
                Type = note.type;
            }
        }
    }
}
