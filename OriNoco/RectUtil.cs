using Raylib_CSharp.Transformations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace OriNoco
{
    public static class RectUtil
    {
        public static bool Intersects(Rectangle a, Rectangle b) =>
            a.X < b.X + b.Width && a.X + a.Width > b.X && a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;


    }
}