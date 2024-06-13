#if THEONE_PLAYFAB
#nullable enable
namespace TheOneStudio.HighScore
{
    using System.Collections.Generic;
    using TheOneStudio.HighScore.Models;
    using Zenject;

    public static class PlayFabLeaderboardManagerBinder
    {
        public static void BindPlayFabLeaderboardManager(
            this DiContainer            container,
            IEnumerable<string>?        usedKeys       = null,
            IEnumerable<HighScoreType>? usedTypes      = null,
            bool                        useName        = false,
            bool                        useAvatar      = false,
            bool                        useCountryCode = false,
            bool                        useAutoSync    = true
        )
        {
            container.BindLocalDataHighScoreManager();

            container.BindInterfacesAndSelfTo<PlayFabLeaderboardConfig>()
                .FromMethod(_ => new PlayFabLeaderboardConfig(
                    usedKeys ?? new[] { ILeaderboardManager.DEFAULT_KEY },
                    usedTypes ?? new[] { HighScoreType.Daily, HighScoreType.Weekly, HighScoreType.Monthly, HighScoreType.AllTime },
                    new()
                    {
                        ShowDisplayName = useName,
                        ShowAvatarUrl   = useAvatar,
                        ShowLocations   = useCountryCode,
                    }
                ))
                .AsSingle();

            container.BindInterfacesTo<PlayFabLeaderboardManager>()
                .AsSingle();

            if (useAutoSync)
            {
                container.BindInterfacesTo<AutoSyncService>()
                    .AsSingle();
            }
        }
    }
}
#endif