using OriNoco.Tweening;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace OriNoco.Rhine
{
    public class RhineNote
    {
        const float particleDuration = 10f;

        public NoteType type = NoteType.Tap;
        public Direction direction = Direction.Up;

        public float time = 0f;
        private float currentTime;
        public bool persistPosition;

        public TextureDrawable note;
        public TextureDrawable arrow;
        public Rectangle noteRect;

        public List<Particle> particles = new List<Particle>();

        public RhineNote()
        {
            note = new TextureDrawable(TextureDictionary.note);
            arrow = new TextureDrawable(TextureDictionary.arrow) { Color = Color.Black };

            InitializeParticles(4, -2f, 2f);
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

        public void Update(float time)
        {
            currentTime = time;
            foreach (var particle in particles)
                particle.Move(note.Position, Math.Clamp(time - this.time, 0f, particleDuration) / particleDuration);
        }

        public void UpdateNoteColor()
        {
            if (time > currentTime)
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
            }
            else
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
                    note.Color = Core.NotePassedColor;
                }
            }
        }

        public bool IsMouseOverNote()
        {
            return Program.Rhine.IsMouseOverRect(noteRect);
        }

        public void Draw()
        {
            note.Draw();
            arrow.Draw();

            foreach (var particle in particles)
                particle.drawable.Draw();
        }

        public void InitializeParticles(int amount, float limitMin, float limitMax)
        {
            var random = new Random();
            for (int i = 0; i < amount; i++)
            {
                var particle = new Particle
                {
                    direction = new Vector2((float)random.NextDouble(), (float)random.NextDouble()),
                    limit = (float)random.NextDouble() * (limitMax - limitMin) + limitMin
                };
                particles.Add(particle);
            }
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
