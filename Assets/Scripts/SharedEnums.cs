﻿using UnityEngine;

namespace OriNoco
{
    public enum NoteType
    {
        Tap = 0,
        Drag = 1,
        Hold = 2,
        Inverse = 3
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

    public enum NoteCreateState
    {
        None,
        StartCreating,
        Creating,
        EndCreating,
        Cancelled
    }

    /// <summary>
    /// A helper class to turn the enum into something much more useful
    /// Order: Tap, Drag, Hold, HoldEnd, Inverse
    /// Order: Left, Down, Up, Right, LeftUp, RightUp, RightDown, LeftDown
    /// </summary>
    public static class SharedHelper
    {
        private static readonly Vector2[] Directions = {
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

        private static readonly float[] Rotations = {
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
            var value = (int)dir;
            return value is > 3 and < 8;
        }
        public static bool IsNotDiagonal(this Direction dir) => ((int)dir) < 4;

        public static float ToRotation(this Direction dir) => Rotations[(int)dir];
        public static Vector2 ToDirection(this Direction dir) => Directions[(int)dir];
    }
}