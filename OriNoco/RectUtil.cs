using System.Numerics;
using Raylib_CSharp.Transformations;


namespace OriNoco
{
    public static class RectUtil
    {
        public static bool Intersects(Rectangle a, Rectangle b) =>
            a.X < b.X + b.Width && a.X + a.Width > b.X && a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;

        public static bool PointInsideRect(Rectangle rect, Vector2 point) =>
            point.X > rect.X && point.X < rect.X + rect.Width && point.Y > rect.Y && point.Y < rect.Y + rect.Height;
    }
}