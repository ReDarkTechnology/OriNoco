using ImGuiNET;
using OriNoco.Data;
using OriNoco.Serializer;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Windowing;
using SharpFileDialog;
using System.IO;
using System.Numerics;

namespace OriNoco.UI
{
    public static class NewProjectWindow
    {
        public static bool Enabled = false;
        public static ChartInfoData newChartInfo = new ChartInfoData();
        public static string AudioPath = "";

        public static void Draw()
        {
            if (!Enabled) return;

            Vector2 size = new Vector2(300f, 170f);
            ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos((Game.WindowSizeF - size) / 2f, ImGuiCond.FirstUseEver);
            GUI.BeginWindow("New Project", ref Enabled);
            {
                newChartInfo.Name = GUI.InputText("Name", newChartInfo.Name, 256);
                AudioPath = GUI.InputText("Audio File", AudioPath, 256);

                if (GUI.Button("Browse Audio File")) BrowseAudioFile();
                if (GUI.Button("Create")) TryCreate();
            }
            GUI.EndWindow();
        }

        private static void BrowseAudioFile()
        {
            var filter = new NativeFileDialog.Filter() { Name = "Audio Files", Extensions = ["ogg", "mp3"] };
            var filter2 = new NativeFileDialog.Filter() { Name = "All Files", Extensions = ["*"] };
            if (NativeFileDialog.OpenDialog([filter, filter2], null, out string? path))
            {
                var audio = Music.Load(path);
                if (audio.IsReady())
                    AudioPath = path;
                else
                    MessageBox.Show("This audio file cannot be loaded! (Not Supported, I think)");
            }
        }

        private static void TryCreate()
        {
            if (AudioPath == null || !File.Exists(AudioPath))
            {
                if (MessageBox.Show("You don't seem to attach any audio file, are you sure you want to chart this way?", "Confirmation", MessageBoxType.YesNo) == Result.No)
                    return;
            }

            var newDirectory = Core.GetFreeDirectoryInProjects(Core.SanitizeFileName(newChartInfo.Name));

            if (AudioPath != null && File.Exists(AudioPath))
            {
                File.Copy(AudioPath, Path.Combine(newDirectory, "audio" + Path.GetExtension(AudioPath)));
                newChartInfo.AudioPath = "audio" + Path.GetExtension(AudioPath);
            }

            var chartFile = Path.Combine(newDirectory, "chart.orinoco");
            var chartData = new ChartData() { Info = newChartInfo };
            File.WriteAllText(chartFile, MainSerializer.Serialize(chartData));

            Program.Rhine.LoadChartData(newDirectory, chartData);
            Core.IsProjectOpen = true;
            Window.SetTitle($"OriNoco - {newChartInfo.Name}");

            ProjectInfoWindow.Enabled = true;
            ProjectsWindow.Enabled = false;
            Enabled = false;
        }

        public static void Show()
        {
            newChartInfo = new ChartInfoData();
            Enabled = true;
        }
    }
}
