#if THEONE_PLAYFAB
#nullable enable
namespace TheOneStudio.HighScore
{
    using System.Collections.Generic;
    using System.Linq;
    using PlayFab.ClientModels;
    using TheOneStudio.HighScore.Models;
    using UniT.Extensions;

    internal sealed class PlayFabLeaderboardConfig : ILeaderboardConfig
    {
        public IReadOnlyCollection<(string Key, HighScoreType Type)> UsedValues         { get; }
        public PlayerProfileViewConstraints                          ProfileConstraints { get; }

        public PlayFabLeaderboardConfig(IEnumerable<string> usedKeys, IEnumerable<HighScoreType> usedTypes, PlayerProfileViewConstraints profileConstraints)
        {
            this.UsedValues         = IterTools.Product(usedKeys, usedTypes).ToArray();
            this.ProfileConstraints = profileConstraints;
        }
    }
}
#endif