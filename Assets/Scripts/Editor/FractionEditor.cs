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

            // Calculate rects for fields and button
            var fieldWidth = position.width / 2 - 4;
            var buttonWidth = 60; // Width for the "Simplify" button

            var numeratorRect = new Rect(position.x, position.y, fieldWidth, position.height);
            var denominatorRect = new Rect(position.x + fieldWidth + 4, position.y, fieldWidth, position.height);
            var buttonRect = new Rect(position.x + 2 * fieldWidth + 8, position.y, buttonWidth, position.height);

            // Draw numerator and denominator fields
            EditorGUI.PropertyField(numeratorRect, numeratorProp, GUIContent.none);
            EditorGUI.PropertyField(denominatorRect, denominatorProp, GUIContent.none);

            // Draw the "Simplify" button
            if (GUI.Button(buttonRect, "Simplify"))
            {
                Simplify(property); // Call the Simplify method when clicked
            }

            // Ensure the denominator isn't zero
            if (denominatorProp.floatValue == 0)
            {
                denominatorProp.floatValue = 1;
            }

            EditorGUI.EndProperty();
        }

        private void Simplify(SerializedProperty property)
        {
            // Get numerator and denominator properties
            var numeratorProp = property.FindPropertyRelative("numerator");
            var denominatorProp = property.FindPropertyRelative("denominator");

            // Simplification logic (find GCD and divide)
            var fraction = FractionSimplify(numeratorProp.floatValue / denominatorProp.floatValue, 0.000001f);
            numeratorProp.floatValue = fraction.x;
            denominatorProp.floatValue = fraction.y;
        }

        private static Vector2 FractionSimplify(float value, double accuracy)
        {
            if (accuracy is <= 0.0 or >= 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(accuracy), "Must be > 0 and < 1.");
            }

            var sign = Math.Sign(value);
            if (sign == -1)
                value = Math.Abs(value);

            var maxError = sign == 0 ? accuracy : value * accuracy;
            var n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return new Vector2(sign * n, 1);
            }

            if (1 - maxError < value)
            {
                return new Vector2(sign * (n + 1), 1);
            }

            var lowerN = 0;
            var lowerD = 1;
            var upperN = 1;
            var upperD = 1;

            while (true)
            {
                var middleN = lowerN + upperN;
                var middleD = lowerD + upperD;

                if (middleD * (value + maxError) < middleN)
                {
                    upperN = middleN;
                    upperD = middleD;
                }
                else if (middleN < (value - maxError) * middleD)
                {
                    lowerN = middleN;
                    lowerD = middleD;
                }
                else
                {
                    return new Vector2((n * middleD + middleN) * sign, middleD);
                }
            }
        }
    }
}
