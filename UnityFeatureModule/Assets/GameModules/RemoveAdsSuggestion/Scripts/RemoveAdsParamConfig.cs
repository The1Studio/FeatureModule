namespace TheOneStudio.HyperCasual.RemoveAdsSuggestion.Scripts
{
    using Newtonsoft.Json;

    // Remote Config
    public class RemoveAdsParamConfig
    {
        [JsonProperty("enable_after_session")]
        public int EnableAfterSession { get; set; } = 2;

        [JsonProperty("suggest_capping_time")]
        public float SuggestCappingTime { get; set; } = 60;

        [JsonProperty("first_enable_after_inter_ads")]
        public int FirstEnableAfterInterAds { get; set; } = 2;

        [JsonProperty("inter_ads_interval")]
        public int InterAdsInterval { get; set; } = 8;
    }
}