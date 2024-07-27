using System;
using System.IO;

using OriNoco.Data;
using OriNoco.UI;
using Raylib_CSharp.Colors;

namespace OriNoco
{
    public static class Core
    {
        public static float Time = 0f;
        public static bool IsProjectOpen = false;
        public static ChartInfoData Info = new();

        public static string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OriNoco");
        public static string? DirectoryPath { get; set; } = null;
        public static string? FilePath => DirectoryPath != null ? Path.Combine(DirectoryPath, "chart.orinoco") : null;

        public static bool IsPlaying { get; set; }

        public static ColorF NotePassedColor { get; set; } = Color.Yellow;
        public static ColorF NoteSelectedHoverColor { get; set; } = new(0.8f, 1f, 1f);
        public static ColorF NoteSelectedColor { get; set; } = new(0.8f, 1f, 0.8f);
        public static ColorF NoteHoverColor { get; set; } = new(0.8f, 0.8f, 1f);
        public static ColorF NoteColor { get; set; } = new(1f, 1f, 1f);

        public static bool ShowNoteCount = true;
        public static bool ShowFPS = true;
        public static bool ShowTime = true;

        public static void Init()
        {
            ShowFPS = Settings.Data.ShowFPS;
            ShowTime = Settings.Data.ShowTime;
            ShowNoteCount = Settings.Data.ShowNoteCount;

            ProjectsWindow.RefreshProjectPanel();
        }

        public static string SanitizeFileName(string name)
        {
            var illegalChars = Path.GetInvalidFileNameChars();
            foreach (var c in illegalChars)
                name = name.Replace(c, '_');
            return name;
        }

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                dir.Create();
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public static string GetFreeDirectoryInProjects(string name)
        {
            var newDirectory = Path.Combine(DataDirectory, "Projects", name);
            if (Directory.Exists(newDirectory))
            {
                newDirectory = Path.Combine(DataDirectory, "Projects", Path.GetRandomFileName());
                return GetFreeDirectoryInProjects(newDirectory);
            }
            
            Directory.CreateDirectory(newDirectory);
            return newDirectory;
        }
    }
}
