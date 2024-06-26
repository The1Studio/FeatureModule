namespace TheOneStudio.GameFeature.Decoration.Blueprints
{
    using System;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine;

    [CsvHeaderKey("CityId")]
    [BlueprintReader("DecorationBlueprint")]
    public class DecorationBlueprint : GenericBlueprintReaderByRow<int, DecorationRecord>
    {
    }

    public class DecorationRecord
    {
        public int                                 CityId;
        public string                              Name;
        public string                              PrefabName;
        public Vector3                             Position;
        public Vector3                             Rotation;
        public Vector3                             Scale;
        public BlueprintByRow<int, BuildingRecord> BuildingRecords;
    }

    [Serializable]
    [CsvHeaderKey("BuildingIndex")]
    public class BuildingRecord
    {
        public int BuildingIndex;
        public int Cost;
    }
}