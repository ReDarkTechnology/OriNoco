using ImGuiNET;

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
                    if (ImGui.MenuItem("Open")) ProjectsWindow.Show(true, true);
                    if (ImGui.MenuItem("Save")) Program.Rhine.Save();

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

                    if (ImGui.MenuItem("Properties", string.Empty, ref Program.Rhine.showProperties))
                    {
                        Settings.Data.ShowProperties = Program.Rhine.showProperties;
                        if (Program.Rhine.showProperties)
                        {
                            Program.Rhine.viewportOffset = new(0, 20);
                            Program.Rhine.viewportScaleOffset = new(300, 320);
                        }
                        else
                        {
                            Program.Rhine.viewportOffset = new(0, 20);
                            Program.Rhine.viewportScaleOffset = new(300, 20);
                        }
                        Settings.Save();
                    }

                    if (ImGui.MenuItem("Project Info")) ProjectInfoWindow.Enabled = true;

                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();
        }
    }
}
