#nullable enable
namespace TheOneStudio.HighScore
{
    using Cysharp.Threading.Tasks;
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
            if (this.leaderboardManager.IsInitialized)
            {
                InitializeInternal();
            }
            else
            {
                this.leaderboardManager.OnInitialized += InitializeInternal;
            }

            void InitializeInternal()
            {
                this.highScoreManager.OnNewHighScore += this.OnNewHighScore;
                this.SyncHighScore();
            }
        }

        private void OnNewHighScore(string key, HighScoreType type, int oldHighScore, int newHighScore)
        {
            this.leaderboardManager.SubmitAsync(key, type, newHighScore);
        }

        private void SyncHighScore() => UniTask.Void(async () =>
        {
            await this.leaderboardManager.FetchAllUsedValuesAsync();
            this.leaderboardConfig.UsedValues.ForEach((key, type) =>
            {
                var entry = this.leaderboardManager.GetCachedCurrentPlayerEntry(key, type)!;
                this.highScoreManager.Submit(key, type, entry.Score);
            });
        });
    }
}