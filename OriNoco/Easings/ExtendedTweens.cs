using System.Numerics;

namespace OriNoco.Tweening
{
    /// <summary>
    /// This class contains a lot of extended methods for components
    /// </summary>
    public static class ExtendedTweens
    {
        public static Vector2Tween EMove(this Drawable transform, Vector2 to, float time) =>
            (Vector2Tween)ETween.TweenVector2(transform.Position, to, time)
            .SetOnUpdate((Vector2 val) => transform.Position = val);

        public static FloatTween ERotate(this Drawable transform, float to, float time) =>
            (FloatTween)ETween.TweenFloat(transform.Rotation, to, time)
            .SetOnUpdate((float val) => transform.Rotation = val);

        public static Vector2Tween EScale(this Drawable transform, Vector2 to, float time) =>
            (Vector2Tween)ETween.TweenVector2(transform.Scale, to, time)
            .SetOnUpdate((Vector2 val) => transform.Scale = val);
    }
}
