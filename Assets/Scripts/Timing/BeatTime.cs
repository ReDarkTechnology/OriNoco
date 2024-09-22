using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OriNoco.Timing
{
    [Serializable]
    public struct BeatTime
    {
        public float beat;
        public float numerator;
        public float denominator;

        public float whole => GetWholeValue();
        public float signature => GetSignature();

        public BeatTime(float whole)
        {
            beat = (int)whole;
            float fractionalPart = whole - beat;
            numerator = fractionalPart;
            denominator = 1;
        }

        public BeatTime(float beat = 0, int numerator = 0, int denominator = 1)
        {
            float whole = beat + Divide(numerator, denominator);

            this.beat = (int)whole;
            float fractionalPart = whole - beat;
            this.numerator = fractionalPart;
            this.denominator = 1;
        }

        public BeatTime(float beat, float fraction)
        {
            float whole = (float)beat + fraction;

            this.beat = (int)whole;
            float fractionalPart = whole - beat;
            numerator = fractionalPart;
            denominator = 1;
        }

        public BeatTime(string str)
        {
            string[] split = str.Split(':');
            string[] split2 = split[1].Split('/');

            float.TryParse(split[0], out beat);
            float.TryParse(split2[0], out numerator);
            float.TryParse(split2[1], out denominator);
        }

        public float GetSignature() =>
            Divide(numerator, denominator);
        public float GetWholeValue() =>
            beat + GetSignature();
        public float ToSeconds(float bpm) =>
            (beat + GetSignature()) * (120f / bpm);
        public float ToSpeed(float speed) =>
            (beat + GetSignature()) * speed;

        public static BeatTime FromNumber(float value, float bpm) =>
            new BeatTime(value / (120f / bpm));

        public static void GetFraction(float value, ref BeatTime time)
        {
            time.beat = (int)value;
            float fractionalPart = value - time.beat;
            time.numerator = fractionalPart;
            time.denominator = 1;
        }

        public override string ToString() => $"{beat}:{numerator}/{denominator}";
        public static BeatTime Parse(string str)
        {
            string[] split = str.Split(':');
            string[] split2 = split[1].Split('/');

            var bt = new BeatTime();
            float.TryParse(split[0], out bt.beat);
            float.TryParse(split2[0], out float n);
            float.TryParse(split2[1], out float d);
            bt.numerator = n;
            bt.denominator = d;
            return bt;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is BeatTime)
            {
                var y = (BeatTime)obj;
                return y.GetWholeValue() == GetWholeValue();
            }
            return false;
        }

        public static BeatTime operator -(BeatTime lhs, BeatTime rhs) =>
            new BeatTime(lhs.beat - rhs.beat, lhs.signature - rhs.signature);
        public static BeatTime operator +(BeatTime lhs, BeatTime rhs) =>
            new BeatTime(lhs.beat + rhs.beat, lhs.signature + rhs.signature);
        public static bool operator !=(BeatTime lhs, BeatTime rhs) => lhs.GetWholeValue() != rhs.GetWholeValue();
        public static bool operator ==(BeatTime lhs, BeatTime rhs) => lhs.GetWholeValue() == rhs.GetWholeValue();
        public static bool operator <(BeatTime lhs, BeatTime rhs) => lhs.GetWholeValue() < rhs.GetWholeValue();
        public static bool operator >(BeatTime lhs, BeatTime rhs) => lhs.GetWholeValue() > rhs.GetWholeValue();
        public static bool operator <=(BeatTime lhs, BeatTime rhs) => lhs.GetWholeValue() <= rhs.GetWholeValue();
        public static bool operator >=(BeatTime lhs, BeatTime rhs) => lhs.GetWholeValue() >= rhs.GetWholeValue();
        public override int GetHashCode() => GetWholeValue().GetHashCode();

        public static float Divide(float numerator, float denominator)
        {
            if (numerator == 0 || denominator == 0) return 0;
            return numerator / denominator;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(BeatTime))]
        public class BeatTimeDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Find the SerializedProperties by name
                var beatProp = property.FindPropertyRelative(nameof(beat));
                var fracN = property.FindPropertyRelative(nameof(numerator));
                var fracD = property.FindPropertyRelative(nameof(denominator));

                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Calculate rects
                var labelRect = new Rect(position.x, position.y, 10, position.height);
                var width = (position.width / 3) - (labelRect.width * 2) + 13;
                var rect = new Rect(position.x, position.y, width, position.height);

                // Draw fields - pass GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(rect, beatProp, GUIContent.none);
                rect.x += width;
                labelRect.x += width;
                EditorGUI.LabelField(labelRect, ":");
                rect.x += labelRect.width;
                EditorGUI.PropertyField(rect, fracN, GUIContent.none);
                rect.x += width;
                labelRect.x += width + labelRect.width;
                EditorGUI.LabelField(labelRect, "/");
                rect.x += labelRect.width;
                EditorGUI.PropertyField(rect, fracD, GUIContent.none);

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }
#endif
    }

    public static class ConverterExtension
    {
        public static float ToFloat(this int v) => (float)v;
        public static int ToInt(this float v) => (int)v;
    }
}