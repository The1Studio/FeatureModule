#nullable enable
namespace TheOneStudio.HighScore
{
    using TheOneStudio.HighScore.Models;
    using UniT.Extensions;
    using UnityEngine.Scripting;
    using Zenject;

    public sealed class AutoSyncService : IInitializable
    {
        private readonly ILeaderboardConfig  leaderboardConfig;
        private readonly IHighScoreManager   highScoreManager;
        private readonly ILeaderboardManager leaderboardManager;

        [Preserve]
        public AutoSyncService(ILeaderboardConfig leaderboardConfig, IHighScoreManager highScoreManager, ILeaderboardManager leaderboardManager)
        {
            this.leaderboardConfig  = leaderboardConfig;
            this.highScoreManager   = highScoreManager;
            this.leaderboardManager = leaderboardManager;
        }

        public void Initialize()
        {
            this.leaderboardManager.OnInitialized += InitializeInternal;

            void InitializeInternal()
            {
                this.SyncHighScore();
                this.highScoreManager.OnNewHighScore  += this.OnNewHighScore;
                this.leaderboardManager.OnInitialized -= InitializeInternal;
            }
        }

        private void SyncHighScore()
        {
            this.leaderboardConfig.UsedValues.ForEach((key, type) =>
            {
                var entry = this.leaderboardManager.GetCachedCurrentPlayerEntry(key, type)!;
                this.highScoreManager.Submit(key, type, entry.Score);
            });
        }

        private void OnNewHighScore(string key, HighScoreType type, int oldHighScore, int newHighScore)
        {
            this.leaderboardManager.SubmitAsync(key, type, newHighScore);
        }
    }
}