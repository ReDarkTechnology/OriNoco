using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;

using OriNoco.UI;
using OriNoco.Data;
using OriNoco.Serializer;

using ImGuiNET;
using Point = System.Drawing.Point;

namespace OriNoco.Rhine
{
    public class RhineScene : Scene
    {
        #region Variables
        // ImGUI Variables
        public bool showProperties = true;
        public bool adjustToGrid = true;

        bool initSize;

        public bool unlockBreakingChanges = false;

        private Color backgroundColor = Color.Black;

        private readonly RhinePlayer player;
        private readonly Viewport2D viewport;
        private readonly TextureDrawable noteDrawable;

        public PredictableLane lane = new();

        public Music defaultMusic;
        public Music music;
        public Sound hitSound;

        public float fontSize = 20;
        public float followSpeed = 1.5f;

        private float previousTime;
        private bool enterKeyPlaymode;

        public Point viewportOffset = new(0, 20);
        public Point viewportScaleOffset = new(300, 320);
        public Rectangle viewportRect = new(0, 0, 0, 0);

        public int divisionMode = 1;
        public float[] divisions = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 16, 24, 32 ];

        public Font mainFont;

        public List<RhineNote> notes = [];
        public string?[] debugInfo = new string?[3];

        public RhineNote? selectedNote;
        public ChartInfoData newChartInfo = new ChartInfoData();
        public DelayQueue? audioDelayQueue;
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

            defaultMusic = Music.Load("Sounds/RhineTheme.mp3");
            hitSound = Sound.Load("Sounds/Hit.ogg");
            music = defaultMusic;

            mainFont = FontsDictionary.GeoSansLight;
            showProperties = Settings.Data.ShowProperties;

