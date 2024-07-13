using System.Numerics;

namespace OriNoco
{
    public class Drawable
    {
        public Vector2 Position
        {
            get => new Vector2(ViewportPosition.X, -ViewportPosition.Y);
            set
            {
                ViewportPosition = new Vector2(value.X, -value.Y);
                OnPositionChanged();
            }
        }

        private float _Rotation;
        public float Rotation
        { 
            get => _Rotation;
            set
            {
                _Rotation = value;
                OnRotationChanged();
            }
        }

        private Vector2 _Scale = new Vector2(1, 1);
        public Vector2 Scale
        {
            get => _Scale;
            set
            {
                _Scale = value;
                OnScaleChanged();
            }
        }

        public Vector2 ViewportPosition { get; private set; }

        public virtual void OnPositionChanged() {}
        public virtual void OnRotationChanged() {}
        public virtual void OnScaleChanged() { }
        public virtual void Draw() { }
        public virtual void Draw(Viewport2D viewport) {}
    }
}
