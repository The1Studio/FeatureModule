namespace TheOneStudio.GameFeature.Decoration.LocalData.Data
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.GameFeature.Decoration.LocalData.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public class DecorationLocalData : ILocalData, IUITemplateLocalData
    {
        public Type ControllerType => typeof(DecorationLocalDataController);

        public int                               CurrencyCityId { get; set; }
        public Dictionary<int, DecorationGroupLocalData> IdToCityData   { get; set; } = new();

        public void Init() { }
    }

    public class DecorationGroupLocalData
    {
        public int                  Id;
        public int                  CurrentBuildingIndex;
        public Dictionary<int, int> IndexToCostPaid;
        public bool                 IsComplete;
    }
}