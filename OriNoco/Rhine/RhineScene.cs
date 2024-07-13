using System.Numerics;

using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace OriNoco.Rhine
{
    public class RhineScene : Scene
    {
        private Color backgroundColor = Color.Black;
        private RhinePlayer player;
        private Viewport2D viewport;
        public float followSpeed = 1.5f;

        public RhineScene()
        {
            viewport = new Viewport2D(this);
            player = new RhinePlayer(false);
            player.freeplay = true;
        }

        public override void Init()
        {
            player.LoadTexture();
        }

        public override void Update()
        {
            player.Update();

            viewport.Position = Vector2.Lerp(viewport.Position, player.drawable.Position, followSpeed * Time.GetFrameTime());
            viewport.Update();
        }

        public override void Draw()
        {
            Graphics.ClearBackground(backgroundColor);

            Graphics.DrawText("OriNoco", 10, 10, 20, Color.White);
            Graphics.DrawText($"FPS: {Time.GetFPS()}", 10, 30, 20, Color.White);

            viewport.Begin();
            player.Draw();
            viewport.End();
        }

        public override void Shutdown()
        {

        }
    }
}
