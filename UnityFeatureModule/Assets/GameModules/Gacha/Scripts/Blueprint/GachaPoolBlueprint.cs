namespace TheOneStudio.HyperCasual.GameModules.Gacha.Scripts.Blueprint
{
    using System;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("GachaPool", true)]
    [CsvHeaderKey("PackId")]
    public class GachaPoolBlueprint : GenericBlueprintReaderByRow<string, GachaPoolRecord>
    {
    }

    public class GachaPoolRecord
    {
        public string PackId { get; set; }

        public BlueprintByRow<string, GachaPoolItemRecord> ItemIdToData;
    }

    [Serializable]
    [CsvHeaderKey("PoolItemId")]
    public class GachaPoolItemRecord
    {
        public string PoolItemId;
        public string Item;
        public int    Amount;
        public float  Weight;
        public string Rarity;
    }
}