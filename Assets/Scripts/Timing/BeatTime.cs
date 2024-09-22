using System;
using UnityEngine;

namespace OriNoco.Timing
{
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
            float fractionalPart = whole - beat;
            fraction = new Fraction(fractionalPart, 1);
        }

        public BeatTime(float beat = 0, int numerator = 0, int denominator = 1)
        {
            float whole = beat + Divide(numerator, denominator);

            this.beat = (int)whole;
            float fractionalPart = whole - beat;
            fraction = new Fraction(fractionalPart, 1);
        }

        public BeatTime(float beat, float fraction)
        {
            float whole = (float)beat + fraction;

            this.beat = (int)whole;
            float fractionalPart = whole - beat;
            this.fraction = new Fraction(fractionalPart, 1);
        }

        public BeatTime(string str)
        {
            string[] split = str.Split(':');
            string[] split2 = split[1].Split('/');

            float.TryParse(split[0], out beat);
            if (float.TryParse(split2[0], out float numerator))
            {
                if (float.TryParse(split2[1], out float denominator))
                    fraction = new Fraction(numerator, denominator);
                else
                    fraction = new Fraction(numerator, 1);
            }
            fraction = new Fraction(0, 1);
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
            float fractionalPart = value - time.beat;
            time.fraction = new Fraction(fractionalPart);
        }

        public override string ToString() => $"{beat}:{fraction.Numerator}/{fraction.Denominator}";
        public static BeatTime Parse(string str)
        {
            string[] split = str.Split(':');
            string[] split2 = split[1].Split('/');

            var bt = new BeatTime();
            float.TryParse(split[0], out bt.beat);
            float.TryParse(split2[0], out float n);
            float.TryParse(split2[1], out float d);
            bt.fraction = new Fraction(n, d);
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