            OnWindowResized();
            UpdateViewport();
        }

        public override void Update()
        {
            UpdateHotKeys();

            if (Core.IsPlaying)
            {
                if (Core.Time <= (music.GeTimeLength() + Core.Info.AudioOffset))
                {
                    Core.Time += Time.GetFrameTime();
                    music.UpdateStream();

                    Program.Charter.PostScrollUpdate();
                }
            }

            viewport.Position = Vector2.Lerp(viewport.Position, player.drawable.Position, followSpeed * Time.GetFrameTime());
            viewport.Update();

            if (Input.IsMouseButtonReleased(MouseButton.Left))
            {
                foreach(var note in notes)
                {
                    if (note.IsMouseOverNote() && selectedNote != note)
                    {
                        selectedNote = note;
                        break;
                    }
                }
            }

            foreach (var note in notes)
                note.UpdateNoteColor();
        }

        public void UpdateHotKeys()
        {
            if (GUI.IsEditing()) return;

            if (Input.IsKeyPressed(KeyboardKey.Space) && !enterKeyPlaymode)
                Play();

            if (Input.IsKeyReleased(KeyboardKey.Space) && !enterKeyPlaymode)
                Stop();

            if (Input.IsKeyPressed(KeyboardKey.Enter))
            {
                if (Core.IsPlaying)
                {
                    if (enterKeyPlaymode)
                    {
                        Stop();
                        enterKeyPlaymode = false;
                    }
                }
                else
                {
                    Play();
                    enterKeyPlaymode = true;
                }
            }
        }

        public void Play()
        {
            previousTime = Core.Time;
            Core.IsPlaying = true;

            if (Core.Time >= Core.Info.AudioOffset)
            {
                music.SeekStream(Core.Time - Core.Info.AudioOffset);
                music.PlayStream();
            }
            else
            {
                audioDelayQueue = new DelayQueue(Core.Info.AudioOffset - Core.Time, music.PlayStream);
                DelayUtil.Queues.Add(audioDelayQueue);
            }
        }

        public void Stop()
        {
            Core.IsPlaying = false;
            Core.Time = previousTime;
            Program.Charter.PostScrollUpdate();
            music.StopStream();

            if (audioDelayQueue != null)
            {
                audioDelayQueue.Cancel();
                audioDelayQueue = null;
            }
        }

        public override void Draw()
        {
            if (Core.IsProjectOpen)
                DrawNormal();
            else
                DrawClear();
        }

        public void DrawNormal()
        {
            var mousePos = Input.GetMousePosition();
            Graphics.BeginScissorMode(viewportOffset.X, viewportOffset.Y, Window.GetScreenWidth() - viewportScaleOffset.X, Window.GetScreenHeight() - viewportScaleOffset.Y);
            Graphics.ClearBackground(Core.Info.BackgroundColor);

            Graphics.DrawTextPro(mainFont, "OriNoco", new Vector2(10, 30), new Vector2(0, 0), 0, fontSize, 5, Color.White);
            debugInfo[0] = Core.ShowFPS ? $"FPS: {Time.GetFPS()}" : null;
            debugInfo[1] = Core.ShowNoteCount ? $"Notes: {notes.Count}" : null; ;
            debugInfo[2] = Core.ShowTime ? $"Time: {Core.Time}" : null;

            int offset = 50;
            for (int i = 0; i < debugInfo.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(debugInfo[i]))
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    Graphics.DrawTextEx(mainFont, debugInfo[i], new Vector2(10, offset), fontSize, 5, Color.White);
#pragma warning restore CS8604 // Possible null reference argument.
                    offset += 20;
                }
            }

            viewport.Begin();

            foreach (var note in notes)
                note.Draw();

            player.Draw();

            var transformed = viewport.GetScreenToWorld(mousePos);
            var size = new Vector2(0.2f, 0.2f);

            viewport.End();
            Graphics.EndScissorMode();
        }

        public void DrawClear()
        {
            Graphics.ClearBackground(Core.Info.BackgroundColor);
        }

        public override Vector2 GetViewportSize()
        {
            var size = base.GetViewportSize();
            size.X -= viewportScaleOffset.X;
            size.Y -= viewportScaleOffset.Y;
            return size;
        }

        public override Vector2 GetViewportOffset() => new Vector2(viewportOffset.X, viewportOffset.Y);

        public override void OnWindowResized()
        {
            viewportRect.X = viewportOffset.X;
            viewportRect.Y = viewportOffset.Y;
            viewportRect.Width = Window.GetScreenWidth() - viewportScaleOffset.X;
            viewportRect.Height = Window.GetScreenHeight() - viewportScaleOffset.Y;
        }
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

            foreach (var note in notes)
                note.Update(Core.Time);
        }

        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        public bool IsMouseOverRect(Rectangle rectangle)
        {
            Vector2 mousePosition = Input.GetMousePosition();
            return RectUtil.PointInsideRect(viewportRect, mousePosition) && RectUtil.PointInsideRect(rectangle, viewport.GetScreenToWorld(mousePosition));
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
                    note.AdjustDrawables(Direction.Up.ToDirection() * lane.GetValueFromTime(note.time), 0.2f);

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
            if (Core.IsProjectOpen)
                DrawProperties();
        }

        public void DrawProperties()
        {
            if (!showProperties) return;

            var size = GetViewportSize();
            GUI.SetNextWindowPos(new Vector2(0, size.Y + 20));
            GUI.SetNextWindowSize(new Vector2(size.X, 300));
            if (ImGui.Begin("Properties", ref showProperties, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
                MenuBar.ChangeProperties();
            {
                if (ImGui.SliderFloat("Time", ref Core.Time, 0f, music.GeTimeLength() + Core.Info.AudioOffset))
                {
                    if (Core.IsPlaying)
                    {
                        if (audioDelayQueue != null)
                        {
                            audioDelayQueue.Cancel();
                            audioDelayQueue = null;
                        }

                        if (Core.Time >= Core.Info.AudioOffset)
                        {
                            music.SeekStream(Core.Time - Core.Info.AudioOffset);
                            music.PlayStream();
                        }
                        else
                        {
                            audioDelayQueue = new DelayQueue(Core.Info.AudioOffset - Core.Time, music.PlayStream);
                            DelayUtil.Queues.Add(audioDelayQueue);
                        }
                    }
                    else
                    {
                        if (adjustToGrid)
                            Core.Time = Program.Charter.lane.AdjustTimeToRate(Core.Time, Program.Charter.division);
                    }
                    Program.Charter.PostScrollUpdate();
                }

                ImGui.Checkbox("Adjust to Grid", ref adjustToGrid);

                if (ImGui.BeginTable("Settings", 3, ImGuiTableFlags.Borders))
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);

                    // --------------- SPEED ----------------
                    ImGui.PushID("Speed");
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

                        if (change.rate > Program.BigNumberLimit)
                        {
                            GUI.TextColored(new Vector4(1f, 0.7f, 0.7f, 1f), "This big of a number might be a bad idea...");
                        }
                        else if (change.rate == 0f)
                        {
                            GUI.TextColored(new Vector4(0.7f, 0.7f, 1f, 1f), "You just froze the player");
                        }

                        if (GUI.Button("Add"))
                        {
                            lane.Add(Core.Time, change.rate);
                            UpdateNotesFromIndex(0);
                        }

                        GUI.SameLine();

                        if (GUI.Button("Remove"))
                        {
                            lane.changes.Remove(change);
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

                        if (lane.initialRate > Program.BigNumberLimit)
                        {
                            GUI.TextColored(new Vector4(1f, 0.7f, 0.7f, 1f), "This big of a number might be a bad idea...");
                        }
                        else if (lane.initialRate == 0f)
                        {
                            GUI.TextColored(new Vector4(0.7f, 0.7f, 1f, 1f), "You just froze the player");
                        }

                        if (Core.Time <= 0f) ImGui.BeginDisabled();
                        if (GUI.Button("Add"))
                        {
                            lane.Add(Core.Time, lane.initialRate);
                        }
                        if (Core.Time <= 0f) ImGui.EndDisabled();
                    }
                    ImGui.PopID();

                    // --------------- BPM ----------------
                    ImGui.TableSetColumnIndex(1);
                    ImGui.PushID("BPM");
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

                        bool isAllowed = Program.Charter.lane.IsAPartOfRate(Core.Time, Program.Charter.division) || unlockBreakingChanges;

                        if (!isAllowed)
                        {
                            GUI.Text("Putting a BPM change offbeat is not recommended!");
                            if (GUI.Button("Force Anyway"))
                                unlockBreakingChanges = true;
                            ImGui.BeginDisabled();
                        }

                        if (GUI.Button("Add"))
                            Program.Charter.lane.Add(Core.Time, Program.Charter.lane.initialRate);

                        if (!isAllowed) ImGui.EndDisabled();

                        GUI.SameLine();

                        if (GUI.Button("Remove"))
                            Program.Charter.lane.changes.Remove(bpmLaneChange);
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

                        bool isAllowed = Program.Charter.lane.IsAPartOfRate(Core.Time, Program.Charter.division) || unlockBreakingChanges;
                        if (!isAllowed)
                        {
                            GUI.Text("Putting a BPM change offbeat is not recommended!");
                            if (GUI.Button("Force Anyway"))
                                unlockBreakingChanges = true;
                        }
                        if (!isAllowed || Core.Time <= 0f) ImGui.BeginDisabled();

                        if (ImGui.Button("Add"))
                        {
                            Program.Charter.lane.Add(Core.Time, Program.Charter.lane.initialRate);
                        }
                        if (!isAllowed || Core.Time <= 0f) ImGui.EndDisabled();
                    }
                    ImGui.PopID();

                    ImGui.TableSetColumnIndex(2);
                    GUI.TextColored(new(0f, 1f, 0f, 1f), "Camera");
                    GUI.Text($"Position: {viewport.Position}");
                    viewport.OrthographicSize = GUI.Slider("Size", viewport.OrthographicSize, 1f, 25f);
                    followSpeed = GUI.Slider("Follow Speed", followSpeed, 0f, 10f);
                    ImGui.EndTable();

                }

                if (ImGui.BeginTable("Charter", 2, ImGuiTableFlags.Borders))
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    GUI.TextColored(new(0f, 1f, 0f, 1f), "Charter");

                    ImGui.SliderInt("Division Mode", ref Program.Charter.division, 2, 13);

                    ImGui.SliderInt("Grid Lines Count", ref Program.Charter.gridLineCount, 16, 512, null);
                    if(ImGui.SliderFloat("Grid Scale", ref Program.Charter.yScale, 50f, 1000f)) Program.Charter.UpdateNotePositions();
                    ImGui.EndTable();
                }
            }
            GUI.EndWindow();
        }

        public void UpdateViewport()
        {
            if (Program.Rhine.showProperties)
            {
                Program.Rhine.viewportOffset = new(0, 20);
                Program.Rhine.viewportScaleOffset = new(300, 320);
            }
            else
            {
                Program.Rhine.viewportOffset = new(0, 20);
                Program.Rhine.viewportScaleOffset = new(300, 20);
            }
        }
        #endregion
        #region Project File IO
        public void Save()
        {
            if (Core.FilePath != null)
                SaveToPath(Core.FilePath);
            else
                MessageBox.Show("PANIC-726: Project folder doesn't exist!");
        }

        public void SaveToPath(string path) => File.WriteAllText(path, MainSerializer.Serialize(GetChartData(), true));
        public ChartData GetChartData()
        {
            var data = new ChartData()
            {
                Info = Core.Info,
                Speed = lane.initialRate,
                BPM = Program.Charter.lane.initialBPM
            };

            foreach (var change in lane.changes)
                data.Speeds.Add(new SpeedData(change));

            foreach (var bpm in Program.Charter.lane.changes)
                data.BPMs.Add(new BPMData(bpm));

            foreach (var note in notes)
                data.Notes.Add(new NoteData(note));

            return data;
        }

        public void LoadChartData(string directory, ChartData data)
        {
            ResetScene();

            Core.DirectoryPath = directory;
            Core.Info = data.Info;
            lane.initialRate = data.Speed;
            Program.Charter.lane.initialBPM = data.BPM;

            if (Core.DirectoryPath != null)
            {
                if (File.Exists(Path.Combine(Core.DirectoryPath, Core.Info.AudioPath)))
                {
                    if (music.GetHashCode() != defaultMusic.GetHashCode())
                    {
                        if (music.IsReady())
                            music.UnloadStream();
                    }

                    music = Music.Load(Path.Combine(Core.DirectoryPath, Core.Info.AudioPath));
                }
                else
                {
                    music = defaultMusic;
                }
            }

            foreach (var speed in data.Speeds)
                lane.Add(speed.Time, speed.Speed);

            foreach (var bpm in data.BPMs)
                Program.Charter.lane.Add(bpm.Time, 60f / bpm.BPM);

            foreach (var note in data.Notes)
            {
                CreateNote(note.Type, note.Direction, note.Time, note.Position);
                Program.Charter.EvaluateDirectionToCreateNote(note.Direction, note.Time);
            }
        }

        public void ResetScene()
        {
            Program.Charter.lane = new RhythmLane();
            Program.Charter.notes.Clear();

            Core.Info = new ChartInfoData();
            Core.DirectoryPath = null;
            Core.Time = 0f;
            notes.Clear();
            UpdatePlayerPosition();
            lane = new PredictableLane();

            Program.Charter.PostScrollUpdate();

            Window.SetTitle("OriNoco - None");
        }
        #endregion
    }
}
