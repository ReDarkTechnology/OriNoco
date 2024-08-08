using Raylib_CSharp;
using System.Numerics;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace OriNoco.Rhine
{
    public class RhineParticles
    {
        public TextureDrawable? drawable;

        public float appearIntensity = 20f;
        public int maxParticles = 2000;

        public float lifetimeDuration = 10f;

        public Vector3 moveDirection = new Vector3(0, 0, -1f);
        public float moveSpeed = 5f;

        public bool rotateRandomly;
        public float randomRotationMin = 0f;
        public float randomRotationMax = 360f;

        public float randomScaleMin = 0.5f;
        public float randomScaleMax = 1.5f;

        public Vector3 startSpawnArea = new(-50f, -50f, 1000f);
        public Vector3 endSpawnArea = new(50f, 50f, 1000f);

        private float cooldown;
        private List<Particle> particles = new List<Particle>();

        public Vector3 GetRandomPositionInSpawnArea() => GetRandomVector3(startSpawnArea, endSpawnArea);
        public static Vector3 GetRandomVector3(Vector3 min, Vector3 max, int details = 100000)
        {
            return new Vector3
            (
                GetRandomNumber(min.X, max.X, details),
                GetRandomNumber(min.Y, max.Y, details),
                GetRandomNumber(min.Z, max.Z, details)
            );
        }

        public static float GetRandomNumber(float min, float max, int details = 100000)
        {
            return RandomNumberGenerator.GetInt32((int)min * details, (int)max * details) / (float)details;
        }

        public RhineParticles() { }
        public RhineParticles(TextureDrawable drawable)
        {
            this.drawable = drawable;
        }

        public void Update()
        {
            cooldown -= Time.GetFrameTime();
            if (cooldown <= 0)
            {
                for (float i = cooldown; i < 0; i += 1f / appearIntensity)
                {
                    SpawnParticle();
                    cooldown = i;
                }
            }
        }

        public void SpawnParticle()
        {
            var position = GetRandomPositionInSpawnArea();
        }

        public void Draw()
        {

        }

        public class Particle
        {
            public Vector2 direction;
            public float limit;
        }
    }
}
