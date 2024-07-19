using System;
using System.Collections.Generic;
using System.Numerics;
using OriNoco.Rhine;
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
            var existingNotes = FindNoteAtTime(Program.Time);
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
                    // Check if a note already exists in the opposite direction, if it does, don't create a new one.
                    switch (direction)
                    {
                        case Direction.Down:
                            if (existingNotes.Exists(val => val.direction == Direction.Up))
                                Console.WriteLine("Unintended behaviour was found by the chartmaker, aborting action.");
                            else
                                CreateNote(direction, Program.Time);
                            break;
                        case Direction.Up:
                            if (existingNotes.Exists(val => val.direction == Direction.Down))
                                Console.WriteLine("Unintended behaviour was found by the chartmaker, aborting action.");
                            else
                                CreateNote(direction, Program.Time);
                            break;
                        case Direction.Left:
                            if (existingNotes.Exists(val => val.direction == Direction.Right))
                                Console.WriteLine("Unintended behaviour was found by the chartmaker, aborting action.");
                            else
                                CreateNote(direction, Program.Time);
                            break;
                        case Direction.Right:
                            if (existingNotes.Exists(val => val.direction == Direction.Left))
                                Console.WriteLine("Unintended behaviour was found by the chartmaker, aborting action.");
                            else
                                CreateNote(direction, Program.Time);
                            break;
                    }
                }
            }
            else
            {
                CreateNote(direction, Program.Time);
            }
        }

        public CharterNote CreateNote(Direction direction, float time)
        {
            var note = new CharterNote(this, time, direction);
            note.UpdatePosition();
            notes.Add(note);
            return note;
        }

        public void UpdateNotePositions()
        {
            foreach (var note in notes)
                note.UpdatePosition();
        }

        public void RemoveNote(CharterNote note) => notes.Remove(note);

        public List<CharterNote> FindNoteAtTime(float time) =>
            notes.FindAll(val => MathF.Abs(val.time - time) < float.Epsilon);

        public void UpdateScroll()
        {
            mouseWheel = Input.GetMouseWheelMove();

            if (mouseWheel > 0)
            {
                Program.Time = lane.GetPreviousTime(Program.Time);
            }
            else if (mouseWheel < 0)
            {
                Program.Time = lane.GetNextTime(Program.Time);
            }

            yOffset = Program.Time * yScale;
            UpdateNotePositions();
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