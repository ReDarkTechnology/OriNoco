using OriNoco.Data;
using Raylib_CSharp.Colors;
using System;
using System.Collections.Generic;
using System.IO;

namespace OriNoco
{
    public static class Core
    {
        public static float Time = 0f;
        public static bool IsProjectOpen = false;
        public static ChartInfoData Info = new();

        public static string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OriNoco");
        public static string? DirectoryPath = null;
        public static string? FilePath => DirectoryPath != null ? Path.Combine(DirectoryPath, "chart.orinoco") : null;

        public static ColorF NotePassedColor = Color.Yellow;
        public static ColorF NoteSelectedHoverColor = new ColorF(0.8f, 1f, 1f);
        public static ColorF NoteSelectedColor = new ColorF(0.8f, 1f, 0.8f);
        public static ColorF NoteHoverColor = new ColorF(0.8f, 0.8f, 1f);
        public static ColorF NoteColor = new ColorF(1f, 1f, 1f);
    }
}
