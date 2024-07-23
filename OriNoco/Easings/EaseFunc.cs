using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_CSharp;

namespace OriNoco.Tweening
{
    public enum Ease
    {
        Unset = 0,
        Linear = 1,
        InSine = 2,
        OutSine = 3,
        InOutSine = 4,
        InQuad = 5,
        OutQuad = 6,
        InOutQuad = 7,
        InCubic = 8,
        OutCubic = 9,
        InOutCubic = 10,
        InQuart = 11,
        OutQuart = 12,
        InOutQuart = 13,
        InQuint = 14,
        OutQuint = 15,
        InOutQuint = 16,
        InExpo = 17,
        OutExpo = 18,
        InOutExpo = 19,
        InCirc = 20,
        OutCirc = 21,
        InOutCirc = 22,
        InElastic = 23,
        OutElastic = 24,
        InOutElastic = 25,
        InBack = 26,
        OutBack = 27,
        InOutBack = 28,
        InBounce = 29,
        OutBounce = 30,
        InOutBounce = 31,
        //
        // Summary:
        //     Don't assign this! It's assigned automatically when creating 0 duration tweens
        INTERNAL_Zero = 36,
        //
        // Summary:
        //     Don't assign this! It's assigned automatically when setting the ease to an AnimationCurve
        //     or to a custom ease function
        INTERNAL_Custom = 37
    }

    public enum LoopType
    {
        Restart = 0,
        Yoyo = 1,
        Incremental = 2
    }
    
    public static class Easing
    {
        public static Dictionary<Ease, Func<float, float>> easeDictionary = new Dictionary<Ease, Func<float, float>>()
        {
            { Ease.Linear, EaseFunc.Linear },
            { Ease.InCubic, EaseFunc.CubicIn },
            { Ease.OutCubic, EaseFunc.CubicOut },
            { Ease.InOutCubic, EaseFunc.CubicInOut },
            { Ease.InQuad, EaseFunc.QuadIn },
            { Ease.OutQuad, EaseFunc.QuadOut },
            { Ease.InOutQuad, EaseFunc.QuadInOut },
            { Ease.InQuart, EaseFunc.QuartIn },
            { Ease.OutQuart, EaseFunc.QuartOut },
            { Ease.InOutQuart, EaseFunc.QuartInOut },
            { Ease.InQuint, EaseFunc.QuintIn },
            { Ease.OutQuint, EaseFunc.QuintOut },
            { Ease.InOutQuint, EaseFunc.QuintInOut },
            { Ease.InSine, EaseFunc.SineIn },
            { Ease.OutSine, EaseFunc.SineOut },
            { Ease.InOutSine, EaseFunc.SineInOut },
            { Ease.InExpo, EaseFunc.ExpoIn },
            { Ease.OutExpo, EaseFunc.ExpoOut },
            { Ease.InOutExpo, EaseFunc.ExpoInOut },
            { Ease.InCirc, EaseFunc.CircIn },
            { Ease.OutCirc, EaseFunc.CircOut },
            { Ease.InOutCirc, EaseFunc.CircInOut },
            { Ease.InElastic, EaseFunc.ElasticIn },
            { Ease.OutElastic, EaseFunc.ElasticOut },
            { Ease.InOutElastic, EaseFunc.ElasticInOut },
            { Ease.InBack, EaseFunc.BackIn },
            { Ease.OutBack, EaseFunc.BackOut },
            { Ease.InOutBack, EaseFunc.BackInOut },
            { Ease.InBounce, EaseFunc.BounceIn },
            { Ease.OutBounce, EaseFunc.BounceOut },
            { Ease.InOutBounce, EaseFunc.BounceInOut }
        };

        public static Func<float, float> GetIntepreter(Ease ease) => easeDictionary[ease];
    }

    public class Tween
    {
        private Ease m_Ease;
        public Ease ease
        {
            get => m_Ease;
            set
            {
                m_Ease = value;
                intepreter = Easing.GetIntepreter(value);
            }
        }

        public Func<float, float> intepreter { get; private set; } = EaseFunc.Linear;

