using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public CharterNote() { 
            charterScene = Program.CharterScene;
        }

        public CharterNote(CharterScene scene, float time, Direction direction)
        {
            charterScene = scene;
            UpdateTexture(direction);
            this.time = time;
        }

        public void UpdateTexture(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    drawable = charterScene.left;
                    break;
                case Direction.Right:
                    drawable = charterScene.right;
                    break;
                case Direction.Up:
                    drawable = charterScene.up;
                    break;
                case Direction.Down:
                    drawable = charterScene.down;
                    break;
            }
        }

        public void UpdatePosition()
        {
            Vector2 mainCoords = charterScene.GetScreenCoords(new Vector2(0f, 0f));
            cachedPosition = new (mainCoords.X, charterScene.GetScreenY(time * charterScene.yScale));
        }

        public void Draw()
        {
            drawable.Draw(cachedPosition, Color.White);
        }
    }
}
