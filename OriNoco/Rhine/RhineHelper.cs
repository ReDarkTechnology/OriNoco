using System.Numerics;

namespace OriNoco.Rhine
{
    /// <summary>
    /// A helper class to turn the enum into something much more useful
    /// Order: Left, Down, Up, Right, LeftUp, RightUp, RightDown, LeftDown
    /// </summary>
    public static class RhineHelper
    {
        public static Vector2[] directions = {
            new(-1, 0),
            new(0, -1),
            new(0, 1),
            new(1, 0),
            new(-0.70710678118654752f, 0.70710678118654752f),
            new(0.70710678118654752f, 0.70710678118654752f),
            new(0.70710678118654752f, -0.70710678118654752f),
            new(-0.70710678118654752f, -0.70710678118654752f),
            new(0, 1)
        };

        public static float[] rotations = {
            90,
            180,
            0,
            -90,
            45,
            -45,
            -135,
            135,
            0
        };

        public static bool IsDiagonal(this Direction dir)
        {
            int value = (int)dir;
            return value > 3 && value < 8;
        }
        public static bool IsNotDiagonal(this Direction dir) => ((int)dir) < 4;

        public static float ToRotation(this Direction dir) => rotations[(int)dir];
        public static Vector2 ToDirection(this Direction dir) => directions[(int)dir];
    }
}