        public object? start;
        public object? end;
        public object? result;
        public float current;
        public float speed = 1;
        public float length;
        public Action<object>? onUpdate;
        public Action? onFinish;

        public bool finished { get; private set; }
        public bool forceComplete { get; private set; }
        public LoopType loopType { get; private set; }
        public int loopCount { get; private set; }

        public Tween SetEase(Ease ease)
        {
            m_Ease = ease;
            intepreter = Easing.GetIntepreter(ease);
            return this;
        }

        public Tween SetOnUpdate(Action<object> obj)
        {
            onUpdate = obj;
            return this;
        }

        public Tween SetOnUpdate(Action<float> obj)
        {
            onUpdate += val => obj?.Invoke((float)val);
            return this;
        }

        public Tween SetOnUpdate(Action<Vector2> obj)
        {
            onUpdate += val => obj?.Invoke((Vector2)val);
            return this;
        }

        public Tween SetOnUpdate(Action<Vector3> obj)
        {
            onUpdate += val => obj?.Invoke((Vector3)val);
            return this;
        }

        public Tween SetOnUpdate(Action<Vector4> obj)
        {
            onUpdate += val => obj?.Invoke((Vector4)val);
            return this;
        }

        public Tween SetOnUpdate(Action<ColorF> obj)
        {
            onUpdate += val => obj?.Invoke((ColorF)val);
            return this;
        }

        public Tween SetOnFinish(Action obj)
        {
            onFinish += obj.Invoke;
            return this;
        }

        public Tween SetLoops(int count, LoopType type)
        {
            loopType = type;
            loopCount = count;
            return this;
        }

        public void ConfirmLoop()
        {
            finished = false;
            current = loopType == LoopType.Yoyo ? length : 0;
            speed = loopType == LoopType.Yoyo ? -speed : speed;
            loopCount--;
        }

        public virtual void Ease()
        {
            var n = current + (Time.GetFrameTime() * speed);
            if (speed > 0)
                current = n > length ? GetFinal() : n;
            else
                current = n < 0 ? GetFinalReverse() : n;
        }

        /// <summary>
        /// Sets how fast the animation should go
        /// </summary>
        /// <exception cref="NotSupportedException">Tween speed set to 0 is not supported</exception>
        public Tween SetSpeed(float to)
        {
            if (to == 0) throw new NotSupportedException("Tween speed set to 0 is not supported");
            speed = to;
            return this;
        }

        public void Kill(bool complete = false)
        {
            loopCount = 0;
            finished = true;
            if(complete) current = length;
        }

        float GetFinal()
        {
            finished = true;
            return length;
        }

        float GetFinalReverse()
        {
            finished = true;
            return 0;
        }
    }

    // Easeable
    public class ColorTween : Tween
    {
        public override void Ease()
        {
            var a = (ColorF)start!;
            var b = (ColorF)end!;
            base.Ease();
            var r = intepreter.Invoke(current / length);
            var m = b - a;
            result = a + (m * r);
            if (onUpdate != null) onUpdate.Invoke(result);
        }
    }

    public class Vector4Tween : Tween
    {
        public override void Ease()
        {
            var a = (Vector4)start!;
            var b = (Vector4)end!;
            base.Ease();
            var r = intepreter.Invoke(current / length);
            var m = b - a;

            result = a + (m * r);
            if (onUpdate != null) onUpdate.Invoke(result);
        }
    }

    public class Vector3Tween : Tween
    {
        public override void Ease()
        {
            var a = (Vector3)start!;
            var b = (Vector3)end!;
            base.Ease();
            var r = intepreter.Invoke(current / length);
            var m = b - a;
            result = a + (m * r);
            if (onUpdate != null) onUpdate.Invoke(result);
        }
    }

    public class Vector2Tween : Tween
    {
        public override void Ease()
        {
            var a = (Vector2)start!;
            var b = (Vector2)end!;
            base.Ease();
            var r = intepreter.Invoke(current / length);
            var m = b - a;
            result = a + (m * r);
            if (onUpdate != null) onUpdate.Invoke(result);
        }
    }

