namespace TheOneStudio.HighScore
{
    using UnityEngine.Scripting;

    internal sealed class HighScoreConfig
    {
        public int CacheSize { get; }

        [Preserve]
        public HighScoreConfig(int cacheSize)
        {
            this.CacheSize = cacheSize;
        }
    }
}