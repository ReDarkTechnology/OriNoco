using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace OriNoco.Rhine
{
    public class RhineScene : Scene
    {
        private Color backgroundColor = Color.Black;
        private RhinePlayer player;
        private Viewport2D viewport;
        public Music music;

        public float followSpeed = 1.5f;
        public float time = 0f;

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
        }

        public override void Init()
        {
            player.LoadTexture();
            music = Music.Load("Sounds/NULL APOPHENIA.ogg");
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
            bool wasHovering = GUI.IsOverAnyElement;
            Graphics.ClearBackground(backgroundColor);

            Graphics.DrawText("OriNoco", 10, 10, 20, Color.White);
            Graphics.DrawText($"FPS: {Time.GetFPS()}", 10, 30, 20, Color.White);
            Graphics.DrawText($"Notes: {notes.Count}", 10, 50, 20, Color.White);
            Graphics.DrawText($"Music: N² - NULL APOPHENIA", 10, 70, 20, Color.White);

            viewport.Begin();
            foreach (var note in notes)
                note.Draw();

            player.Draw();
            viewport.End();

            GUI.Begin();

            GUI.BeginWindow("Rhine Settings");
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
                GUI.Text($"Hovering GUI: " + wasHovering);

                GUI.Separator();

                GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Chart");
                if(GUI.Button("Save"))
                {
                    var serializables = new List<NoteSerializable>();
                    foreach(var note in notes)
                        serializables.Add(new NoteSerializable(note));
                    File.WriteAllText("notes.json", JsonSerializer.Serialize(serializables));
                }
            }
            GUI.EndWindow();

            GUI.End();
        }

        public override void Shutdown()
        {
            notes.Clear();
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
