using ImGuiNET;
using rlImGui_cs;

using System.Numerics;

namespace OriNoco
{
    public static class GUI
    {
        public static void Begin() => rlImGui.Begin();
        public static void End() => rlImGui.End();

        public static bool Toggle(string label, bool value) => Checkbox(label, value);
        public static bool Checkbox(string label, bool value)
        {
            ImGui.Checkbox(label, ref value);
            return value;
        }

        public static bool BeginWindow(string name) => ImGui.Begin(name);
        public static bool BeginWindow(string name, ImGuiWindowFlags flags) => ImGui.Begin(name, flags);
        public static void EndWindow() => ImGui.End();

        public static void Text(string text) => ImGui.Text(text);
        public static void TextColored(Vector4 color, string text) => ImGui.TextColored(color, text);

        public static void Separator() => ImGui.Separator();
        public static bool Button(string text) => ImGui.Button(text);
        public static bool RepeatButton(string text, bool value)
        {
            ImGui.PushButtonRepeat(value);
            bool result = ImGui.Button(text);
            ImGui.PopButtonRepeat();
            return result;
        }

        /// <summary>
        /// Imports a font from a TTF file
        /// </summary>
        public static GUIFont ImportFont(string fileName, int size = 20) => new GUIFont(ImGui.GetIO().Fonts.AddFontFromFileTTF(fileName, size));
        /// <summary>
        /// Applies the font
        /// </summary>
        public static void ApplyFont(GUIFont font) => ImGui.PushFont(font.GetPointer());
        /// <summary>
        /// Gets the current font
        /// </summary>
        public static GUIFont GetFont() => new GUIFont(ImGui.GetFont());
    }

    /// <summary>
    /// Simple wrapper for ImFontPtr just in case I moved to another library
    /// </summary>
    public class GUIFont
    {
        private ImFontPtr fontPointer;
        public GUIFont(ImFontPtr pointer) => this.fontPointer = pointer;
        public ImFontPtr GetPointer() => fontPointer;
    }
}
