using System.Numerics;

using Raylib_CSharp;

namespace OriNoco.Rhine
{
    public class RhineTrail
    {
        public ShapeDrawable drawable;
        public ShapeDrawable source;

        public RhineTrail(Drawable player, Direction direction)
        {
            drawable = new ShapeDrawable(ShapeDrawable.ShapeType.Rectangle);
            source = new ShapeDrawable(ShapeDrawable.ShapeType.Circle);

            drawable.Position = player.Position;
            drawable.Rotation = direction.ToRotation();
            drawable.Scale = new Vector2(0.1f, 0);

            source.Position = player.Position;
            source.Scale = new Vector2(0.1f, 0.1f);
        }

        public void Draw()
        {
            source.Draw();
            drawable.Draw();
        }

        public void Stretch(Vector2 direction, float speed)
        {
            drawable.Scale += new Vector2(0f, speed * Time.GetFrameTime());
            drawable.Position += direction * Time.GetFrameTime() * speed / 2;
        }
    }
}
