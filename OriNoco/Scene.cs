using Raylib_CSharp.Windowing;
using System.Numerics;

namespace OriNoco
{
    public class Scene
    {
        /// <summary>
        /// Called when the library is initialized
        /// </summary>
        public virtual void Init() {}

        /// <summary>
        /// Called every frame before Draw()
        /// </summary>
        public virtual void Update() {}

        /// <summary>
        /// Called every frame after Update()
        /// </summary>
        public virtual void Draw() { }

        /// <summary>
        /// Called every frame after Draw()
        /// </summary>
        public virtual void DrawGUI() { }

        /// <summary>
        /// Called every frame after DrawGUI()
        /// </summary>
        public virtual void PostRender() { }

        /// <summary>
        /// Called at the end of the program
        /// </summary>
        public virtual void Shutdown() {}

        /// <summary>
        /// Gets the viewport size, also used for Viewport2D and Viewport3D
        /// </summary>
        public virtual Vector2 GetViewportSize() =>
            new Vector2(Window.GetScreenWidth(), Window.GetScreenHeight());

        /// <summary>
        /// Gets the viewport offset, also used for Viewport2D and Viewport3D
        /// </summary>
        public virtual Vector2 GetViewportOffset() => Vector2.Zero;
    }
}
