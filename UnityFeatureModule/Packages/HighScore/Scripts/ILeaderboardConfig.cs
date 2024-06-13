#nullable enable
namespace TheOneStudio.HighScore
{
    using System.Collections.Generic;
    using TheOneStudio.HighScore.Models;

    public interface ILeaderboardConfig
    {
        public IReadOnlyCollection<(string Key, HighScoreType Type)> UsedValues { get; }
    }
}