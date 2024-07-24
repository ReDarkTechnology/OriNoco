using OriNoco.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace OriNoco
{
    public static class Core
    {
        public static float Time = 0f;
        public static bool IsProjectOpen;
        public static ChartInfoData Info = new();

        public static string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OriNoco");
        public static string? DirectoryPath = null;
        public static string? FilePath = null;
    }
}
