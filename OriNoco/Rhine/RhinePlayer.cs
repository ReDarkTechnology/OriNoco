using System.Numerics;
using System.Collections.Generic;

using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;

namespace OriNoco.Rhine
{
    public class RhinePlayer
    {
        #region Variables
        public TextureDrawable drawable;
        public bool freeplay = false;
        public bool createNotes = false;
        public bool showTail = true;
        public float speed = 5f;

        public CreateMode mode = CreateMode.Main;
        private Direction _direction = Direction.Up;
        public Direction direction = Direction.Up;

        public bool IsStarted { get; private set; }
        public bool IsControllable { get; set; }

        private List<RhineTrail> trails = [];
        private List<RhineTrail> deleteTrailQueue = [];

        public List<RhineNote> notes => Program.Rhine.notes;
        #endregion

        #region Main Methods
        public RhinePlayer(bool loadTexture = true)
        {
            if (loadTexture)
                drawable = new TextureDrawable(TextureDictionary.note);
            else
                drawable = new TextureDrawable(default);

            drawable.Color = new Color(0, 173, 91, 255);
        }

        public void LoadTexture()
        {
            drawable.Texture = TextureDictionary.note;
            drawable.Scale = new Vector2(0.2f, 0.2f);
        }

        public void Update()
        {
            if (freeplay)
                UpdateFreeplay();
            else
                UpdateGameplay();
        }

        public void Draw()
        {
            if (showTail)
                foreach (var trail in trails) trail.Draw();

            drawable.Draw();
        }

        public RhineTrail CreateTail()
        {
            var tail = new RhineTrail(drawable, direction);
            trails.Add(tail);
            return tail;
        }
        #endregion

        #region Freeplay
        private void UpdateFreeplay()
        {
            if (IsStarted)
            {
                drawable.Position += direction.ToDirection() * speed * Time.GetFrameTime();
                trails[trails.Count - 1].Stretch(direction.ToDirection(), speed);

                if (IsControllable)
                {
                    if (mode == CreateMode.MainAndDiagonals)
                    {
                        if (Input.IsKeyDown(Settings.Data.GameplayLeftKey) || 
                            Input.IsKeyDown(Settings.Data.GameplayAltLeftKey))
                        {
                            if (Input.IsKeyDown(Settings.Data.GameplayUpKey) || 
                                Input.IsKeyDown(Settings.Data.GameplayAltUpKey))
                                direction = Direction.LeftUp;
                            else if (Input.IsKeyDown(Settings.Data.GameplayDownKey) || 
                                     Input.IsKeyDown(Settings.Data.GameplayAltDownKey))
                                direction = Direction.LeftDown;
                            else
                                direction = Direction.Left;
                        }
                        else if (Input.IsKeyDown(Settings.Data.GameplayRightKey) || 
                                 Input.IsKeyDown(Settings.Data.GameplayAltRightKey))
                        {
                            if (Input.IsKeyDown(Settings.Data.GameplayUpKey) || 
                                Input.IsKeyDown(Settings.Data.GameplayAltUpKey))
                                direction = Direction.RightUp;
                            else if (Input.IsKeyDown(Settings.Data.GameplayDownKey) || 
                                     Input.IsKeyDown(Settings.Data.GameplayAltDownKey))
                                direction = Direction.RightDown;
                            else
                                direction = Direction.Right;
                        }
                        else if (Input.IsKeyDown(Settings.Data.GameplayDownKey) || 
                                 Input.IsKeyDown(Settings.Data.GameplayAltDownKey))
                            direction = Direction.Down;
                        else if (Input.IsKeyDown(Settings.Data.GameplayUpKey) || 
                                 Input.IsKeyDown(Settings.Data.GameplayAltUpKey))
                            direction = Direction.Up;
                    }
                    else if (mode == CreateMode.Main)
                    {
                        if (Input.IsKeyDown(Settings.Data.GameplayLeftKey) || 
                            Input.IsKeyDown(Settings.Data.GameplayAltLeftKey))
                            direction = Direction.Left;
                        else if (Input.IsKeyDown(Settings.Data.GameplayRightKey) || 
                                 Input.IsKeyDown(Settings.Data.GameplayAltRightKey))
                            direction = Direction.Right;
                        else if (Input.IsKeyDown(Settings.Data.GameplayDownKey) || 
                                 Input.IsKeyDown(Settings.Data.GameplayAltDownKey))
                            direction = Direction.Down;
                        else if (Input.IsKeyDown(Settings.Data.GameplayUpKey) || 
                                 Input.IsKeyDown(Settings.Data.GameplayAltUpKey))
                            direction = Direction.Up;
                    }
                    else if (mode == CreateMode.Diagonals)
                    {
                        if ((Input.IsKeyDown(Settings.Data.GameplayLeftKey) || Input.IsKeyDown(Settings.Data.GameplayAltLeftKey))
                         && (Input.IsKeyDown(Settings.Data.GameplayUpKey)   || Input.IsKeyDown(Settings.Data.GameplayAltUpKey)))
                            direction = Direction.LeftUp;
                        else if ((Input.IsKeyDown(Settings.Data.GameplayLeftKey) || Input.IsKeyDown(Settings.Data.GameplayAltLeftKey))
                              && (Input.IsKeyDown(Settings.Data.GameplayDownKey) || Input.IsKeyDown(Settings.Data.GameplayAltDownKey)))
                            direction = Direction.LeftDown;
                        else if ((Input.IsKeyDown(Settings.Data.GameplayRightKey) || Input.IsKeyDown(Settings.Data.GameplayAltRightKey))
                              && (Input.IsKeyDown(Settings.Data.GameplayUpKey)    || Input.IsKeyDown(Settings.Data.GameplayAltUpKey)))
                            direction = Direction.RightUp;
                        else if ((Input.IsKeyDown(Settings.Data.GameplayRightKey) || Input.IsKeyDown(Settings.Data.GameplayAltRightKey))
                              && (Input.IsKeyDown(Settings.Data.GameplayDownKey)  || Input.IsKeyDown(Settings.Data.GameplayAltDownKey)))
                            direction = Direction.RightDown;
                    }
                }
            }
            else
            {
                if (IsControllable)
                {
                    if (Input.IsKeyPressed(Settings.Data.GameplayUpKey) || 
                        Input.IsKeyPressed(Settings.Data.GameplayAltUpKey))
                    {
                        CreateTail();
                        Program.Rhine.music.PlayStream();
                        IsStarted = true;
                    }
                }
            }

            if (_direction != direction)
            {
                _direction = direction;
                CreateTail();

                if (createNotes)
                {
                    var note = new RhineNote
                    {
                        type = NoteType.Tap,
                        direction = direction,
                        time = Core.Time
                    };
                    note.AdjustDrawables(drawable.Position, 0.2f);
                    notes.Add(note);
                }
            }
        }
        #endregion

        #region Gameplay
        private void UpdateGameplay()
        {

        }
        #endregion

        public enum CreateMode
        {
            Main,
            Diagonals,
            MainAndDiagonals
        }
    }
}
