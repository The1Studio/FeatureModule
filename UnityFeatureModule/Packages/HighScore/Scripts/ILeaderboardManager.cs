#nullable enable
namespace TheOneStudio.HighScore
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.HighScore.Models;

    public interface ILeaderboardManager
    {
        public event Action OnInitialized;

        public ILeaderboardConfig Config { get; }

        public bool IsInitialized { get; }

        public Player CurrentPlayer { get; }

        public UniTask ChangeNameAsync(string name);

        public UniTask ChangeAvatarAsync(string avatar);

        public UniTask<Player> GetPlayerAsync(string id);

        public UniTask SubmitAsync(string key, HighScoreType type, int score);

        public UniTask<PlayerLeaderboardEntry> GetCurrentPlayerEntryAsync(string key, HighScoreType type);

        public UniTask<IEnumerable<PlayerLeaderboardEntry>> GetLeaderboardAsync(string key, HighScoreType type, int startPosition = 0, int count = 100);

        public UniTask FetchUsedCurrentPlayerEntriesAsync();

        public UniTask FetchUsedLeaderboardsAsync(int startPosition = 0, int count = 100);

        public UniTask FetchAllUsedValuesAsync(int startPosition = 0, int count = 100);

        public PlayerLeaderboardEntry? GetCachedCurrentPlayerEntry(string key, HighScoreType type);

        public IEnumerable<PlayerLeaderboardEntry> GetCachedLeaderboard(string key, HighScoreType type, int startPosition = 0, int count = 100);

        #region Default

        public const string DEFAULT_KEY = "DEFAULT";

        public UniTask SubmitAsync(HighScoreType type, int score) => this.SubmitAsync(DEFAULT_KEY, type, score);

        public UniTask<PlayerLeaderboardEntry> GetCurrentPlayerEntryAsync(HighScoreType type) => this.GetCurrentPlayerEntryAsync(DEFAULT_KEY, type);

        public UniTask<IEnumerable<PlayerLeaderboardEntry>> GetLeaderboardAsync(HighScoreType type, int startPosition = 0, int count = 100) => this.GetLeaderboardAsync(DEFAULT_KEY, type, startPosition, count);

        public PlayerLeaderboardEntry? GetCachedCurrentPlayerEntry(HighScoreType type) => this.GetCachedCurrentPlayerEntry(DEFAULT_KEY, type);

        public IEnumerable<PlayerLeaderboardEntry> GetCachedLeaderboard(HighScoreType type, int startPosition = 0, int count = 100) => this.GetCachedLeaderboard(DEFAULT_KEY, type, startPosition, count);

        #endregion
    }
}