#nullable enable
namespace TheOneStudio.HighScore
{
    using Zenject;

    public static class LocalDataHighScoreManagerBinder
    {
        public static void BindLocalDataHighScoreManager(this DiContainer container, int cacheSize = 3)
        {
            container.BindInterfacesAndSelfTo<HighScoreConfig>()
                .FromMethod(_ => new HighScoreConfig(cacheSize))
                .AsSingle();

            container.BindInterfacesTo<HighScoreDataController>()
                .AsSingle();
        }
    }
}