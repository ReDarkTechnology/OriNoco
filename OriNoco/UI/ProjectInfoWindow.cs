using System.Numerics;
using ImGuiNET;

namespace OriNoco.UI
{
    public static class ProjectInfoWindow
    {
        public static bool Enabled = false;

        public static void Draw()
        {
            if (!Enabled) return;

            Vector2 size = new(640f, 360f);
            ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos((Game.WindowSizeF - size) / 2f, ImGuiCond.FirstUseEver);
            GUI.BeginWindow("Project Info", ref Enabled);
            {
                Core.Info.Name = GUI.InputText("Chart Codename", Core.Info.Name, 256);
                Core.Info.DisplayName = GUI.InputText("Display Name", Core.Info.DisplayName, 256);

                GUI.Separator();

                Core.Info.AudioName = GUI.InputText("Audio Name", Core.Info.AudioName, 256);
                Core.Info.AudioComposer = GUI.InputText("Audio Composer", Core.Info.AudioComposer, 256);
                Core.Info.AudioOffset = GUI.InputFloat("Audio Offset", Core.Info.AudioOffset);

                GUI.Separator();

                Core.Info.LevelDifficulty = GUI.InputFloat("Level Difficulty", Core.Info.LevelDifficulty);
                Core.Info.LevelSet = GUI.ComboBox("Level Set", Core.Info.LevelSet);

                GUI.Separator();

                Core.Info.BackgroundColor = GUI.ColorEdit4("Background Color", Core.Info.BackgroundColor);
                Core.Info.LineColor = GUI.ColorEdit4("Line Color", Core.Info.LineColor);
                Core.Info.ParticleColor = GUI.ColorEdit4("Particle Color", Core.Info.ParticleColor);
                Core.Info.GradientColor = GUI.ColorEdit4("Gradient Color", Core.Info.GradientColor);
                Core.Info.TextColor = GUI.ColorEdit4("Text Color", Core.Info.TextColor);
                Core.Info.FirefliesColor = GUI.ColorEdit4("Fireflies Color", Core.Info.FirefliesColor);

                GUI.Separator();

                Core.Info.IsLockLevel = GUI.Checkbox("Is Lock Level", Core.Info.IsLockLevel);
                Core.Info.LockMode = GUI.ComboBox("Lock Mode", Core.Info.LockMode);
                Core.Info.RequiredLevelName = GUI.InputText("Required Level Name", Core.Info.RequiredLevelName, 256);
                Core.Info.RequiredAmount = GUI.InputInt("Required Amount", Core.Info.RequiredAmount);

                GUI.Separator();

                GUI.Text($"Directory Path: {Core.DirectoryPath}");
                GUI.Text($"File Path: {Core.FilePath}");
            }
            GUI.EndWindow();
        }
    }
}
