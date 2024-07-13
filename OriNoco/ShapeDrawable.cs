using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace OriNoco
{
    public class ShapeDrawable
    {
        public ShapeType shape = ShapeType.Rectangle;

        public Color color = Color.White;
        public Vector2 position = new(0, 0);
        public float rotation = 0;
        public Vector2 scale = new(1, 1);

        private Vector2 _position = new(0, 0);

        public ShapeDrawable() 
        {

        }

        public virtual void Draw()
        {
            _position.X = position.X;
            _position.Y = -position.Y;

            switch (shape)
            {
                case ShapeType.Rectangle:
                    Graphics.DrawRectanglePro(new Rectangle(Rectangle.ConstructMode.TopLeftScale, _position, scale), scale / 2, rotation, Color.White);
                    break;
                case ShapeType.Circle:
                    Graphics.DrawCircleV(_position, scale.X / 2, Color.White);
                    break;
                default:
                    break;
            }
        }

        public enum ShapeType
        {
            Rectangle,
            Circle
        }
    }
}
