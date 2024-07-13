using System;
using System.Text.Json;
using System.IO;
using Raylib_CSharp.Interact;

namespace OriNoco
{
    public static class Settings
    {
        public static string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OriNoco\\Settings.json";
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
                    var settings = JsonSerializer.Deserialize<SettingsData>(content);
                    if (settings != null)
                        Data = settings;
                    else
                        Console.WriteLine("Unable to read settings!");
                }
            }
        }

        public static void Save()
        {
            var content = JsonSerializer.Serialize(Data, new JsonSerializerOptions() { 
                WriteIndented = true
            });
            if (content != null)
            {
                var localDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OriNoco";
                if (!Directory.Exists(localDirectory))
                    Directory.CreateDirectory(localDirectory);

                File.WriteAllText(SettingsPath, content);
            }
        }
    }

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
    }
}
