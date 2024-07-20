using OriNoco.Charter;
using OriNoco.Rhine;
using System;
using System.Numerics;

namespace OriNoco
{
    public static class Program
    {
        public static RhineScene Rhine = new RhineScene();
        public static CharterScene Charter = new CharterScene();

        public static float Time 
        {
            get => Rhine.time;
            set => Rhine.time = value;
        }

        [STAThread]
        static void Main(string[] args)
        {
            Settings.Load();

            Game.RegisterScene(Rhine);
            Game.RegisterScene(Charter);
            Game.Start();
        }

        #region Extension Methods
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
        #endregion
    }
}