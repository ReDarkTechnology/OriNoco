using ImGuiNET;
using Raylib_CSharp.Audio;
using SharpFileDialog;
using System.IO;

namespace OriNoco.UI
{
    public static class MenuBar
    {
        public static void Draw()
        {
            if (!Core.IsProjectOpen) return;

            ImGui.BeginMainMenuBar();
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New")) NewProjectWindow.Show();

                    ImGui.Separator();

                    if (ImGui.MenuItem("Open")) ProjectsWindow.Show(true, true);
                    if (ImGui.MenuItem("Save")) Program.Rhine.Save();

                    ImGui.Separator();

                    if (ImGui.MenuItem("Change Song")) ChangeSong();

                    ImGui.Separator();

                    if (ImGui.MenuItem("Exit")) Game.StopRunning = true;

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Refresh All Notes"))
                        Program.Rhine.UpdateNotesFromIndex(0);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("FPS", string.Empty, ref Core.ShowFPS))
                    {
                        Settings.Data.ShowFPS = Core.ShowFPS;
                        Settings.Save();
                    }

                    if (ImGui.MenuItem("Note Count", string.Empty, ref Core.ShowNoteCount))
                    {
                        Settings.Data.ShowNoteCount = Core.ShowNoteCount;
                        Settings.Save();
                    }

                    if (ImGui.MenuItem("Time", string.Empty, ref Core.ShowTime))
                    {
                        Settings.Data.ShowTime = Core.ShowTime;
                        Settings.Save();
                    }

                    if (ImGui.MenuItem("Properties", string.Empty, ref Program.Rhine.showProperties)) ChangeProperties();

                    if (ImGui.MenuItem("Hit Sound", string.Empty, ref Core.PlayHitSound))
                    {
                        Settings.Data.PlayHitSound = Core.PlayHitSound;
                        Settings.Save();
                    }

                    if (ImGui.MenuItem("Project Info")) ProjectInfoWindow.Enabled = true;

                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();
        }

        private static void ChangeSong()
        {
            var filter = new NativeFileDialog.Filter() { Name = "Audio Files", Extensions = ["ogg", "mp3"] };
            var filter2 = new NativeFileDialog.Filter() { Name = "All Files", Extensions = ["*"] };
            if (NativeFileDialog.OpenDialog([filter, filter2], null, out string? path))
            {
                var audio = Music.Load(path);
                if (audio.IsReady())
                {
                    Program.Rhine.music = audio;

                    if (Core.DirectoryPath != null)
                    {
                        var adFile = Path.Combine(Core.DirectoryPath, "audio.ogg");
                        if (File.Exists(adFile)) File.Delete(adFile);
                        adFile = Path.Combine(Core.DirectoryPath, "audio.mp3");
                        if (File.Exists(adFile)) File.Delete(adFile);

                        File.Copy(path, Path.Combine(Core.DirectoryPath, "audio" + Path.GetExtension(path)));
                        Core.Info.AudioPath = "audio" + Path.GetExtension(path);
                    }
                }
                else
                {
                    MessageBox.Show("This audio file cannot be loaded! (Not Supported, I think)");
                }
            }
        }

        public static void ChangeProperties()
        {
            Settings.Data.ShowProperties = Program.Rhine.showProperties;
            Program.Rhine.UpdateViewport();
            Settings.Save();
        }
    }
}
