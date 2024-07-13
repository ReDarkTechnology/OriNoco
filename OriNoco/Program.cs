using OriNoco.Rhine;
using System;

namespace OriNoco
{
    internal class Program
    {
        public static RhineScene RhineScene = new RhineScene();

        [STAThread]
        static void Main(string[] args)
        {
            Settings.Load();

            Game.RegisterScene(RhineScene);
            Game.Start();
        }
    }
}