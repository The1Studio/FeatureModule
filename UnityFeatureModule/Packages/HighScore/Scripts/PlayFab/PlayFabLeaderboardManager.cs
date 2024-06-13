#if THEONE_PLAYFAB
#nullable enable
namespace TheOneStudio.HighScore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using PlayFab;
    using PlayFab.ClientModels;
    using TheOneStudio.HighScore.Models;
    using UniT.Extensions;
    using UnityEngine.Device;
    using UnityEngine.Scripting;
    using Zenject;
    using PlayerLeaderboardEntry = TheOneStudio.HighScore.Models.PlayerLeaderboardEntry;

    internal sealed class PlayFabLeaderboardManager : ILeaderboardManager, IInitializable
    {
        #region Constructor

        private readonly PlayFabLeaderboardConfig config;

        private readonly Dictionary<(string Key, HighScoreType Type), PlayerLeaderboardEntry>                                 cachedCurrentPlayerEntry = new();
        private readonly Dictionary<(string Key, HighScoreType Type, int StartPosition, int Count), PlayerLeaderboardEntry[]> cachedLeaderboard        = new();

        [Preserve]
        public PlayFabLeaderboardManager(PlayFabLeaderboardConfig config)
        {
            this.config = config;
        }

        #endregion

        private Action? onInitialized;

        public event Action OnInitialized
        {
            add
            {
                this.onInitialized += value;
                if (this.IsInitialized) value();
            }
            remove => this.onInitialized -= value;
        }

        public ILeaderboardConfig Config        => this.config;
        public bool               IsInitialized { get; private set; }
        public Player             CurrentPlayer { get; private set; } = null!;

        void IInitializable.Initialize() => UniTask.Void(async () =>
        {
            #if UNITY_EDITOR
            var loginResult = await InvokeAsync<LoginWithCustomIDRequest, LoginResult>(
                PlayFabClientAPI.LoginWithCustomID,
                new() { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true }
            );
            #elif UNITY_ANDROID
            var loginResult = await InvokeAsync<LoginWithAndroidDeviceIDRequest, LoginResult>(
                PlayFabClientAPI.LoginWithAndroidDeviceID,
                new() { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true }
            );
            #elif UNITY_IOS
            var loginResult = await InvokeAsync<LoginWithIOSDeviceIDRequest, LoginResult>(
                PlayFabClientAPI.LoginWithIOSDeviceID,
                new() { DeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true }
            );
            #endif

            await (
                this.GetPlayerAsync(loginResult.PlayFabId).ContinueWith<Player>(player => this.CurrentPlayer = player),
                this.config.UsedValues.ForEachAsync((key, type) => UniTask.WhenAll(
                    this.GetCurrentPlayerEntryAsyncInternal(key, type),
                    this.GetLeaderboardAsyncInternal(key, type, 0, 100)
                ))
            );

            this.IsInitialized = true;
            this.onInitialized?.Invoke();
        });

        public async UniTask<Player> GetPlayerAsync(string id)
        {
            var result = await InvokeAsync<GetPlayerProfileRequest, GetPlayerProfileResult>(
                PlayFabClientAPI.GetPlayerProfile,
                new()
                {
                    PlayFabId          = id,
                    ProfileConstraints = this.config.ProfileConstraints,
                }
            );
            return Convert(result.PlayerProfile);
        }

        public async UniTask ChangeNameAsync(string name)
        {
            Throw.IfNotInitialized(this);
            await InvokeAsync<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(
                PlayFabClientAPI.UpdateUserTitleDisplayName,
                new() { DisplayName = name }
            );
            this.CurrentPlayer.Name = name;
        }

        public async UniTask ChangeAvatarAsync(string avatar)
        {
            Throw.IfNotInitialized(this);
            await InvokeAsync<UpdateAvatarUrlRequest, EmptyResponse>(
                PlayFabClientAPI.UpdateAvatarUrl,
                new() { ImageUrl = avatar }
            );
            this.CurrentPlayer.Avatar = avatar;
        }

        public async UniTask SubmitAsync(string key, HighScoreType type, int score)
        {
            Throw.IfNotInitialized(this);
            await InvokeAsync<UpdatePlayerStatisticsRequest, UpdatePlayerStatisticsResult>(
                PlayFabClientAPI.UpdatePlayerStatistics,
                new()
                {
                    Statistics = new()
                    {
                        new()
                        {
                            StatisticName = GetKey(key, type),
                            Value         = score,
                        },
                    },
                }
            );
        }

        public UniTask<PlayerLeaderboardEntry> GetCurrentPlayerEntryAsync(string key, HighScoreType type)
        {
            Throw.IfNotInitialized(this);
            return this.GetCurrentPlayerEntryAsyncInternal(key, type);
        }

        public UniTask<IEnumerable<PlayerLeaderboardEntry>> GetLeaderboardAsync(string key, HighScoreType type, int startPosition, int count)
        {
            Throw.IfNotInitialized(this);
            Throw.IfLessThanZero(startPosition, nameof(startPosition));
            Throw.IfLessThanZero(count, nameof(count));
            Throw.IfGreaterThanOneHundred(count, nameof(count));
            return this.GetLeaderboardAsyncInternal(key, type, startPosition, count);
        }

        public UniTask FetchUsedCurrentPlayerEntriesAsync()
        {
            return this.config.UsedValues.ForEachAsync((key, type) => this.GetCurrentPlayerEntryAsync(key, type));
        }

        public UniTask FetchUsedLeaderboardsAsync(int startPosition, int count)
        {
            return this.config.UsedValues.ForEachAsync((key, type) => this.GetLeaderboardAsync(key, type, startPosition, count));
        }

        public UniTask FetchAllUsedValuesAsync(int startPosition, int count)
        {
            return this.config.UsedValues.ForEachAsync((key, type) => UniTask.WhenAll(
                this.GetCurrentPlayerEntryAsync(key, type),
                this.GetLeaderboardAsync(key, type, startPosition, count)
            ));
        }

        public PlayerLeaderboardEntry? GetCachedCurrentPlayerEntry(string key, HighScoreType type)
        {
            return this.cachedCurrentPlayerEntry.GetOrDefault((key, type));
        }

        public IEnumerable<PlayerLeaderboardEntry> GetCachedLeaderboard(string key, HighScoreType type, int startPosition, int count)
        {
            return this.cachedLeaderboard.GetOrDefault((key, type, startPosition, count)) ?? Enumerable.Empty<PlayerLeaderboardEntry>();
        }

        #region Private

        private async UniTask<PlayerLeaderboardEntry> GetCurrentPlayerEntryAsyncInternal(string key, HighScoreType type)
        {
            var result = await InvokeAsync<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(
                PlayFabClientAPI.GetLeaderboardAroundPlayer,
                new()
                {
                    StatisticName      = GetKey(key, type),
                    MaxResultsCount    = 1,
                    ProfileConstraints = this.config.ProfileConstraints,
                }
            );
            return this.cachedCurrentPlayerEntry[(key, type)] = Convert(result.Leaderboard.Single(entry => entry.PlayFabId == this.CurrentPlayer.Id));
        }

        public async UniTask<IEnumerable<PlayerLeaderboardEntry>> GetLeaderboardAsyncInternal(string key, HighScoreType type, int startPosition, int count)
        {
            var result = await InvokeAsync<GetLeaderboardRequest, GetLeaderboardResult>(
                PlayFabClientAPI.GetLeaderboard,
                new()
                {
                    StatisticName      = GetKey(key, type),
                    StartPosition      = startPosition,
                    MaxResultsCount    = count,
                    ProfileConstraints = this.config.ProfileConstraints,
                }
            );
            return this.cachedLeaderboard[(key, type, startPosition, count)] = result.Leaderboard.Select(Convert).ToArray();
        }

        private static UniTask<TResult> InvokeAsync<TRequest, TResult>(PlayFabAction<TRequest, TResult> action, TRequest request)
        {
            var tcs = new UniTaskCompletionSource<TResult>();
            action(
                request,
                result => tcs.TrySetResult(result),
                error => tcs.TrySetException(new(error.ErrorMessage))
            );
            return tcs.Task;
        }

        private static string GetKey(string key, HighScoreType type) => $"{key}_{type}";

        private static Player Convert(PlayerProfileModel player) => new(
            player.PlayerId,
            player.DisplayName,
            player.AvatarUrl,
            player.Locations?
                .Where(location => location.CountryCode is { })
                .LastOrDefault()?
                .CountryCode
                .ToString()
                .ToLower()
        );

        private static PlayerLeaderboardEntry Convert(PlayFab.ClientModels.PlayerLeaderboardEntry playerLeaderboardEntry) => new(
            Convert(playerLeaderboardEntry.Profile),
            playerLeaderboardEntry.StatValue,
            playerLeaderboardEntry.Position
        );

        private delegate void PlayFabAction<in TRequest, out TResult>(TRequest request, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object? customData = null, Dictionary<string, string>? extraHeaders = null);

        #endregion
    }
}
#endif