using Raylib_CSharp;
using Raylib_CSharp.Colors;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OriNoco.Rhine
{
    public class RhineNote
    {
        public NoteType type = NoteType.Tap;
        public Direction direction = Direction.Up;

        public float time = 0f;

        public TextureDrawable note;
        public TextureDrawable arrow;

        public List<Particle> particles = new List<Particle>();

        public RhineNote()
        {
            note = new TextureDrawable(TextureDictionary.note);
            arrow = new TextureDrawable(TextureDictionary.arrow) { Color = Color.Black };

            InitializeParticles(8, 2f, 10f);
        }

        public void AdjustDrawables(Vector2 position, float scale)
        {
            note.Position = position;
            note.Scale = new Vector2(scale, scale);

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
            foreach (var particle in particles)
                particle.Move(time);
        }

        public void Draw()
        {
            note.Draw();
            arrow.Draw();
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
                drawable.Color = Color.Yellow;
            }

            public void Move(float time)
            {
                drawable.Position += direction * (limit * time) * Time.GetFrameTime();
            }
        }
    }
}
