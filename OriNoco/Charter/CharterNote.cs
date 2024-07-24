using System.Numerics;

using OriNoco.Rhine;
using Raylib_CSharp.Colors;

namespace OriNoco.Charter
{
    public class CharterNote
    {
        public float time;
        public Direction direction;

        public TextureDrawable drawable = new TextureDrawable(default);

        private CharterScene charterScene;
        public Vector2 cachedPosition;
        private float xPosition;

        public CharterNote() { 
            charterScene = Program.Charter;
        }

        public CharterNote(CharterScene scene, float time, Direction direction)
        {
            charterScene = scene;
            UpdateTexture(direction);
            this.time = time;
        }

        public void UpdateTexture(Direction direction)
        {
            this.direction = direction;
            switch (direction)
            {
                case Direction.Left:
                    drawable = charterScene.left;
                    xPosition = CharterScene.xSpacing * -3f;
                    break;
                case Direction.Down:
                    drawable = charterScene.down;
                    xPosition = CharterScene.xSpacing * -1f;
                    break;
                case Direction.Up:
                    drawable = charterScene.up;
                    xPosition = CharterScene.xSpacing * 1f;
                    break;
                case Direction.Right:
                    drawable = charterScene.right;
                    xPosition = CharterScene.xSpacing * 3f;
                    break;
            }
        }

        public void UpdatePosition() => cachedPosition = charterScene.GetScreenCoords(new Vector2(xPosition, time * charterScene.yScale), true).InvertY();
        public void Draw() => drawable.Draw(cachedPosition, Color.White);
    }
}
