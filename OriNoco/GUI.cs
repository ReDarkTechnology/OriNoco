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
        public static bool IsEditing() => ImGui.IsAnyItemActive();

        public static bool Toggle(string label, bool value) => Checkbox(label, value);
        public static bool Checkbox(string label, bool value)
        {
            ImGui.Checkbox(label, ref value);
            return value;
        }

        public static bool BeginWindow(string name) => ImGui.Begin(name);
        public static void BeginWindow(string name, ref bool open) => ImGui.Begin(name, ref open);
        public static void BeginWindow(string name, ref bool open, ImGuiWindowFlags flags) => ImGui.Begin(name, ref open, flags);
        public static bool BeginWindow(string name, ImGuiWindowFlags flags) => ImGui.Begin(name, flags);
        public static void EndWindow()
        {
            if (!_IsOverAnyElement)
                _IsOverAnyElement = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows) || ImGui.IsMouseHoveringRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize(), true);
            ImGui.End();
        }

        public static void SameLine() => ImGui.SameLine();

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

        public static int Slider(string label, int value, int min, int max)
        {
            ImGui.SliderInt(label, ref value, min, max);
            return value;
        }

        public static bool Slider(string label, float value, float min, float max, out float result)
        {
            bool changed = ImGui.SliderFloat(label, ref value, min, max);
            result = value;
            return changed;
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

        public static bool InputText(string label, string? value, uint maxLength, out string result)
        {
            bool changed = ImGui.InputText(label, ref value, maxLength);
            result = value;
            return changed;
        }

        public static string InputText(string label, string? value, uint maxLength)
        {
            ImGui.InputText(label, ref value, maxLength);
            return value;
        }

        public static bool InputFloat(string label, float value, out float result)
        {
            bool changed = ImGui.InputFloat(label, ref value);
            result = value;
            return changed;
        }

        public static bool InputFloat(string label, float value, float step, out float result)
        {
            bool changed = ImGui.InputFloat(label, ref value, step);
            result = value;
            return changed;
        }

        public static float InputFloat(string label, float value)
        {
            ImGui.InputFloat(label, ref value);
            return value;
        }

        public static float InputFloat(string label, float value, float step)
        {
            ImGui.InputFloat(label, ref value, step);
            return value;
        }

        public static bool InputInt(string label, int value, out float result)
        {
            bool changed = ImGui.InputInt(label, ref value);
            result = value;
            return changed;
        }

        public static int InputInt(string label, int value)
        {
            ImGui.InputInt(label, ref value);
            return value;
        }

        public static bool ColorButton(string label, ColorF value) =>
            ImGui.ColorButton(label, new Vector4(value.R, value.G, value.B, value.A));

        public static bool ColorPicker4(string label, ColorF value, out ColorF result)
        {
            Vector4 vecValue = new Vector4(value.R, value.G, value.B, value.A);
            if (ImGui.ColorPicker4(label, ref vecValue))
            {
                result = new ColorF(vecValue.X, vecValue.Y, vecValue.Z, vecValue.W);
                return true;
            }
            result = value;
            return false;
        }

        public static ColorF ColorPicker4(string label, ColorF value)
        {
            Vector4 vecValue = new Vector4(value.R, value.G, value.B, value.A);
            if (ImGui.ColorPicker4(label, ref vecValue))
                value = new ColorF(vecValue.X, vecValue.Y, vecValue.Z, vecValue.W);
            return value;
        }

        public static bool ColorEdit4(string label, ColorF value, out ColorF result)
        {
            Vector4 vecValue = new Vector4(value.R, value.G, value.B, value.A);
            if (ImGui.ColorEdit4(label, ref vecValue))
            {
                result = new ColorF(vecValue.X, vecValue.Y, vecValue.Z, vecValue.W);
                return true;
            }
            result = value;
            return false;
        }

        public static ColorF ColorEdit4(string label, ColorF value)
        {
            Vector4 vecValue = new Vector4(value.R, value.G, value.B, value.A);
            if (ImGui.ColorEdit4(label, ref vecValue))
                value = new ColorF(vecValue.X, vecValue.Y, vecValue.Z, vecValue.W);
            return value;
        }

        public static void StartUpdate() => _IsOverAnyElement = false;
        public static void EndUpdate() => IsOverAnyElement = _IsOverAnyElement;

        public static bool IsOverAnyElement { get; private set; }
        private static bool _IsOverAnyElement = false;
    }
}
