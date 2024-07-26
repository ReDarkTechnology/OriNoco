using System.IO;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;

using OriNoco.Data;
using OriNoco.Serializer;

using ImGuiNET;
using SharpFileDialog;
using Raylib_CSharp.Windowing;

namespace OriNoco.UI
{
    public static class ProjectsWindow
    {
        public static bool Enabled = true;
        public static bool ShowCloseButton = false;
        public static List<ProjectInstance> projectInstances = new List<ProjectInstance>();

        public static void Draw()
        {
            if (!Enabled) return;

            Vector2 defaultSize = Game.WindowSizeF - new Vector2(20f, 20f);
            ImGui.SetNextWindowSize(defaultSize, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos((Game.WindowSizeF - defaultSize) / 2f, ImGuiCond.FirstUseEver);

            if (ShowCloseButton)
                GUI.BeginWindow("Projects", ref Enabled);
            else
                GUI.BeginWindow("Projects");
            {
                if (GUI.Button("New")) NewProjectWindow.Show();
                ImGui.SameLine();
                if (GUI.Button("Browse"))
                {
                    var filter1 = new NativeFileDialog.Filter() { Name = "OriNoco Projects", Extensions = ["orinoco"] };
                    var filter2 = new NativeFileDialog.Filter() { Name = "All Files", Extensions = ["*"] };
                    if (NativeFileDialog.OpenDialog([filter1, filter2], null, out string? path))
                    {
                        string content = File.ReadAllText(path);
                        ChartData? data = MainSerializer.Deserialize<ChartData>(content);
                        if (data != null)
                        {
                            var directory = Path.GetDirectoryName(path);
                            if (Directory.Exists(directory))
                            {
                                var newDirectory = Path.Combine(Core.DataDirectory, "Projects", Path.GetFileName(directory));

                                if (Directory.Exists(newDirectory))
                                {
                                    newDirectory = Path.Combine(Core.DataDirectory, "Projects", Path.GetRandomFileName());
                                    Core.CopyDirectory(directory, newDirectory, true);
                                }

                                Core.CopyDirectory(directory, newDirectory, true);
                                Program.Rhine.LoadChartData(data);
                                Core.DirectoryPath = newDirectory;
                                Core.IsProjectOpen = true;
                                Enabled = false;
                            }
                            else
                            {
                                MessageBox.Show("PANIC-727: Chart file can be read but no directory is available to copy");
                            }
                        }
                    }
                }

                GUI.SameLine();
                if (GUI.Button("Refresh")) RefreshProjectPanel();

                int i = 0;
                ProjectInstance? askDelete = null;
                foreach (var instance in projectInstances)
                {
                    GUI.Separator();
                    ImGui.PushID("projects-" + i);
                    ImGui.Text(instance.data.Info.Name);
                    if (ImGui.Button("Open"))
                    {
                        var chartFile = Path.Combine(instance.directory, "chart.orinoco");
                        if (File.Exists(chartFile))
                        {
                            string content = File.ReadAllText(chartFile);
                            ChartData? data = MainSerializer.Deserialize<ChartData>(content);
                            if (data != null)
                            {
                                Program.Rhine.LoadChartData(data);
                                Core.DirectoryPath = instance.directory;
                                Core.IsProjectOpen = true;
                                Window.SetTitle($"OriNoco - {data.Info.Name}");

                                Enabled = false;
                                ProjectInfoWindow.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("PANIC-621: Chart file was available but unable to read when being opened");
                            }
                        }
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Open Folder"))
                    {
                        if (Directory.Exists(instance.directory))
                            Process.Start("explorer.exe", instance.directory);
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Delete"))
                    {
                        if (MessageBox.Show("Are you sure you want to delete this project?", "Confimation", MessageBoxType.YesNo) == Result.Yes)
                        {
                            Directory.Delete(instance.directory, true);
                            askDelete = instance;
                        }
                    }
                    ImGui.PopID();

                    i++;
                }

                if (askDelete != null)
                {
                    projectInstances.Remove(askDelete);
                    askDelete = null;
                }
            }
            GUI.EndWindow();
        }

        public static void Show(bool refresh = true, bool showCloseButton = false)
        {
            if (refresh)
                RefreshProjectPanel();

            ShowCloseButton = showCloseButton;
            Enabled = true;
        }

        public static void RefreshProjectPanel()
        {
            var projectsDirectory = Path.Combine(Core.DataDirectory, "Projects");
            if (Directory.Exists(projectsDirectory))
            {
                projectInstances.Clear();
                var directories = Directory.GetDirectories(projectsDirectory);
                foreach (var directory in directories)
                {
                    var chartFile = Path.Combine(directory, "chart.orinoco");
                    if (File.Exists(chartFile))
                    {
                        string content = File.ReadAllText(chartFile);
                        ChartData? data = MainSerializer.Deserialize<ChartData>(content);

                        if (data != null)
                            projectInstances.Add(new ProjectInstance(directory, data));
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(projectsDirectory);
                if (projectInstances.Count > 0)
                    MessageBox.Show("You don't have any projects anymore... seems like you deleted the folder while the program is running.");
                projectInstances.Clear();
            }
        }
    }

    public class ProjectInstance
    {
        public string directory;
        public ChartData data;

        public ProjectInstance(string directory, ChartData data)
        {
            this.directory = directory;
            this.data = data;
        }
    }
}
