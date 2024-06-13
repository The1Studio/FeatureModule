#nullable enable
namespace TheOneStudio.HighScore.Models
{
    public sealed class PlayerLeaderboardEntry
    {
        public Player Player   { get; }
        public int    Score    { get; }
        public int    Position { get; }

        public PlayerLeaderboardEntry(Player player, int score, int position)
        {
            this.Player   = player;
            this.Score    = score;
            this.Position = position;
        }
    }
}