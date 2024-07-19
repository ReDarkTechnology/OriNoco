using Raylib_CSharp.Textures;

namespace OriNoco
{
    public static class TextureDictionary
    {
        public static Texture2D note { get; private set; }
        public static Texture2D arrow { get; private set; }
        public static Texture2D inverseNote { get; private set; }

        public static Texture2D up { get; private set; }
        public static Texture2D upActive { get; private set; }
        public static Texture2D down { get; private set; }
        public static Texture2D downActive { get; private set; }
        public static Texture2D left { get; private set; }
        public static Texture2D leftActive { get; private set; }
        public static Texture2D right { get; private set; }
        public static Texture2D rightActive { get; private set; }

        public static void Init()
        {
            note = Texture2D.Load("Images/note.png");
            arrow = Texture2D.Load("Images/arrow.png");
            inverseNote = Texture2D.Load("Images/inverse_note.png");

            up = Texture2D.Load("Images/up.png");
            upActive = Texture2D.Load("Images/up_active.png");
            down = Texture2D.Load("Images/down.png");
            downActive = Texture2D.Load("Images/down_active.png");
            left = Texture2D.Load("Images/left.png");
            leftActive = Texture2D.Load("Images/left_active.png");
            right = Texture2D.Load("Images/right.png");
            rightActive = Texture2D.Load("Images/right_active.png");
        }
    }
}