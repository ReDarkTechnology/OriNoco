using UnityEngine;

namespace OriNoco
{
    public enum NoteType
    {
        Tap = 0,
        Drag = 1,
        HoldStart = 2,
        HoldEnd = 3,
        Inverse = 4
    }

    public enum Direction
    {
        Left = 0,
        Down = 1,
        Up = 2,
        Right = 3,
        LeftUp = 4,
        RightUp = 5,
        RightDown = 6,
        LeftDown = 7,
        None = 8
    }

    /// <summary>
    /// A helper class to turn the enum into something much more useful
    /// Order: Tap, Drag, Hold, HoldEnd, Inverse
    /// Order: Left, Down, Up, Right, LeftUp, RightUp, RightDown, LeftDown
    /// </summary>
    public static class SharedHelper
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