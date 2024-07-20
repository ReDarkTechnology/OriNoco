using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json.Serialization;
using ImGuiNET;
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
            
            Graphics.DrawTextPro(mainFont, "OriNoco", new Vector2(10, 30), new Vector2(0, 0), 0, fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"FPS: {Time.GetFPS()}", new Vector2(10, 50), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Notes: {notes.Count}", new Vector2(10, 70), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Music: N² - NULL APOPHENIA", new Vector2(10, 90), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Time: {time}", new Vector2(10, 110), fontSize, 5, Color.White);

            viewport.Begin();

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

        public void UpdatePlayerPosition()
        {
            Vector2 position = Vector2.Zero;
            var direction = Direction.Up;
            float previousValue = 0f;
            if (notes.Count > 0)
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    if (time < notes[i].time)
                    {
                        var value = lane.GetValueFromTime(time);
                        Console.WriteLine($"{time}: {value}");
                        position += direction.ToDirection() * (value - previousValue);
                        goto skip;
                    }
                    else
                    {
                        var value = lane.GetValueFromTime(notes[i].time);
                        position += direction.ToDirection() * (value - previousValue);
                        previousValue = value;
                        direction = notes[i].direction;
                    }
                }

                var lastValue = lane.GetValueFromTime(time);
                position += direction.ToDirection() * (lastValue - previousValue);
            }
            else
            {
                position = Direction.Up.ToDirection() * lane.GetValueFromTime(time);
            }

            skip:
            player.drawable.Position = position;
            player.drawable.Rotation = direction.ToRotation();

            int index = lane.GetChangeIndexFromTime(time);
            player.speed = index >= 0 ? lane.changes[index].rate : lane.initialRate;
        }

        public void UpdateNote(float time)
        {
            var note = GetNoteAtTime(time);
            var direction = Program.CharterScene.GetDirectionAtTime(time);

            if (note != null)
            {
                if (direction == Direction.None)
                    DeleteNote(note);
                else
                    note.UpdateDirection(direction);
            }
            else if (direction != Direction.None)
            {
                CreateNote(NoteType.Tap, direction, time, player.drawable.Position);
            }

            UpdatePlayerPosition();
        }

        public void CreateNote(NoteType type, Direction direction, float time, Vector2 position)
        {
            var note = new RhineNote
            {
                type = type,
                direction = direction,
                time = time
            };
            note.AdjustDrawables(position, 0.2f);
            notes.Add(note);
            notes.Sort((a, b) => a.time.CompareTo(b.time));

            UpdatePlayerPosition();
        }

        public void DeleteNote(RhineNote note)
        {
            notes.Remove(note);
            notes.Sort((a, b) => a.time.CompareTo(b.time));
        }

        public void DeleteNote(float time)
        {
            var note = GetNoteAtTime(time);
            if (note != null)
            {
                notes.Remove(note);
                notes.Sort((a, b) => a.time.CompareTo(b.time));
            }
        }

        public RhineNote? GetNoteAtTime(float time) =>
            notes.Find(val => MathF.Abs(val.time - time) < float.Epsilon);

        public override void DrawGUI()
        {
            if (showWindow)
            {
                var size = GetViewportSize();
                GUI.SetNextWindowPos(new Vector2(0, size.Y - 200));
                GUI.SetNextWindowSize(new Vector2(size.X, 200));
                GUI.BeginWindow("Properties", ref showWindow, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);
                {
                    if (ImGui.BeginTable("Settings", 2, ImGuiTableFlags.Borders))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);

                        GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Player");
                        GUI.Text($"Position: {player.drawable.Position}");
                        GUI.Text($"Direction: {player.direction}");
                        player.mode = GUI.ComboBox("Create Mode", player.mode);
                        player.speed = GUI.Slider("Speed", player.speed, 1f, 20f);


                        ImGui.TableSetColumnIndex(1);
                        GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Camera");
                        GUI.Text($"Position: {viewport.Position}");
                        viewport.OrthographicSize = GUI.Slider("Size", viewport.OrthographicSize, 1f, 25f);
                        followSpeed = GUI.Slider("Follow Speed", followSpeed, 0f, 10f);


                        ImGui.TableNextRow();

                        ImGui.TableSetColumnIndex(0);
                        GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "UI");
                        fontSize = GUI.Slider("Font Size", fontSize, 10f, 50f);
                        GUI.Text($"Hovering GUI: " + GUI.IsOverAnyElement);


                        ImGui.TableSetColumnIndex(1);
                        GUI.TextColored(new Vector4(0f, 1f, 0f, 1f), "Chart");

                        if (GUI.Button("Save"))
                        {
                            var serializables = new List<NoteSerializable>();
                            foreach (var note in notes)
                                serializables.Add(new NoteSerializable(note));
                            File.WriteAllText("notes.json", MainSerializer.Serialize(serializables, true));
                        }


                        if (File.Exists("notes.json"))
                        {
                            GUI.SameLine();
                            if (GUI.Button("Load"))
                            {
                                var serializables = MainSerializer.Deserialize<List<NoteSerializable>>(File.ReadAllText("notes.json"));
                                if (serializables != null)
                                {
                                    notes.Clear();
                                    foreach (var serializable in serializables)
                                    {
                                        CreateNote(serializable.Type, serializable.Direction, serializable.Time, serializable.Position);
                                        switch (serializable.Direction)
                                        {
                                            case Direction.Left:
                                                Program.CharterScene.CreateNote(Direction.Left, serializable.Time, false);
                                                break;
                                            case Direction.Down:
                                                Program.CharterScene.CreateNote(Direction.Down, serializable.Time, false);
                                                break;
                                            case Direction.Up:
                                                Program.CharterScene.CreateNote(Direction.Up, serializable.Time, false);
                                                break;
                                            case Direction.Right:
                                                Program.CharterScene.CreateNote(Direction.Right, serializable.Time, false);
                                                break;
                                            case Direction.LeftUp:
                                                Program.CharterScene.CreateNote(Direction.Left, serializable.Time, false);
                                                Program.CharterScene.CreateNote(Direction.Up, serializable.Time, false);
                                                break;
                                            case Direction.LeftDown:
                                                Program.CharterScene.CreateNote(Direction.Left, serializable.Time, false);
                                                Program.CharterScene.CreateNote(Direction.Down, serializable.Time, false);
                                                break;
                                            case Direction.RightUp:
                                                Program.CharterScene.CreateNote(Direction.Right, serializable.Time, false);
                                                Program.CharterScene.CreateNote(Direction.Up, serializable.Time, false);
                                                break;
                                            case Direction.RightDown:
                                                Program.CharterScene.CreateNote(Direction.Right, serializable.Time, false);
                                                Program.CharterScene.CreateNote(Direction.Down, serializable.Time, false);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Failed to load notes");
                                }
                            }
                        }

                        ImGui.EndTable();
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
