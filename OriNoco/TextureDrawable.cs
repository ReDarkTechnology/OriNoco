using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace OriNoco
{
    public class TextureDrawable
    {
        public Texture2D texture;

        public float pixelsPerUnit = 100f;
        public Rectangle sourceRect = new(0, 0, 1, 1);
        public Vector2 position = new(0, 0);
        public float rotation = 0;
        public Vector2 scale = new(1, 1);
        public Color color = Color.White;

        private Vector2 _scale = new(1, 1);
        private Vector2 _position = new(0, 0);

        public TextureDrawable(Texture2D texture)
        {
            this.texture = texture;

            _scale.X = texture.Width * scale.X / pixelsPerUnit;
            _scale.Y = texture.Height * scale.Y / pixelsPerUnit;

            sourceRect = new(0, 0, texture.Width, texture.Height);
        }

        public void Draw()
        {
            _scale.X = texture.Width * scale.X / pixelsPerUnit;
            _scale.Y = texture.Height * scale.Y / pixelsPerUnit;

            _position.X = position.X;
            _position.Y = -position.Y;

            Graphics.DrawTexturePro(texture, 
                sourceRect, 
                new Rectangle(Rectangle.ConstructMode.TopLeftScale, _position, _scale),
                _scale / 2, rotation, color);
        }
    }
}
