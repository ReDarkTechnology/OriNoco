using System;
using System.IO;
using Raylib_CSharp.Interact;
using OriNoco.Serializer;

namespace OriNoco
{
    public static class Settings
    {
        public static string SettingsPath = Path.Combine(
            Core.DataDirectory,
            "Settings.json"
        );

        public static SettingsData Data = new SettingsData();

        public static void Load()
        {
            if (!File.Exists(SettingsPath))
            {
                Save();
            }
            else
            {
                var content = File.ReadAllText(SettingsPath);
                if (content != null)
                {
                    var settings = MainSerializer.Deserialize<SettingsData>(content);
                    if (settings != null)
                        Data = settings;
                    else
                        Console.WriteLine("Unable to read settings!");
                }
            }
        }

        public static void Save()
        {
            var content = MainSerializer.Serialize(Data, true);
            if (content != null)
            {
                if (!Directory.Exists(Core.DataDirectory))
                    Directory.CreateDirectory(Core.DataDirectory);

                File.WriteAllText(SettingsPath, content);
            }
        }
    }

    [Serializable]
    public class SettingsData
    {
        public KeyboardKey GameplayLeftKey { get; set; } = KeyboardKey.Left;
        public KeyboardKey GameplayUpKey { get; set; } = KeyboardKey.Up;
        public KeyboardKey GameplayRightKey { get; set; } = KeyboardKey.Right;
        public KeyboardKey GameplayDownKey { get; set; } = KeyboardKey.Down;

        public KeyboardKey GameplayAltLeftKey { get; set; } = KeyboardKey.A;
        public KeyboardKey GameplayAltUpKey { get; set; } = KeyboardKey.W;
        public KeyboardKey GameplayAltRightKey { get; set; } = KeyboardKey.D;
        public KeyboardKey GameplayAltDownKey { get; set; } = KeyboardKey.S;

        public bool ShowFPS { get; set; } = true;
        public bool ShowNoteCount { get; set; } = true;
        public bool ShowTime { get; set; } = true;
        public bool ShowProperties { get; set; } = true;
        public bool AlwaysAllowBreakingChanges { get; set; } = false;
    }
}
