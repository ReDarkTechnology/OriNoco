using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace OriNoco
{
    public class TextureDrawable : Drawable
    {
        private Texture2D _Texture;
        public Texture2D Texture
        {
            get => _Texture;
            set
            {
                _Texture = value;
                SourceRectangle = new(0, 0, value.Width, value.Height);
            }
        }

        private float _PixelsPerUnit = 100f;
        public float PixelsPerUnit
        {
            get => _PixelsPerUnit;
            set
            {
                _PixelsPerUnit = value;
                UpdateScale();
            }
        }

        public Color Color = Color.White;

        private Rectangle _SourceRectangle = new(0, 0, 1, 1);
        private Rectangle SourceRectangle
        {
            get => _SourceRectangle;
            set
            {
                _SourceRectangle = value;
                UpdateScale();
            }
        }

        private Vector2 _scale = new(1, 1);

        public TextureDrawable(Texture2D texture)
        {
            Texture = texture;
            SourceRectangle = new(0, 0, texture.Width, texture.Height);
            UpdateScale();
        }

        public override void Draw()
        {
            Graphics.DrawTexturePro(Texture, 
                SourceRectangle, 
                new Rectangle(Rectangle.ConstructMode.TopLeftScale, ViewportPosition, _scale),
                _scale / 2, -Rotation, Color);
        }
        public override void Draw(Viewport2D viewport) => Draw();
        public void Draw(Vector2 position, Color color)
        {
            Graphics.DrawTexturePro(Texture,
                SourceRectangle,
                new Rectangle(Rectangle.ConstructMode.TopLeftScale, position.InvertY(), _scale),
                _scale / 2, -Rotation, color);
        }

        public override void OnScaleChanged() => UpdateScale();
        private void UpdateScale()
        {
            _scale.X = SourceRectangle.Width * Scale.X / _PixelsPerUnit;
            _scale.Y = SourceRectangle.Height * Scale.Y / _PixelsPerUnit;
        }
    }
}
