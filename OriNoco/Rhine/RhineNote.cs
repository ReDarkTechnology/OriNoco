using OriNoco.Tweening;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Numerics;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Rendering;

namespace OriNoco.Rhine
{
    public class RhineNote
    {
        const float particleDuration = 1f;

        public NoteType type = NoteType.Tap;
        public Direction direction = Direction.Up;

        public float time = 0f;
        private float currentTime;

        private bool wasHit;
        public bool persistPosition;

        Sound sound;

        public TextureDrawable note;
        public TextureDrawable arrow;
        public Rectangle noteRect;

        public List<Particle> particles = new List<Particle>();

        public RhineNote()
        {
            note = new TextureDrawable(TextureDictionary.note);
            arrow = new TextureDrawable(TextureDictionary.arrow) { Color = Color.Black };

            InitializeParticles(5, -3f, 3f);
            sound = Program.Rhine.hitSound;
        }

        public void AdjustDrawables(Vector2 position, float scale)
        {
            note.Position = position;
            note.Rotation = 0f;
            note.Scale = new Vector2(scale, scale);

            noteRect.Width = note.Texture.Width / note.PixelsPerUnit * scale;
            noteRect.Height = note.Texture.Height / note.PixelsPerUnit * scale;
            noteRect.X = position.X - noteRect.Width / 2f;
            noteRect.Y = -position.Y - noteRect.Height / 2f;

            arrow.Position = position;
            arrow.Rotation = direction.ToRotation();
            arrow.Scale = new Vector2(scale, scale);
        }

        public void UpdateDirection(Direction direction)
        {
            this.direction = direction;
            arrow.Rotation = direction.ToRotation();
        }

        public void UpdateType(NoteType type)
        {
            if (this.type == type) return;

            if (type == NoteType.Inverse)
            {
                note.Texture = TextureDictionary.inverseNote;
                note.Color = Color.White;
                arrow.Color = Color.White;
            }
            else
            {
                note.Texture = TextureDictionary.note;
                switch (type)
                {
                    case NoteType.Tap:
                        note.Color = Color.White;
                        break;
                    case NoteType.Hold:
                        note.Color = Color.Blue;
                        break;
                    case NoteType.Drag:
                        note.Color = Color.Yellow;
                        break;
                }
                arrow.Color = Color.Black;
            }
            this.type = type;
        }

        public void Update(float time)
        {
            currentTime = time;
            foreach (var particle in particles)
                particle.Move(note.Position, Math.Clamp(time - this.time, 0f, particleDuration) / particleDuration);
        }

        public void UpdateNoteColor(bool forcePass = false)
        {
            if (time > currentTime && !forcePass)
            {
                if (Program.Rhine.IsMouseOverRect(noteRect))
                {
                    if (Program.Rhine.selectedNote == this)
                        note.Color = Core.NoteSelectedHoverColor;
                    else
                        note.Color = Core.NoteHoverColor;
                }
                else
                {
                    if (Program.Rhine.selectedNote == this)
                        note.Color = Core.NoteSelectedColor;
                    else
                        note.Color = Core.NoteColor;
                }

                wasHit = false;
            }
            else
            {
                if (!wasHit)
                {
                    if (Core.IsPlaying && Core.PlayHitSound)
                        sound.Play();
                    wasHit = true;
                }

                if (Program.Rhine.IsMouseOverRect(noteRect))
                {
                    if (Program.Rhine.selectedNote == this)
                        note.Color = Core.NoteSelectedHoverColor;
                    else
                        note.Color = Core.NoteHoverColor;
                }
                else
                {
                    note.Color = Core.NotePassedColor;
                }
            }
        }

        public bool IsMouseOverNote()
        {
            return Program.Rhine.IsMouseOverRect(noteRect);
        }

        public void Draw(RhineNote? previousNote, RhineNote? nextNote)
        {
            if (previousNote != null)
                UpdateNoteColor(previousNote.type == NoteType.Hold && currentTime >= previousNote.time);
            else
                UpdateNoteColor();

            if (type == NoteType.Hold && nextNote != null)
            {
                if (nextNote.type == NoteType.HoldEnd)
                    Graphics.DrawLineEx(note.Position.InvertY(), nextNote.note.Position.InvertY(), 0.2f, note.Color);
            }

            note.Draw();
            arrow.Draw();

            foreach (var particle in particles)
                particle.drawable.Draw();
        }

        public void InitializeParticles(int amount, float limitMin, float limitMax)
        {
            for (int i = 0; i < amount; i++)
            {
                var particle = new Particle
                {
                    direction = new Vector2(GetRandomNumber(-1f, 1f), GetRandomNumber(-1f, 1f)),
                    limit = GetRandomNumber(limitMin, limitMax)
                };
                particles.Add(particle);
            }
        }

        public static float GetRandomNumber(float min, float max, int details = 100000)
        {
            return RandomNumberGenerator.GetInt32((int)min * details, (int)max * details) / (float)details;
        }

        public class Particle
        {
            public Vector2 direction = new Vector2(0f, 0f);
            public float limit = 5f;
            public TextureDrawable drawable;

            public Particle()
            {
                drawable = new TextureDrawable(TextureDictionary.note);
                drawable.Scale = new Vector2(0.2f, 0.2f);
                drawable.Color = Color.Yellow;
            }

            public void Move(Vector2 center, float time)
            {
                drawable.Position = center + (direction * EaseFunc.CubicOut(time) * limit);
                if (time == 0)
                    drawable.Color.A = 0f;
                else
                    drawable.Color.A = 1f - EaseFunc.CubicOut(time);
            }

            public static float OutCubicFloat(float start, float end, float time) => start + ((end - start) * OutCubic(time));
            public static Vector2 OutCubicVector3(Vector2 start, Vector2 end, float time) => start + ((end - start) * OutCubic(time));

            public static float InCubic(float t) => t * t * t;
            public static float OutCubic(float t) => 1 - InCubic(1 - t);
        }
    }
}
