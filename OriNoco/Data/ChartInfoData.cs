using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OriNoco.Data
{
    [Serializable]
    public class ChartInfoData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "untitled_level";
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = "Untitled";
        [JsonPropertyName("author")]
        public string Author { get; set; } = "Unknown";

        [JsonPropertyName("audioName")]
        public string AudioName { get; set; } = "Unnamed";
        [JsonPropertyName("audioPath")]
        public string AudioPath { get; set; } = "";
        [JsonPropertyName("audioComposer")]
        public string AudioComposer { get; set; } = "Unknown";
        [JsonPropertyName("syncOffset")]
        public float AudioOffset { get; set; }
        [JsonPropertyName("composerSocials")]
        public List<ComposerSocials> ComposerSocials { get; set; } = [];

        [JsonPropertyName("levelDifficulty")]
        public float LevelDifficulty { get; set; } = 0f;
        [JsonPropertyName("levelSet")]
        public LevelSets LevelSet { get; set; } = LevelSets.Community;

        [JsonPropertyName("noteAnimationTimeDelay")]
        public float NoteAnimationTimeDelay { get; set; } = 1f;
        [JsonPropertyName("noteAnimationSpeed")]
        public float NoteAnimationSpeed { get; set; } = 0.3f;
        [JsonPropertyName("orthographicSize")]
        public float OrthographicSize { get; set; } = 5f;

        [JsonPropertyName("globalSpriteColor")]
        public ColorF GlobalSpriteColor { get; set; } = new ColorF(1f);
        [JsonPropertyName("invertedColors")]
        public bool InvertedColors { get; set; }

        [JsonPropertyName("backgroundColor")]
        public ColorF BackgroundColor { get; set; } = new ColorF(0f);
        [JsonPropertyName("lineColor")]
        public ColorF LineColor { get; set; } = new ColorF(1f);
        [JsonPropertyName("particleColor")]
        public ColorF ParticleColor { get; set; } = new ColorF(0.3113208f);
        [JsonPropertyName("gradientColor")]
        public ColorF GradientColor { get; set; } = new ColorF(0f);
        [JsonPropertyName("textColor")]
        public ColorF TextColor { get; set; } = new ColorF(1f);
        [JsonPropertyName("firefliesColor")]
        public ColorF FirefliesColor { get; set; }

        [JsonPropertyName("isLockLevel")]
        public bool IsLockLevel { get; set; }
        [JsonPropertyName("lockMode")]
        public LockType LockMode { get; set; }
        [JsonPropertyName("requiredLevelName")]
        public string RequiredLevelName { get; set; } = "None";
        [JsonPropertyName("requiredAmount")]
        public int RequiredAmount { get; set; }
    }

    [Serializable]
    public struct ComposerSocials
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
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