    public class FloatTween : Tween
    {
        public override void Ease()
        {
            var a = (float)start!;
            var b = (float)end!;
            base.Ease();
            var r = intepreter.Invoke(current / length);
            var m = b - a;
            result = a + (m * r);
            if (onUpdate != null) onUpdate.Invoke(result);
        }
    }

    public static class EaseFunc
    {
        public static float Linear(float t) => t;

        public static float QuadIn(float t) => t * t;
        public static float QuadOut(float t) => 1 - QuadIn(1 - t);
        public static float QuadInOut(float t)
        {
            if (t < 0.5) return QuadIn(t * 2) / 2;
            return 1 - QuadIn((1 - t) * 2) / 2;
        }

        public static float CubicIn(float t) => t * t * t;
        public static float CubicOut(float t) => 1 - CubicIn(1 - t);
        public static float CubicInOut(float t)
        {
            if (t < 0.5) return CubicIn(t * 2) / 2;
            return 1 - CubicIn((1 - t) * 2) / 2;
        }
    
        public static float IntQuartIn(float t) => (1 / 5) * t * t * t * t * t;
        public static float QuartIn(float t) => t * t * t * t;
        public static float QuartOut(float t) => 1 - QuartIn(1 - t);
        public static float QuartInOut(float t)
        {
            if (t < 0.5) return QuartIn(t * 2) / 2;
            return 1 - QuartIn((1 - t) * 2) / 2;
        }

        public static float QuintIn(float t) => t * t * t * t * t;
        public static float QuintOut(float t) => 1 - QuintIn(1 - t);
        public static float QuintInOut(float t)
        {
            if (t < 0.5) return QuintIn(t * 2) / 2;
            return 1 - QuintIn((1 - t) * 2) / 2;
        }

        const float halfPi = MathF.PI / 2;

        public static float SineIn(float t) => (float)-Math.Cos(t * Math.PI / 2);
        public static float SineOut(float t) => (float)Math.Sin(t * Math.PI / 2);
        public static float SineInOut(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;

        public static float ExpoIn(float t) => MathF.Pow(2, 10 * (t - 1));
        public static float ExpoOut(float t) => 1 - ExpoIn(1 - t);
        public static float ExpoInOut(float t)
        {
            if (t < 0.5) return ExpoIn(t * 2) / 2;
            return 1 - ExpoIn((1 - t) * 2) / 2;
        }

        public static float CircIn(float t) => -(MathF.Sqrt(1 - t * t) - 1);
        public static float CircOut(float t) => 1 - CircIn(1 - t);
        public static float CircInOut(float t)
        {
            if (t < 0.5) return CircIn(t * 2) / 2;
            return 1 - CircIn((1 - t) * 2) / 2;
        }

        const float elasticPowerExp = (2 * MathF.PI) / 0.3f;
        public static float ElasticIn(float t) => 1 - ElasticOut(1 - t);
        public static float ElasticOut(float t) => MathF.Pow(2, -10 * t) * MathF.Sin((t - 0.3f / 4) * elasticPowerExp) + 1;
        public static float ElasticInOut(float t)
        {
            if (t < 0.5) return ElasticIn(t * 2) / 2;
            return 1 - ElasticIn((1 - t) * 2) / 2;
        }

        public static float BackIn(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }
        public static float BackOut(float t) => 1 - BackIn(1 - t);
        public static float BackInOut(float t)
        {
            if (t < 0.5) return BackIn(t * 2) / 2;
            return 1 - BackIn((1 - t) * 2) / 2;
        }

        public static float BounceIn(float t) => 1 - BounceOut(1 - t);
        public static float BounceOut(float t)
        {
            float div = 2.75f;
            float mult = 7.5625f;

            if (t < 1 / div)
            {
                return mult * t * t;
            }
            else if (t < 2 / div)
            {
                t -= 1.5f / div;
                return mult * t * t + 0.75f;
            }
            else if (t < 2.5 / div)
            {
                t -= 2.25f / div;
                return mult * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / div;
                return mult * t * t + 0.984375f;
            }
        }

        public static float BounceInOut(float t)
        {
            if (t < 0.5) return BounceIn(t * 2) / 2;
            return 1 - BounceIn((1 - t) * 2) / 2;
        }
    }
}