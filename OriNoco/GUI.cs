using ImGuiNET;
using rlImGui_cs;
using System;
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
        public static void BeginWindow(string name, ref bool open) => ImGui.Begin(name, ref open);
        public static bool BeginWindow(string name, ImGuiWindowFlags flags) => ImGui.Begin(name, flags);
        public static void EndWindow()
        {
            if (!_IsOverAnyElement)
                _IsOverAnyElement = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows) || ImGui.IsMouseHoveringRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize(), true);
            ImGui.End();
        }

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

        public static void BeginChild(string name) => ImGui.BeginChild(name);
        public static void BeginChild(string name, Vector2 size) => ImGui.BeginChild(name, size);
        public static void EndChild()
        {
            if (!_IsOverAnyElement)
                _IsOverAnyElement = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows) || ImGui.IsMouseHoveringRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize(), true);
            ImGui.EndChild();
        }

        public static void SetNextWindowPos(Vector2 pos) => ImGui.SetNextWindowPos(pos);
        public static void SetNextWindowSize(Vector2 size) => ImGui.SetNextWindowSize(size);
        public static void SetNextWindowBgAlpha(float alpha) => ImGui.SetNextWindowBgAlpha(alpha);

        public static void BeginGroup() => ImGui.BeginGroup();
        public static void EndGroup() => ImGui.EndGroup();

        public static void PushID(string id) => ImGui.PushID(id);
        public static void PopID() => ImGui.PopID();

        public static void TextUnformatted(string text) => ImGui.TextUnformatted(text);
        public static Enum ComboBox(string label, Enum value)
        {
            ImGui.BeginCombo(label, value.ToString("G"));
            foreach (var enums in Enum.GetValues(value.GetType()))
            {
                bool s = ImGui.Selectable(((Enum)enums).ToString("G"), enums == value);
                if (s)
                    value = (Enum)enums;
            }
            ImGui.EndCombo();
            return value;
        }

        public static float Slider(string label, float value, float min, float max)
        {
            ImGui.SliderFloat(label, ref value, min, max);
            return value;
        }

        /// <summary>
        /// ComboBox for an Enum, doesn't support Flags
        /// </summary>
        public static T ComboBox<T>(string label, T value) where T : Enum
        {
            if (ImGui.BeginCombo(label, value.ToString("G")))
            {
                foreach (var enums in Enum.GetValues(value.GetType()))
                {
                    bool s = ImGui.Selectable(((Enum)enums).ToString("G"), enums.Equals(value));
                    if (s)
                        value = (T)enums;
                }
                ImGui.EndCombo();
            }
            return value;
        }

        public static void StartUpdate() => _IsOverAnyElement = false;
        public static void EndUpdate() => IsOverAnyElement = _IsOverAnyElement;

        public static bool IsOverAnyElement { get; private set; }
        private static bool _IsOverAnyElement = false;
    }
}
