using System;

namespace OriNoco.Data
{
    [Serializable]
    public class ChartInfoData
    {
        public string? levelCodename = "untitled_level";
        public string? audioCodename = "Unnamed";

        public string? displayName = "Untitled";
        public string? chartCreator = "Unknown";
        public string? levelComposer = "Unknown";
        public float levelDifficulty = 0f;
        public ComposerSocials[] composerSocials = [];
        public LevelSets levelSet = LevelSets.Community;

        public float speed = 1f;
        public float syncOffset;
        public float syncOffsetAnim = -1;

        public float noteAnimationTimeDelay = 1f;
        public float noteAnimationSpeed = 0.3f;
        public float orthographicSize = 5f;

        public ColorF globalSpriteColor = new ColorF(1f, 1f, 1f);
        public bool invertedColors;

        public ColorF bgColor;
        public ColorF lineColor;
        public ColorF particleColor;
        public ColorF gradientColor;
        public ColorF textColor;
        public ColorF firefliesColor;

        public bool isLockLevel;
        public LockType lockMode;
        public int requiredAmount;
    }

    [Serializable]
    public struct ComposerSocials
    {
        public string url;
        public string path;
    }

    public enum LockType
    {
        RankingScore,
        Score,
        Accuracy
    }

    public enum LevelSets
    {
        Chapter1,
        Chapter2,
        Chapter3,
        Chapter4,
        SideStory1,
        SideStory2,
        Single,
        Community,
        Legacy,
        Test
    }
}
