using Raylib_CSharp.Textures;

namespace OriNoco
{
    public static class TextureDictionary
    {
        public static Texture2D note { get; private set; }
        public static Texture2D arrow { get; private set; }
        public static Texture2D inverseNote { get; private set; }

        static TextureDictionary()
        {
            note = Texture2D.Load("Images/note.png");
            arrow = Texture2D.Load("Images/arrow.png");
            inverseNote = Texture2D.Load("Images/inverse_note.png");
        }
    }
}