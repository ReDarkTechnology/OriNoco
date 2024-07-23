using System.Numerics;
using System.Collections.Generic;

namespace OriNoco.Tweening
{
    public static class ETween
    {
        public static List<Tween> tweens = new List<Tween>();
        public static List<Tween> onDestroy = new List<Tween>();
        public static System.Action? afterTween;

        public static void Update()
        {
            for (int i = 0; i < tweens.Count; i++)
            {
                // This is intentional, please do not change the structure of this code
                var e = tweens[i];

                if (!e.finished)
                    e.Ease();

                if (e.finished)
                {
                    if (e.loopCount > 0)
                        e.ConfirmLoop();
                    else
                    {
                        e.Ease();
                        onDestroy.Add(e);
                    }
                }
            }

            for (int i = 0; i < onDestroy.Count; i++)
            {
                onDestroy[i].onFinish?.Invoke();
                tweens.Remove(onDestroy[i]);
            }

            afterTween?.Invoke();
        }

        public static FloatTween TweenFloat(float start, float end, float time)
        {
            var t = new FloatTween
            {
                start = start,
                end = end,
                length = time
            };
            tweens.Add(t);
            return t;
        }

        public static Vector2Tween TweenVector2(Vector2 start, Vector2 end, float time)
        {
            var t = new Vector2Tween
            {
                start = start,
                end = end,
                length = time
            };
            tweens.Add(t);
            return t;
        }

        public static Vector3Tween TweenVector3(Vector3 start, Vector3 end, float time)
        {
            var t = new Vector3Tween
            {
                start = start,
                end = end,
                length = time
            };
            tweens.Add(t);
            return t;
        }

        public static Vector4Tween TweenVector4(Vector4 start, Vector4 end, float time)
        {
            var t = new Vector4Tween
            {
                start = start,
                end = end,
                length = time
            };
            tweens.Add(t);
            return t;
        }

        public static ColorTween TweenColor(ColorF start, ColorF end, float time)
        {
            var t = new ColorTween
            {
                start = start,
                end = end,
                length = time
            };
            tweens.Add(t);
            return t;
        }
    }
}