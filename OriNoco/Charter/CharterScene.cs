﻿using System;
using System.Collections.Generic;
using System.Numerics;

using ImGuiNET;
using OriNoco.Rhine;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

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
        public CharterNote? holdStartNote = null;

        public Point viewportPosition = new Point();
        public Size viewportSize = new Size();
        public Rectangle viewportRect = new Rectangle();

        public NoteType createNoteType = NoteType.Tap;

        public NoteType[] createNoteTypes = { 
            NoteType.Tap,
            NoteType.Drag,
            NoteType.Inverse,
            NoteType.Hold,
        };

        public string[] createNoteNames =
        {
            "Tap",
            "Drag",
            "Inverse",
            "Hold",
        };

        public int createTypeIndex = 0;

        public float yScale = 240f;
        public float yOffset = 0;
        public int division = 4;
        public int gridLineCount = 64;

        public ColorF judgementNoteColor = new ColorF(0.8f, 0.8f, 0.8f, 1f);
        public List<CharterNote> notes = new List<CharterNote>();

        private float mouseWheel;
        public const float xSpacing = 32f;
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

            viewportPosition.X = Window.GetScreenWidth() - 300;
            viewportPosition.Y = 20;

            viewportSize.Width = 300;
            viewportSize.Height = Window.GetScreenHeight() - 20;

            viewportRect.X = viewportPosition.X;
            viewportRect.Y = viewportPosition.Y;
            viewportRect.Width = viewportSize.Width;
            viewportRect.Height = viewportSize.Height;
        }

        public void ReadInputs()
        {
            if (Input.IsKeyPressed(Settings.Data.GameplayLeftKey) || Input.IsKeyPressed(Settings.Data.GameplayAltLeftKey))
                OnActionPressed(Direction.Left);
            if (Input.IsKeyPressed(Settings.Data.GameplayRightKey) || Input.IsKeyPressed(Settings.Data.GameplayAltRightKey))
                OnActionPressed(Direction.Right);
            if (Input.IsKeyPressed(Settings.Data.GameplayUpKey) || Input.IsKeyPressed(Settings.Data.GameplayAltUpKey))
                OnActionPressed(Direction.Up);
            if (Input.IsKeyPressed(Settings.Data.GameplayDownKey) || Input.IsKeyPressed(Settings.Data.GameplayAltDownKey))
                OnActionPressed(Direction.Down);

            if (Input.IsKeyPressed(Settings.Data.NextNoteType))
            {
                createTypeIndex = (createTypeIndex + 1) % createNoteTypes.Length;
                createNoteType = createNoteTypes[createTypeIndex];
                Console.WriteLine($"Next note type: {createNoteType}");
            }

            if (Input.IsKeyPressed(Settings.Data.PreviousNoteType))
            {
                createTypeIndex = (createTypeIndex - 1 + createNoteTypes.Length) % createNoteTypes.Length;
                createNoteType = createNoteTypes[createTypeIndex];
                Console.WriteLine($"Previous note type: {createNoteType}");
            }
        }

        public void OnActionPressed(Direction direction)
        {
            if (GUI.IsEditing()) return;

            if (createNoteType == NoteType.Hold) CreateHoldNote(direction);
            else if (createNoteType == NoteType.HoldEnd) CreateHoldEndNote();
            else CreateNormalNote(direction);
        }

        public void CreateNormalNote(Direction direction)
        {
            Program.Rhine.queueType = createNoteType;
            var existingNotes = FindNotesAtTime(Core.Time);
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
                                    CreateNote(direction, Core.Time);
                                break;
                            case Direction.Up:
                                if (existingNotes.Exists(val => val.direction == Direction.Down))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Core.Time);
                                break;
                            case Direction.Left:
                                if (existingNotes.Exists(val => val.direction == Direction.Right))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Core.Time);
                                break;
                            case Direction.Right:
                                if (existingNotes.Exists(val => val.direction == Direction.Left))
                                    Console.WriteLine("Note in the opposite direction is not allowed!");
                                else
                                    CreateNote(direction, Core.Time);
                                break;
                        }
                    }
                }
            }
            else
            {
                CreateNote(direction, Core.Time);
            }
        }

        public void CreateHoldNote(Direction direction)
        {
            var existingNotes = FindNotesAtTime(Core.Time);
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
                    if (existingNotes.Count >= 1)
                    {
                        if (MessageBox.Show("Are you sure you want to replace this note with the current direction and type?", "Confirmation", MessageBoxType.YesNo, MessageBoxIcon.Warning) == Result.Yes)
                        {
                            RemoveNotes(Core.Time);

                            holdStartNote = CreateNote(direction, Core.Time);
                            createNoteType = NoteType.HoldEnd;
                            createTypeIndex = -1;
                        }
                    }
                    else
                    {
                        holdStartNote = CreateNote(direction, Core.Time);
                        createNoteType = NoteType.HoldEnd;
                        createTypeIndex = -1;
                    }
                }
            }
            else
            {
                holdStartNote = CreateNote(direction, Core.Time);
                createNoteType = NoteType.HoldEnd;
                createTypeIndex = -1;
            }
        }

        public void CreateHoldEndNote()
        {
            if (holdStartNote != null)
            {
                if (holdStartNote.time == Core.Time)
                {
                    Program.Rhine.queueType = NoteType.Tap;
                    Program.Rhine.UpdateNote(Core.Time);
                }
                else if (holdStartNote.time > Core.Time)
                {
                    Console.WriteLine("This ain't recommended bud...");
                    var notesAround = FindNotesNonStartAroundTime(Core.Time, holdStartNote.time);
                    if (notesAround.Count > 0)
                    {
                        if(MessageBox.Show("There are notes in between the hold note, do you want to remove it?\n" +
                            "The notes inbetween needs to be removed in order to create a hold note", "Confirmation", MessageBoxType.YesNo, MessageBoxIcon.Warning) == Result.Yes)
                        {
                            foreach(var note in notesAround) RemoveNote(note);

                            // Update the note at the start of the hold as the end hold note instead
                            Program.Rhine.queueType = NoteType.HoldEnd;
                            Program.Rhine.UpdateNote(holdStartNote.time);

                            Program.Rhine.queueType = NoteType.Hold;
                            CreateNote(holdStartNote.direction, Core.Time);

                            createNoteType = NoteType.Hold;
                            createTypeIndex = GetCreateTypeIndex(NoteType.Hold);
                        }
                        else
                        {
                            RemoveSingleNote(holdStartNote, true);
                            createNoteType = NoteType.Tap;
                            createTypeIndex = 0;
                        }
                    }
                    else
                    {
                        Program.Rhine.queueType = NoteType.HoldEnd;
                        Program.Rhine.UpdateNote(holdStartNote.time);

                        Program.Rhine.queueType = NoteType.Hold;
                        CreateNote(holdStartNote.direction, Core.Time);

                        createNoteType = NoteType.Hold;
                        createTypeIndex = GetCreateTypeIndex(NoteType.Hold);
                    }
                }
                else
                {
                    var notesAround = FindNotesNonStartAroundTime(holdStartNote.time, Core.Time);
                    if (notesAround.Count > 0)
                    {
                        if (MessageBox.Show("There are notes in between the hold note, do you want to remove it?\n" +
                            "The notes inbetween needs to be removed in order to create a hold note", "Confirmation", MessageBoxType.YesNo, MessageBoxIcon.Warning) == Result.Yes)
                        {
                            foreach (var note in notesAround) RemoveNote(note);

                            Program.Rhine.queueType = NoteType.HoldEnd;
                            CreateNote(holdStartNote.direction, Core.Time);

                            createNoteType = NoteType.Hold;
                            createTypeIndex = GetCreateTypeIndex(NoteType.Hold);
                        }
                        else
                        {
                            RemoveSingleNote(holdStartNote, true);
                        }
                    }
                    else
                    {
                        Program.Rhine.queueType = NoteType.HoldEnd;
                        CreateNote(holdStartNote.direction, Core.Time);

                        createNoteType = NoteType.Hold;
                        createTypeIndex = GetCreateTypeIndex(NoteType.Hold);
                    }
                }
            }
            else
            {
                MessageBox.Show("PANIC-243: You didn't actually started making any hold notes... okay...");
                createNoteType = NoteType.Tap;
                createTypeIndex = 0;
            }
        }

        public int GetCreateTypeIndex(NoteType type)
        {
            for(int i = 0; i < createNoteTypes.Length; i++)
                if (createNoteTypes[i] == type) return i;
            return -1;
        }

        public CharterNote CreateNote(Direction direction, float time, bool refresh = true)
        {
            if (Program.Rhine.adjustToGrid && Core.IsPlaying)
                time = lane.AdjustTimeToRate(time, division);

            var note = new CharterNote(this, time, direction);
            note.UpdatePosition();
            notes.Add(note);

            if (refresh)
            {
                Program.Rhine.queueType = createNoteType;
                Program.Rhine.UpdateNote(time);
            }
            return note;
        }

        public CharterNote[] EvaluateDirectionToCreateNote(Direction direction, float time, bool refresh = false)
        {
            switch (direction)
            {
                case Direction.Left:
                    var n1 = CreateNote(Direction.Left, time, false);
                    return [n1];
                case Direction.Down:
                    n1 = CreateNote(Direction.Down, time, false);
                    return [n1];
                case Direction.Up:
                    n1 = CreateNote(Direction.Up, time, false);
                    return [n1];
                case Direction.Right:
                    n1 = CreateNote(Direction.Right, time, false);
                    return [n1];
                case Direction.LeftUp:
                    n1 = CreateNote(Direction.Left, time, false);
                    var n2 = CreateNote(Direction.Up, time, false);
                    return [n1, n2];
                case Direction.LeftDown:
                    n1 = CreateNote(Direction.Left, time, false);
                    n2 = CreateNote(Direction.Down, time, false);
                    return [n1, n2];
                case Direction.RightUp:
                    n1 = CreateNote(Direction.Right, time, false);
                    n2 = CreateNote(Direction.Up, time, false);
                    return [n1, n2];
                case Direction.RightDown:
                    n1 = CreateNote(Direction.Right, time, false);
                    n2 = CreateNote(Direction.Down, time, false);
                    return [n1, n2];
            }

            return [];
        }

        public void UpdateNotePositions()
        {
            foreach (var note in notes)
                note.UpdatePosition();
        }

        public void RemoveNote(CharterNote note, bool refresh = true)
        {
            var type = Program.Rhine.GetTypeAtTime(note.time);
            if (type == NoteType.Hold)
            {
                var holdEnd = Program.Rhine.GetNextNoteAtTime(note.time, NoteType.HoldEnd);

                if (holdEnd != null)
                {
                    RemoveNotes(note.time, refresh);
                    RemoveNotes(holdEnd.time, refresh);
                }
                else
                {
                    MessageBox.Show("PANIC-634 - What is happening? HoldStart found but no HoldEnd found.");
                }
            }
            else if (type == NoteType.HoldEnd)
            {
                var hold = Program.Rhine.GetPreviousNoteAtTime(note.time, NoteType.Hold);

                if (hold != null)
                {
                    RemoveNotes(note.time, refresh);
                    RemoveNotes(hold.time, refresh);
                }
                else
                {
                    MessageBox.Show("PANIC-635 - I cannot comprehend this, HoldEnd found but no HoldStart found.");
                }
            }
            else
            {
                RemoveSingleNote(note, refresh);
            }
        }

        public void RemoveSingleNote(CharterNote note, bool refresh = true)
        {
            float time = note.time;
            notes.Remove(note);

            if (refresh)
                Program.Rhine.UpdateNote(time);
        }

        public void RemoveNotes(float time, bool refresh = true)
        {
            var notesToRemove = FindNotesAtTime(time);
            foreach (var note in notesToRemove)
                RemoveSingleNote(note, false);

            if (refresh)
                Program.Rhine.DeleteNote(time);
        }

        public List<CharterNote> FindNotesAtTime(float time) =>
            notes.FindAll(val => MathF.Abs(val.time - time) < Program.TolerableEpsilon);

        public List<CharterNote> FindNotesAroundTime(float startTime, float endTime)
        {
            if (startTime > endTime)
                (startTime, endTime) = (endTime, startTime);

            return notes.FindAll(val => val.time > startTime - Program.TolerableEpsilon && val.time < endTime + Program.TolerableEpsilon);
        }

        public List<CharterNote> FindNotesNonStartAroundTime(float startTime, float endTime)
        {
            if (startTime > endTime)
                (startTime, endTime) = (endTime, startTime);

            return notes.FindAll(val => val.time > startTime - Program.TolerableEpsilon && val.time < endTime + Program.TolerableEpsilon && val != holdStartNote);
        }

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
                    Core.Time = lane.GetPreviousTime(Core.Time, division);
                    PostScrollUpdate();
                }
                else if (mouseWheel < 0)
                {
                    Core.Time = lane.GetNextTime(Core.Time, division);
                    PostScrollUpdate();
                }
            }
        }

        public void PostScrollUpdate()
        {
            yOffset = Core.Time * yScale;
            UpdateNotePositions();
            Program.Rhine.UpdatePlayerPosition();
        }

        public override void Draw()
        {
            if (!Core.IsProjectOpen) return;

            Graphics.BeginScissorMode(viewportPosition.X, viewportPosition.Y, viewportSize.Width, viewportSize.Height);

            Graphics.ClearBackground(new ColorF(0.1f, 0.1f, 0.1f));

            var screenCoord = GetScreenCoords(new Vector2(-xSpacing * 3f, 0f)).InvertY();
            var screenCoord1 = GetScreenCoords(new Vector2(-xSpacing * 1f, 0f)).InvertY();
            var screenCoord2 = GetScreenCoords(new Vector2(xSpacing * 1f, 0f)).InvertY();
            var screenCoord3 = GetScreenCoords(new Vector2(xSpacing * 3f, 0f)).InvertY();

            Graphics.DrawLineEx(new Vector2(screenCoord.X, 0), new Vector2(screenCoord.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);
            Graphics.DrawLineEx(new Vector2(screenCoord1.X, 0), new Vector2(screenCoord1.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);
            Graphics.DrawLineEx(new Vector2(screenCoord2.X, 0), new Vector2(screenCoord2.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);
            Graphics.DrawLineEx(new Vector2(screenCoord3.X, 0), new Vector2(screenCoord3.X, Window.GetScreenHeight() - YPadding), 1f, Color.Red);

            float time = lane.GetNextTime(Core.Time, division);
            for (int i = 0; i < gridLineCount; i++)
            {
                Graphics.DrawLineEx(new Vector2(screenCoord.X, GetScreenY(time * yScale)), new Vector2(screenCoord3.X, GetScreenY(time * yScale)), 1f, lane.IsAPartOfRate(time) ? Color.Blue : Color.Green);
                time = lane.GetNextTime(time, division);
            }

            leftActive.Draw(screenCoord, judgementNoteColor);
            downActive.Draw(screenCoord1, judgementNoteColor);
            upActive.Draw(screenCoord2, judgementNoteColor);
            rightActive.Draw(screenCoord3, judgementNoteColor);

            foreach (var note in notes)
                note.Draw();

            Graphics.EndScissorMode();
        }

        public bool IsMouseOverRect(Rectangle rectangle)
        {
            Vector2 mousePosition = Input.GetMousePosition();
            return RectUtil.PointInsideRect(viewportRect, mousePosition) && RectUtil.PointInsideRect(rectangle, mousePosition);
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