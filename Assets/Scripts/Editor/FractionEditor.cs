using UnityEngine;
using UnityEditor;
using System;

namespace OriNoco.Timing
{
    [CustomPropertyDrawer(typeof(Fraction))]
    public class FractionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get the numerator and denominator fields
            var numeratorProp = property.FindPropertyRelative("numerator");
            var denominatorProp = property.FindPropertyRelative("denominator");

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate rects for fields
            var numeratorRect = new Rect(position.x, position.y, position.width / 2 - 2, position.height);
            var denominatorRect = new Rect(position.x + position.width / 2 + 2, position.y, position.width / 2 - 2, position.height);

            // Draw numerator and denominator fields
            EditorGUI.PropertyField(numeratorRect, numeratorProp, GUIContent.none);
            EditorGUI.PropertyField(denominatorRect, denominatorProp, GUIContent.none);

            // Apply changes and call Simplify if either field changes
            if (numeratorProp.floatValue != 0 || denominatorProp.floatValue != 0)
            {
                // Ensure the denominator isn't zero
                if (denominatorProp.floatValue == 0)
                {
                    denominatorProp.floatValue = 1;
                }

                // Call Simplify when either value is changed
                Simplify(property);
            }

            EditorGUI.EndProperty();
        }

        private void Simplify(SerializedProperty property)
        {
            // Get numerator and denominator properties
            var numeratorProp = property.FindPropertyRelative("numerator");
            var denominatorProp = property.FindPropertyRelative("denominator");

            // Simplification logic (example: find GCD and divide)
            Vector2 frac = GCD(numeratorProp.floatValue / denominatorProp.floatValue, 0.000001f);
            numeratorProp.floatValue = frac.x;
            denominatorProp.floatValue = frac.y;
        }

        // Example GCD function
        private Vector2 GCD(float value, double accuracy)
        {
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");
            }

            int sign = Math.Sign(value);
            if (sign == -1)
                value = Math.Abs(value);

            // Accuracy is the maximum relative error; convert to absolute maxError
            double maxError = sign == 0 ? accuracy : value * accuracy;

            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return new Vector2(sign * n, 1);
            }

            if (1 - maxError < value)
            {
                return new Vector2(sign * (n + 1), 1);
            }

            // The lower fraction is 0/1
            int lower_n = 0;
            int lower_d = 1;

            // The upper fraction is 1/1
            int upper_n = 1;
            int upper_d = 1;

            while (true)
            {
                // The middle fraction is (lower_n + upper_n) / (lower_d + upper_d)
                int middle_n = lower_n + upper_n;
                int middle_d = lower_d + upper_d;

                if (middle_d * (value + maxError) < middle_n)
                {
                    // real + error < middle : middle is our new upper
                    upper_n = middle_n;
                    upper_d = middle_d;
                }
                else if (middle_n < (value - maxError) * middle_d)
                {
                    // middle < real - error : middle is our new lower
                    lower_n = middle_n;
                    lower_d = middle_d;
                }
                else
                {
                    return new Vector2((n * middle_d + middle_n) * sign, middle_d);
                }
            }
        }
    }
}