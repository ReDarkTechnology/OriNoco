using Raylib_CSharp.Colors;

namespace OriNoco
{
    public struct ColorF
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public ColorF(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            A = 1f;
        }

        public ColorF(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public float sqrMagnitude => R * R + G * G + B * B + A * A;

        public static ColorF operator +(ColorF a, ColorF b) => new ColorF(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
        public static ColorF operator -(ColorF a, ColorF b) => new ColorF(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
        public static ColorF operator *(ColorF a, ColorF b) => new ColorF(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
        public static ColorF operator /(ColorF a, ColorF b) => new ColorF(a.R / b.R, a.G / b.G, a.B / b.B, a.A / b.A);
        public static ColorF operator -(ColorF a) => new ColorF(-a.R, -a.G, -a.B, -a.A);
        public static ColorF operator *(ColorF a, float d) => new ColorF(a.R * d, a.G * d, a.B * d, a.A * d);
        public static ColorF operator *(float d, ColorF a) => new ColorF(a.R * d, a.G * d, a.B * d, a.A * d);
        public static ColorF operator /(ColorF a, float d) => new ColorF(a.R / d, a.G / d, a.B / d, a.A / d);
        public static bool operator ==(ColorF lhs, ColorF rhs) => (lhs - rhs).sqrMagnitude < 9.99999944E-11f;
        public static bool operator !=(ColorF lhs, ColorF rhs) => !(lhs == rhs);

        public override int GetHashCode() => R.GetHashCode() ^ G.GetHashCode() << 2 ^ B.GetHashCode() >> 2 ^ A.GetHashCode() >> 1;
        public override bool Equals(object obj) => obj is ColorF && Equals((ColorF)obj);
        public bool Equals(ColorF other) => R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);

        public static ColorF Lerp(ColorF a, ColorF b, float t) => LerpUnclamped(a, b, t.Clamp(0f, 1f));
        public static ColorF LerpUnclamped(ColorF a, ColorF b, float t) => new ColorF(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);

        public static implicit operator ColorF(Color v) => new ColorF(ByteToFloat(v.R), ByteToFloat(v.G), ByteToFloat(v.B), ByteToFloat(v.A));
        public static implicit operator Color(ColorF v) => new Color(FloatToByte(v.R), FloatToByte(v.G), FloatToByte(v.B), FloatToByte(v.A));

        public static float ByteToFloat(float byteValue) => byteValue / 255;
        public static byte FloatToByte(float floatValue) => (byte)(floatValue * 255);
    }
}
