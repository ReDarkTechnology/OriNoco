using System;
using UnityEngine;

namespace OriNoco.Timing
{
    [Serializable]
    public struct Fraction
    {
        public readonly float Numerator => numerator;
        public readonly float Denominator => denominator;
        public readonly float Value => GetValue();

        [SerializeField]
        private float numerator;
        [SerializeField]
        private float denominator;

        public Fraction(float numerator, float denominator)
        {
            if (denominator == 0)
            {
                numerator = 0;
                denominator = 1;
            }

            this.numerator = numerator;
            this.denominator = denominator;

            Simplify();
        }

        public Fraction(float value)
        {
            // Convert the float value to a fraction with denominator = 1
            numerator = value;
            denominator = 1;

            Simplify();
        }

        private void Simplify(float accuracy = 0.000001f)
        {
            SimplifyFirstStage(accuracy);
            if (numerator == 0) denominator = 1;
        }

        private void SimplifyFirstStage(float accuracy = 0.000001f)
        {
            float value = Value;
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");
            }

            int sign = Math.Sign(value);
            if (sign == -1)
                value = Math.Abs(value);

            // Accuracy is the maximum relative error; convert to absolute maxError
            float maxError = sign == 0 ? accuracy : value * accuracy;

            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                numerator = sign * n;
                denominator = 1;
            }

            if (1 - maxError < value)
            {
                numerator = sign * (n + 1);
                denominator = 1;
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
                    // Middle is our best fraction
                    numerator = (n * middle_d + middle_n) * sign;
                    denominator = middle_d;
                    return;
                }
            }
        }

        public void SetNumerator(float to)
        {
            if (to == 0)
            {
                numerator = 0;
                denominator = 1;
                return;
            }

            numerator = to;
            Simplify();
        }

        public void SetDenominator(float to)
        {
            if (to == 0)
            {
                numerator = 0;
                denominator = 1;
                return;
            }

            denominator = to;
            Simplify();
        }

        public override string ToString()
        {
            if (denominator == 1)
                return numerator.ToString();

            return $"{numerator}/{denominator}";
        }

        public readonly float GetValue()
        {
            if (denominator == 0 || numerator == 0) return 0;
            return numerator / denominator;
        }

        // Arithmetic Operations
        public static Fraction operator +(Fraction a, Fraction b)
        {
            float commonDenominator = a.denominator * b.denominator;
            float newNumerator = a.numerator * b.denominator + b.numerator * a.denominator;
            return new Fraction(newNumerator, commonDenominator);
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            float commonDenominator = a.denominator * b.denominator;
            float newNumerator = a.numerator * b.denominator - b.numerator * a.denominator;
            return new Fraction(newNumerator, commonDenominator);
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator * b.numerator, a.denominator * b.denominator);
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.numerator == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            return new Fraction(a.numerator * b.denominator, a.denominator * b.numerator);
        }
    }
}