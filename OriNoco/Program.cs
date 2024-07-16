using OriNoco.Charter;
using OriNoco.Rhine;
using OriNoco.Tests;
using System;
using System.Numerics;

namespace OriNoco
{
    public static class Program
    {
        public static RhineScene RhineScene = new RhineScene();
        public static CharterScene CharterScene = new CharterScene();

        public static float Time 
        {
            get => RhineScene.time;
            set => RhineScene.time = value;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Settings.Load();

            Game.RegisterScene(RhineScene);
            Game.RegisterScene(CharterScene);
            Game.Start();
        }

        public static Vector2 InvertY(this Vector2 value)
        {
            value.Y = -value.Y;
            return value;
        }

        public static Vector2 InvertX(this Vector2 value)
        {
            value.X = -value.X;
            return value;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static int AsInt(this float val)
        {
            return Convert.ToInt32(val);
        }
    }
}