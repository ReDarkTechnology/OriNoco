using System;
using UnityEngine;

namespace OriNoco.Timing
{
    /// <summary>
    /// Represents a point in time within a musical composition, defined by a beat value and a fractional component.
    /// </summary>
    /// <remarks>
    /// This struct provides methods for manipulating and converting beat times, including adjusting the beat and fraction,
    /// getting the whole value, signature, and more.
    /// </remarks>
    // ReSharper disable CompareOfFloatsByEqualityOperator
    [Serializable]
    public struct BeatTime
    {
        public float beat;
        public Fraction fraction;

        public float numerator
        {
            get => fraction.Numerator;
            set => fraction.SetNumerator(value);
        }

        public float denominator
        {
            get => fraction.Denominator;
            set => fraction.SetDenominator(value);
        }

        public float whole => GetWholeValue();
        public float signature => GetSignature();

        public BeatTime(float whole)
        {
            beat = (int)whole;
            var fractionalPart = whole - beat;
            fraction = new Fraction(fractionalPart, 1);
        }

        public BeatTime(float beat = 0, float numerator = 0, float denominator = 1)
        {
            this.beat = beat;
            fraction = new Fraction(numerator, denominator, false);
        }

        public BeatTime(float beat, float fraction)
        {
            var f = (float)beat + fraction;

            this.beat = (int)f;
            var fractionalPart = f - beat;
            this.fraction = new Fraction(fractionalPart, 1);
        }

        public BeatTime(string str)
        {
            var split = str.Split(':');
            var split2 = split[1].Split('/');

            float.TryParse(split[0], out beat);
            if (float.TryParse(split2[0], out var num))
                fraction = float.TryParse(split2[1], out var den) ? 
                    new Fraction(num, den) : 
                    new Fraction(num, 1);
            
            fraction = new Fraction(0, 1);
        }

        /// <summary>
        /// Adjust the beat and fraction to ensure that the numerator is within [0, denominator).
        /// </summary>
        /// <remarks>
        /// This is done by subtracting whole beats from the numerator and adding or subtracting that many beats from the whole beat value.
        /// </remarks>
        public void AdjustBeatAndFraction()
        {
            if (numerator != 0)
            {
                beat += (int)(numerator / denominator);
                fraction.SetNumerator(numerator % denominator);
        
                if (numerator < 0)
                {
                    beat -= 1;
                    fraction.SetNumerator(numerator + denominator);
                }
            }
        }

        public readonly float GetSignature() => fraction.Value;
        public readonly float GetWholeValue() => beat + fraction.Value;
        public readonly float ToSeconds(float bpm) => (beat + GetSignature()) * (120f / bpm);
        public readonly float ToSpeed(float speed) => (beat + GetSignature()) * speed;

        public static BeatTime FromNumber(float value, float bpm) =>
            new BeatTime(value / (120f / bpm));

        public static void GetFraction(float value, ref BeatTime time)
        {
            time.beat = (int)value;
            var fractionalPart = value - time.beat;
            time.fraction = new Fraction(fractionalPart);
        }

        public override string ToString() => $"{beat}:{fraction.Numerator}/{fraction.Denominator}";
        public static BeatTime Parse(string str)
        {
            var split = str.Split(':');
            var split2 = split[1].Split('/');

            var bt = new BeatTime();
            float.TryParse(split[0], out bt.beat);
            float.TryParse(split2[0], out var n);
            float.TryParse(split2[1], out var d);
            bt.fraction = new Fraction(n, d);
            return bt;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                null => false,
                BeatTime time => time.GetWholeValue() == GetWholeValue(),
                _ => false
            };
        }

        public static BeatTime operator -(BeatTime lhs, BeatTime rhs) =>
            new(lhs.beat - rhs.beat, lhs.signature - rhs.signature);
        public static BeatTime operator +(BeatTime lhs, BeatTime rhs) =>
            new(lhs.beat + rhs.beat, lhs.signature + rhs.signature);
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

    }
}