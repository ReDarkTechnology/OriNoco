using ImGuiNET;
using OriNoco.Data;
using OriNoco.Serializer;
using Raylib_CSharp.Windowing;
using System.IO;
using System.Numerics;

namespace OriNoco.UI
{
    public static class NewProjectWindow
    {
        public static bool Enabled = false;
        public static ChartInfoData newChartInfo = new ChartInfoData();
        public static void Draw()
        {
            if (!Enabled) return;

            Vector2 size = new Vector2(300f, 100f);
            ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos((Game.WindowSizeF - size) / 2f, ImGuiCond.FirstUseEver);
            GUI.BeginWindow("New Project", ref Enabled);
            {
                newChartInfo.Name = GUI.InputText("Name", newChartInfo.Name, 256);
                if (GUI.Button("Create"))
                {
                    Program.Rhine.ResetScene();

                    var newDirectory = Path.Combine(Core.DataDirectory, "Projects", Core.SanitizeFileName(newChartInfo.Name));
                    if (Directory.Exists(newDirectory))
                    {
                        newDirectory = Path.Combine(Core.DataDirectory, "Projects", Path.GetRandomFileName());
                        Directory.CreateDirectory(newDirectory);
                    }
                    else
                    {
                        Directory.CreateDirectory(newDirectory);
                    }

                    var chartFile = Path.Combine(newDirectory, "chart.orinoco");
                    File.WriteAllText(chartFile, MainSerializer.Serialize(new ChartData() { Info = newChartInfo }));

                    Program.Rhine.LoadChartData(new ChartData() { Info = newChartInfo });
                    Core.DirectoryPath = newDirectory;
                    Core.IsProjectOpen = true;
                    Window.SetTitle($"OriNoco - {newChartInfo.Name}");

                    ProjectInfoWindow.Enabled = true;
                    ProjectsWindow.Enabled = false;
                    Enabled = false;
                }
            }
            GUI.EndWindow();
        }

        public static void Show()
        {
            newChartInfo = new ChartInfoData();
            Enabled = true;
        }
    }
}
