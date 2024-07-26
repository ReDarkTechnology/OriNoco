using System.Numerics;
using Raylib_CSharp.Camera.Cam2D;
using Raylib_CSharp.Rendering;

namespace OriNoco
{
    /// <summary>
    /// A wrapper for Camera2D that follows Unity's standard
    /// </summary>
    public class Viewport2D
    {
        /// <summary>
        /// Unity based OrthographicSize
        /// </summary>
        public float OrthographicSize = 5f;

        /// <summary>
        /// Camera position, Camera2D's target
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(camera2D.Target.X, -camera2D.Target.Y);
            set
            {
                camera2D.Target.X = value.X;
                camera2D.Target.Y = -value.Y;
            }
        }

        /// <summary>
        /// Rotation in degrees
        /// </summary>
        public float Rotation
        {
            get => camera2D.Rotation;
            set => camera2D.Rotation = value;
        }

        private Scene scene;
        private Camera2D camera2D;

        public Viewport2D(Scene scene)
        {
            this.scene = scene;
            camera2D = new (scene.GetViewportSize() / 2, Vector2.Zero, 0f, GetSize());
        }

        /// <summary>
        /// Updates the Camera2D properties to the viewport properties based on the scene
        /// </summary>
        public void Update()
        {
            camera2D.Offset = scene.GetViewportOffset() + (scene.GetViewportSize() / 2);
            camera2D.Zoom = GetSize();
        }

        /// <summary>
        /// Adjusts the camera size based on Unity's 2D Camera
        /// </summary>
        private float GetSize() =>
            scene.GetViewportSize().Y / OrthographicSize / 2f;

        public void Begin() => Graphics.BeginMode2D(camera2D);
        public void End() => Graphics.EndMode2D();

        public Vector2 GetWorldToScreen(Vector2 position)
        {
            return camera2D.GetWorldToScreen(position.InvertY());
        }

        public Vector2 GetScreenToWorld(Vector2 position)
        {
            return camera2D.GetScreenToWorld(position);
        }
    }
}
