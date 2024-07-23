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
using SharpFileDialog;
using Point = System.Drawing.Point;
using OriNoco.Data;

namespace OriNoco.Rhine
{
    public class RhineScene : Scene
    {
        #region Variables
        public bool showProperties = true;
        private Color backgroundColor = Color.Black;

        private readonly RhinePlayer player;
        private readonly Viewport2D viewport;
        private readonly TextureDrawable noteDrawable;

        public PredictableLane lane = new();

        public Music music;
        public float fontSize = 20;
        public float followSpeed = 1.5f;

        public Point viewportOffset = new(0, 20);
        public Point viewportScaleOffset = new(300, 220);
        public int divisionMode = 1;
        public float[] divisions = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 16, 24, 32 ];

        public Font mainFont;

        public List<RhineNote> notes = [];
        #endregion
        #region Initialization
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
        #endregion
        #region Overrides
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
            if (showProperties)
            {
                viewportOffset = new(0, 20);
                viewportScaleOffset = new(300, 220);
            }
            else
            {
                viewportOffset = new(0, 20);
                viewportScaleOffset = new(300, 20);
            }

            if (player.IsStarted)
            {
                Core.Time += Time.GetFrameTime();
                music.UpdateStream();
            }
            player.Update();

            viewport.Position = Vector2.Lerp(viewport.Position, player.drawable.Position, followSpeed * Time.GetFrameTime());
            viewport.Update();
        }

        public override void Draw()
        {
            Graphics.BeginScissorMode(viewportOffset.X, viewportOffset.Y, Window.GetScreenWidth() - viewportScaleOffset.X, Window.GetScreenHeight() - viewportScaleOffset.Y);
            Graphics.ClearBackground(backgroundColor);
            
            Graphics.DrawTextPro(mainFont, "OriNoco", new Vector2(10, 30), new Vector2(0, 0), 0, fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"FPS: {Time.GetFPS()}", new Vector2(10, 50), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Notes: {notes.Count}", new Vector2(10, 70), fontSize, 5, Color.White);
            Graphics.DrawTextEx(mainFont, $"Time: {Core.Time}", new Vector2(10, 110), fontSize, 5, Color.White);

            viewport.Begin();

            foreach (var note in notes)
                note.Draw();

            player.Draw();
            viewport.End();
            Graphics.EndScissorMode();
        }

        public override Vector2 GetViewportSize()
        {
            var size = base.GetViewportSize();
            size.X -= viewportScaleOffset.X;
            size.Y -= viewportScaleOffset.Y;
            return size;
        }

        public override Vector2 GetViewportOffset() => new Vector2(viewportOffset.X, viewportOffset.Y);
        #endregion
        #region Helpers and Methods
        public static void DrawPoint(Vector2 point, float thickness, Color color)
        {
            Graphics.DrawLineEx(new(0, point.Y), new(Window.GetScreenWidth(), point.Y), thickness, color);
            Graphics.DrawLineEx(new(point.X, 0), new(point.X, Window.GetScreenHeight()), thickness, color);
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
                    if (Core.Time < notes[i].time)
                    {
                        var value = lane.GetValueFromTime(Core.Time);
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

                var lastValue = lane.GetValueFromTime(Core.Time);
                position += direction.ToDirection() * (lastValue - previousValue);
            }
            else
            {
                position = Direction.Up.ToDirection() * lane.GetValueFromTime(Core.Time);
            }

            skip:
            player.drawable.Position = position;

            int index = lane.GetChangeIndexFromTime(Core.Time);
            player.speed = index >= 0 ? lane.changes[index].rate : lane.initialRate;
        }
        #endregion
        #region Notes
        public RhineNote CreateNote(NoteType type, Direction direction, float time, Vector2 position)
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
            return note;
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
            notes.Find(val => MathF.Abs(val.time - time) < Program.TolerableEpsilon);

        public void UpdateNote(float time)
        {
            var note = GetNoteAtTime(time);
            var direction = Program.Charter.GetDirectionAtTime(time);

            if (note != null)
            {
                int index = notes.IndexOf(note);
                if (direction == Direction.None)
                    DeleteNote(note);
                else
                    note.UpdateDirection(direction);

                UpdateNotesFromIndex(index - 1);
                UpdatePlayerPosition();
            }
            else if (direction != Direction.None)
            {
                note = CreateNote(NoteType.Tap, direction, time, player.drawable.Position);
                UpdateNotesFromIndex(notes.IndexOf(note));
            }
        }

        public void UpdateNotesFromIndex(int index)
        {
            if (index < 0) index = 0;

            if (notes.Count > 0)
            {
                var note = notes[index];

                if (index == 0)
                    note.note.Position = Direction.Up.ToDirection() * lane.GetValueFromTime(note.time);

                Vector2 position = note.note.Position;
                float previousValue = lane.GetValueFromTime(note.time);
                Direction direction = note.direction;

                for (int i = index + 1; i < notes.Count; i++)
                {
                    note = notes[i];
                    var value = lane.GetValueFromTime(note.time);
                    position += direction.ToDirection() * (value - previousValue);
                    previousValue = value;
                    direction = note.direction;
                    note.AdjustDrawables(position, 0.2f);
                }
            }
        }

        #endregion
        #region GUI
        public override void DrawGUI()
        {
            DrawMenuBar();
            DrawProperties();
        }

        public void DrawMenuBar()
        {
            ImGui.BeginMainMenuBar();
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New"))
                    {
                        Program.Charter.lane = new RhythmLane();
                        Program.Charter.notes.Clear();

                        Core.Time = 0f;
                        notes.Clear();
                        UpdatePlayerPosition();
                        lane = new PredictableLane();

                        Program.Charter.PostScrollUpdate();

                        Window.SetTitle("OriNoco - None");
                    }

                    if (File.Exists("notes.json"))
                    {
                        if (ImGui.MenuItem("Load"))
                        {
                            var serializables = MainSerializer.Deserialize<List<NoteData>>(File.ReadAllText("notes.json"));
                            if (serializables != null)
                            {
                                notes.Clear();
                                foreach (var serializable in serializables)
                                {
                                    CreateNote(serializable.Type, serializable.Direction, serializable.Time, serializable.Position);
                                    Program.Charter.EvaulateDirectionToCreateNote(serializable.Direction, serializable.Time);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Failed to load notes");
                            }
                        }
                    }

                    if (ImGui.MenuItem("Save"))
                    {
                        var serializables = new List<NoteData>();
                        foreach (var note in notes)
                            serializables.Add(new NoteData(note));
                        File.WriteAllText("notes.json", MainSerializer.Serialize(serializables, true));
                    }

                    if (ImGui.MenuItem("Exit"))
                    {
                        Window.Close();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Refresh All Notes"))
                    {
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    ImGui.MenuItem("Properties", string.Empty, ref showProperties);
                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();
        }

        public void DrawProperties()
        {
            if (!showProperties) return;

            var size = GetViewportSize();
            GUI.SetNextWindowPos(new Vector2(0, size.Y + 20));
            GUI.SetNextWindowSize(new Vector2(size.X, 200));
            GUI.BeginWindow("Properties", ref showProperties, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);
            {
                if (ImGui.BeginTable("Settings", 3, ImGuiTableFlags.Borders))
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);

                    GUI.TextColored(new(0f, 1f, 0f, 1f), "Player Speed");
                    var change = lane.GetChangeFromTime(Core.Time);
                    if (change != null)
                    {
                        GUI.Text($"Change Time: " + change.time);
                        GUI.Text($"Change Index: " + lane.changes.IndexOf(change));
                        if (ImGui.InputFloat("Speed", ref change.rate, 0.5f))
                        {
                            change.rate = Math.Max(change.rate, 0f);
                            UpdateNotesFromIndex(0);
                        }
                    }
                    else
                    {
                        GUI.Text($"Initial Speed");
                        if (ImGui.InputFloat("Speed", ref lane.initialRate, 0.5f))
                        {
                            lane.initialRate = Math.Max(lane.initialRate, 0f);
                            UpdateNotesFromIndex(0);
                        }
                    }

                    ImGui.TableSetColumnIndex(1);
                    GUI.TextColored(new(0f, 1f, 0f, 1f), "Rhythm");
                    var bpmLaneChange = Program.Charter.lane.GetChangeFromTime(Core.Time);
                    if (bpmLaneChange != null)
                    {
                        GUI.Text($"Change Time: " + bpmLaneChange.time);
                        GUI.Text($"Change Index: " + Program.Charter.lane.changes.IndexOf(bpmLaneChange));
                        var bpmChange = new BPMChange(bpmLaneChange);
                        if (ImGui.InputFloat("BPM", ref bpmChange.bpm, 0.5f))
                        {
                            bpmChange.bpm = Math.Max(bpmChange.bpm, 10f);
                            bpmLaneChange.rate = bpmChange.GetRate();
                            UpdateNotesFromIndex(0);
                        }
                    }
                    else
                    {
                        GUI.Text($"Initial BPM");
                        float initRate = 60f / Program.Charter.lane.initialRate;
                        if (ImGui.InputFloat("BPM", ref initRate, 0.5f))
                        {
                            initRate = Math.Max(initRate, 10f);
                            Program.Charter.lane.initialRate = 60f / initRate;
                            UpdateNotesFromIndex(0);
                        }
                    }

                    ImGui.TableSetColumnIndex(2);
                    GUI.TextColored(new(0f, 1f, 0f, 1f), "Camera");
                    GUI.Text($"Position: {viewport.Position}");
                    viewport.OrthographicSize = GUI.Slider("Size", viewport.OrthographicSize, 1f, 25f);
                    followSpeed = GUI.Slider("Follow Speed", followSpeed, 0f, 10f);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    GUI.TextColored(new(0f, 1f, 0f, 1f), "Charter");

                    if (ImGui.SliderInt("Division Mode", ref divisionMode, 0, divisions.Length - 1))
                        Program.Charter.division = divisions[divisionMode];
                    GUI.Text("Current division: " + divisions[divisionMode]);

                    ImGui.SliderInt("Grid Lines Count", ref Program.Charter.gridLineCount, 16, 512);

                    ImGui.EndTable();
                }
            }
            GUI.EndWindow();
        }
        #endregion
    }
}
