using Raylib_CSharp.Textures;

namespace OriNoco
{
    public static class TextureDictionary
    {
        public static Texture2D note { get; private set; }
        public static Texture2D arrow { get; private set; }
        public static Texture2D inverse_note { get; private set; }

        public static void Load()
        {
            note = Texture2D.Load("Images/note.png");
            arrow = Texture2D.Load("Images/arrow.png");
            inverse_note = Texture2D.Load("Images/inverse_note.png");
        }
    }
}