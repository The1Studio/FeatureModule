#nullable enable
namespace TheOneStudio.HighScore.Models
{
    public sealed class Player
    {
        public string  Id          { get; }
        public string? Name        { get; internal set; }
        public string? Avatar      { get; internal set; }
        public string? CountryCode { get; }

        public Player(string id, string? name, string? avatar, string? countryCode)
        {
            this.Id          = id;
            this.Name        = name;
            this.Avatar      = avatar;
            this.CountryCode = countryCode;
        }
    }
}