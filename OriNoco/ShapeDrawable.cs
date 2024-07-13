using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace OriNoco
{
    public class ShapeDrawable : Drawable
    {
        public ShapeType Shape = ShapeType.Rectangle;

        public Color Color = Color.White;

        public ShapeDrawable() {}
        public ShapeDrawable(ShapeType shape) { Shape = shape; }

        public override void Draw()
        {
            switch (Shape)
            {
                case ShapeType.Rectangle:
                    Graphics.DrawRectanglePro(new Rectangle(Rectangle.ConstructMode.TopLeftScale, ViewportPosition, Scale), Scale / 2, -Rotation, Color.White);
                    break;
                case ShapeType.Circle:
                    Graphics.DrawCircleV(ViewportPosition, Scale.X / 2, Color.White);
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