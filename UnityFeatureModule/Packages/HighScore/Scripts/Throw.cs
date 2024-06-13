#nullable enable
namespace TheOneStudio.HighScore
{
    using System;

    internal static class Throw
    {
        public static void IfNotInitialized(ILeaderboardManager leaderboardManager)
        {
            if (!leaderboardManager.IsInitialized)
                throw new InvalidOperationException("Not initialized");
        }

        public static void IfLessThanZero(int value, string name)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(name, value, $"{name} must be greater than or equal to zero");
        }

        public static void IfGreaterThanOneHundred(int value, string name)
        {
            if (value > 100)
                throw new ArgumentOutOfRangeException(name, value, $"{name} must be less than or equal to one hundred");
        }
    }
}