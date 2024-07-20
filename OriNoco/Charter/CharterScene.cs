using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using ImGuiNET;
using OriNoco.Rhine;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace OriNoco.Charter
{
    public class CharterScene : Scene
    {
        public float YPadding { get; set; } = 50f;

        public TextureDrawable left = new (default);
        public TextureDrawable down = new (default);
        public TextureDrawable up = new (default);
        public TextureDrawable right = new (default);

        public TextureDrawable leftActive = new (default);
        public TextureDrawable downActive = new (default);
        public TextureDrawable upActive = new (default);

        public RhythmLane lane = new();
        public TextureDrawable rightActive = new(default);

        public float xSpacing = 32f;
        public float yScale = 75f;
        public float yOffset = 0;
        private float mouseWheel;

        public ColorF judgementNoteColor = new ColorF(0.8f, 0.8f, 0.8f, 1f);
        public List<CharterNote> notes = new List<CharterNote>();

        public CharterScene() {}

        public override void Init()
        {
            left.Texture = TextureDictionary.left;
            down.Texture = TextureDictionary.down;
            up.Texture = TextureDictionary.up;
            right.Texture = TextureDictionary.right;

            left.PixelsPerUnit = 1f;
            down.PixelsPerUnit = 1f;
            up.PixelsPerUnit = 1f;
            right.PixelsPerUnit = 1f;

            leftActive.Texture = TextureDictionary.leftActive;
            downActive.Texture = TextureDictionary.downActive;
            upActive.Texture = TextureDictionary.upActive;
            rightActive.Texture = TextureDictionary.rightActive;

            leftActive.PixelsPerUnit = 1f;
            downActive.PixelsPerUnit = 1f;
            upActive.PixelsPerUnit = 1f;
            rightActive.PixelsPerUnit = 1f;
        }

        public override void Update() 
        {
            UpdateScroll();
            ReadInputs();
        }

        public override void DrawGUI()
        {
            ImGui.BeginMainMenuBar();
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New"))


                if (ImGui.MenuItem("Open"))
                {
                }
                if (ImGui.MenuItem("Save"))
                {
                }
                if (ImGui.MenuItem("Save As"))
                {
                }
                if (ImGui.MenuItem("Exit"))
                {
                    Window.Close();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Refresh"))
                {
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Run"))
            {
                ImGui.Text("Not yet :(");
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        public void ReadInputs()
        {
            if (Input.IsKeyPressed(KeyboardKey.Left))
                OnActionPressed(Direction.Left);
            if (Input.IsKeyPressed(KeyboardKey.Right))
                OnActionPressed(Direction.Right);
            if (Input.IsKeyPressed(KeyboardKey.Up))
                OnActionPressed(Direction.Up);
            if (Input.IsKeyPressed(KeyboardKey.Down))
                OnActionPressed(Direction.Down);
        }

        public void OnActionPressed(Direction direction)
        {
            var existingNotes = FindNotesAtTime(Program.Time);
            if (existingNotes.Count > 0)
            {
                var sameNote = existingNotes.Find(val => val.direction == direction);
                if (sameNote != null)
                {
                    // Remove a note if it is already at the same position and direction
                    RemoveNote(sameNote);
                }
                else
                {
                    if (existingNotes.Count >= 2)
                    {
                        Console.WriteLine("3 notes at the same time is not allowed!");
                    }
                    else
                    {
                        // Check if a note already exists in the opposite direction, if it does, don't create a new one.
                        switch (direction)
                        {
                            case Direction.Down:
                                if (existingNotes.Exists(val => val.direction == Direction.Up))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Program.Time);
                                break;
                            case Direction.Up:
                                if (existingNotes.Exists(val => val.direction == Direction.Down))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Program.Time);
                                break;
                            case Direction.Left:
                                if (existingNotes.Exists(val => val.direction == Direction.Right))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Program.Time);
                                break;
                            case Direction.Right:
                                if (existingNotes.Exists(val => val.direction == Direction.Left))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Program.Time);
                                break;
                        }
                    }
                }
            }
            else
            {
                CreateNote(direction, Program.Time);
            }
        }

        public CharterNote CreateNote(Direction direction, float time, bool refresh = true)
        {
            var note = new CharterNote(this, time, direction);
            note.UpdatePosition();
            notes.Add(note);

            if (refresh)
                Program.RhineScene.UpdateNote(time);
            return note;
        }

        public void UpdateNotePositions()
        {
            foreach (var note in notes)
                note.UpdatePosition();
        }

        public void RemoveNote(CharterNote note, bool refresh = true)
        {
            float time = note.time;
            notes.Remove(note);

            if (refresh)
                Program.RhineScene.UpdateNote(time);
        }

        public List<CharterNote> FindNotesAtTime(float time) =>
            notes.FindAll(val => MathF.Abs(val.time - time) < float.Epsilon);

        public Direction GetDirectionAtTime(float time)
        {
            var notes = FindNotesAtTime(time);
            var direction = Direction.None;

            if (notes.Count > 0)
            {
                direction = notes[0].direction;
                if (notes.Count > 1)
                {
                    if (notes[1].direction == Direction.Left && direction == Direction.Up)
                        direction = Direction.LeftUp;
                    else if (notes[1].direction == Direction.Left && direction == Direction.Down)
                        direction = Direction.LeftDown;
                    else if (notes[1].direction == Direction.Right && direction == Direction.Up)
                        direction = Direction.RightUp;
                    else if (notes[1].direction == Direction.Right && direction == Direction.Down)
                        direction = Direction.RightDown;
                    else if (notes[1].direction == Direction.Up && direction == Direction.Left)
                        direction = Direction.LeftUp;
                    else if (notes[1].direction == Direction.Up && direction == Direction.Right)
                        direction = Direction.RightUp;
                    else if (notes[1].direction == Direction.Down && direction == Direction.Left)
                        direction = Direction.LeftDown;
                    else if (notes[1].direction == Direction.Down && direction == Direction.Right)
                        direction = Direction.RightDown;
                }
            }

            return direction;
        }

        public void UpdateScroll()
        {
            mouseWheel = Input.GetMouseWheelMove();

            if (!GUI.IsOverAnyElement)
            {
                if (mouseWheel > 0)
                {
                    Program.Time = lane.GetPreviousTime(Program.Time);
                    PostScrollUpdate();
                }
                else if (mouseWheel < 0)
                {
                    Program.Time = lane.GetNextTime(Program.Time);
                    PostScrollUpdate();
                }
            }
        }

        public void PostScrollUpdate()
        {
            yOffset = Program.Time * yScale;
            UpdateNotePositions();
            Program.RhineScene.UpdatePlayerPosition();
        }

        public override void Draw()
        {
            int xPosition = Window.GetScreenWidth() - 300;
            Graphics.BeginScissorMode(xPosition, 0, 300, Window.GetScreenHeight());

            Graphics.ClearBackground(new ColorF(0.1f, 0.1f, 0.1f));

            var screenCoord = GetScreenCoords(new Vector2(-xSpacing * 3f, 0f)).InvertY();
            var screenCoord1 = GetScreenCoords(new Vector2(-xSpacing * 1f, 0f)).InvertY();
            var screenCoord2 = GetScreenCoords(new Vector2(xSpacing * 1f, 0f)).InvertY();
            var screenCoord3 = GetScreenCoords(new Vector2(xSpacing * 3f, 0f)).InvertY();

            Graphics.DrawLineEx(new Vector2(screenCoord.X, 0), new Vector2(screenCoord.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);
            Graphics.DrawLineEx(new Vector2(screenCoord1.X, 0), new Vector2(screenCoord1.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);
            Graphics.DrawLineEx(new Vector2(screenCoord2.X, 0), new Vector2(screenCoord2.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);
            Graphics.DrawLineEx(new Vector2(screenCoord3.X, 0), new Vector2(screenCoord3.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);

            float time = lane.GetNextTime(Program.Time);
            for (int i = 0; i < 12; i++)
            {
                Graphics.DrawLineEx(new Vector2(screenCoord.X, GetScreenY(time * yScale)), new Vector2(screenCoord3.X, GetScreenY(time * yScale)), 1f, Color.Green);
                time = lane.GetNextTime(time);
            }

            leftActive.Draw(screenCoord, judgementNoteColor);
            downActive.Draw(screenCoord1, judgementNoteColor);
            upActive.Draw(screenCoord2, judgementNoteColor);
            rightActive.Draw(screenCoord3, judgementNoteColor);

            foreach (var note in notes)
                note.Draw();

            Graphics.EndScissorMode();
        }

        public override Vector2 GetViewportSize() =>  
            new(300, base.GetViewportSize().Y);

        public Vector2 GetScreenCoords(Vector2 point, bool useOffset = false)
        {
            float xPosition = Window.GetScreenWidth() - 300;
            return new Vector2(xPosition + (300f / 2f) + point.X, Window.GetScreenHeight() - YPadding - point.Y + (useOffset ? yOffset : 0));
        }

        public float GetScreenY(float y)
        {
            return Window.GetScreenHeight() - YPadding - y + yOffset;
        }
    }
}