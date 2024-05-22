namespace TheOneStudio.HyperCasual.GameModules.Gacha.Scripts.Blueprint
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("Gacha", true)]
    [CsvHeaderKey("Id")]
    public class GachaBlueprint : GenericBlueprintReaderByRow<string, GachaRecord>
    {
    }

    public class GachaRecord
    {
        public string                  Id;
        public Dictionary<string, int> Price1Time;
        public Dictionary<string, int> Price10Times;
        public Dictionary<string, int> GachaPackId;
        public string                  GachaImageAddressable;
        public string                  IconAddressable;
        public string                  Title;
    }
}