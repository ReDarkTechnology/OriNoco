using System;
using System.Collections.Generic;
using System.Numerics;

using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace OriNoco.Charter
{
    public class CharterScene : Scene
    {
        public float YPadding { get; set; } = 50f;

        public TextureDrawable left = new TextureDrawable(default);
        public TextureDrawable down = new TextureDrawable(default);
        public TextureDrawable up = new TextureDrawable(default);
        public TextureDrawable right = new TextureDrawable(default);

        public TextureDrawable leftActive = new TextureDrawable(default);
        public TextureDrawable downActive = new TextureDrawable(default);
        public TextureDrawable upActive = new TextureDrawable(default);

        public RhythmLane lane = new RhythmLane();
        public TextureDrawable rightActive = new TextureDrawable(default);

        public float xSpacing = 32f;
        public float yScale = 75f;

        public CharterScene() 
        {
        }

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

            float time = lane.GetNextTime(Program.RhineScene.time);
            for (int i = 0; i < 12; i++)
            {
                float value = lane.GetValueFromTime(time);
                Graphics.DrawLineEx(new Vector2(screenCoord.X, GetScreenY(value * yScale)), new Vector2(screenCoord3.X, GetScreenY(value * yScale)), 1f, Color.Green);
                time = lane.GetNextTime(time);
            }

            leftActive.Draw(screenCoord, Color.White);
            downActive.Draw(screenCoord1, Color.White);
            upActive.Draw(screenCoord2, Color.White);
            rightActive.Draw(screenCoord3, Color.White);

            Graphics.EndScissorMode();
        }

        public override Vector2 GetViewportSize() =>  
            new(300, base.GetViewportSize().Y);

        public Vector2 GetScreenCoords(Vector2 point)
        {
            float xPosition = Window.GetScreenWidth() - 300;
            return new Vector2(xPosition + (300f / 2f) + point.X, Window.GetScreenHeight() - YPadding - point.Y);
        }

        public float GetScreenY(float y)
        {
            return Window.GetScreenHeight() - YPadding - y;
        }
    }
}
