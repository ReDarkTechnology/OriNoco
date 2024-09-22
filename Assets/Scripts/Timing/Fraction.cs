using System;

namespace OriNoco.Timing
{
    public struct Fraction
    {
        public readonly float Numerator => numerator;
        public readonly float Denominator => denominator;
        public readonly float Value => GetValue();
        private float numerator;
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

        private void Simplify()
        {
            // Find the greatest common divisor (GCD) to simplify the fraction
            float gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
            numerator /= gcd;
            denominator /= gcd;

            // Adjust the sign to be on the numerator
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }
        }

        private float GCD(float a, float b)
        {
            float epsilon = 1e-3f; // Tolerance value, adjust as needed

            while (Math.Abs(b) > epsilon)
            {
                float temp = b;
                b = a % b;
                a = temp;
            }
            return a;
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