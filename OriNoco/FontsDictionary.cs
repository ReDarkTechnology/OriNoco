using ImGuiNET;
using Raylib_CSharp.Fonts;
using rlImGui_cs;
using System.Drawing;

namespace OriNoco
{
    public static class FontsDictionary
    {
        public static Font CaviarDreams { get; private set; }
        public static Font GeoSansLight { get; private set; }
        public static Font Kiona { get; private set; }

        public static void Init(int size = 20)
        {
            CaviarDreams = Font.LoadEx("Fonts/CaviarDreams.ttf", size, null);
            GeoSansLight = Font.LoadEx("Fonts/GeoSansLight.ttf", size, null);
            Kiona = Font.LoadEx("Fonts/Kiona.ttf", size, null);
        }

        public static void InitImGui()
        {
            ImGui.GetIO().Fonts.Clear(); 
            ImGui.GetIO().Fonts.AddFontFromFileTTF("Fonts/GeoSansLight.ttf", 20);
            rlImGui.ReloadFonts();
        }
    }
}
