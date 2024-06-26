namespace TheOneStudio.GameFeature.Decoration.LocalData.Controllers
{
    using System.Collections.Generic;
    using TheOneStudio.GameFeature.Decoration.Blueprints;
    using TheOneStudio.GameFeature.Decoration.LocalData.Data;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class DecorationLocalDataController : IUITemplateControllerData
    {
        private readonly DecorationLocalData decorationLocalData;
        private readonly DecorationBlueprint decorationBlueprint;

        public DecorationLocalDataController(DecorationLocalData decorationLocalData,
                                             DecorationBlueprint decorationBlueprint)
        {
            this.decorationLocalData = decorationLocalData;
            this.decorationBlueprint = decorationBlueprint;
        }

        public int GetCurrentCityId() { return this.decorationLocalData.CurrencyCityId; }

        public DecorationGroupLocalData GetCurrentCityData() { return this.decorationLocalData.IdToCityData[this.decorationLocalData.CurrencyCityId]; }

        public DecorationGroupLocalData GetCityData(int cityId, bool isForce = false)
        {
            if (this.decorationLocalData.IdToCityData.ContainsKey(cityId))
            {
                return this.decorationLocalData.IdToCityData[cityId];
            }

            //create new city data
            if (isForce && this.decorationBlueprint.TryGetValue(cityId, out var record))
            {
                var buildingAmount = record.BuildingRecords.Count;
                this.UnlockNewCity(cityId, buildingAmount);

                return this.decorationLocalData.IdToCityData[cityId];
            }

            return null;
        }

        public void UpdateProgress(int value)
        {
            var cityData             = this.GetCurrentCityData();
            var currentBuildingIndex = cityData.CurrentBuildingIndex;
            cityData.IndexToCostPaid[currentBuildingIndex] += value;
        }

        public void UnlockNextBuilding()
        {
            var cityData = this.GetCurrentCityData();
            cityData.CurrentBuildingIndex++;
        }

        public void UnlockNextCity(int cityId, int buildingAmount)
        {
            this.UnlockNewCity(cityId, buildingAmount);
            this.decorationLocalData.CurrencyCityId = cityId;
        }

        public void UnlockNewCity(int cityId, int buildingAmount)
        {
            if (this.decorationLocalData.IdToCityData.ContainsKey(cityId)) return;
            var idToCostPaid = new Dictionary<int, int>();
            for (var i = 0; i < buildingAmount; i++)
            {
                idToCostPaid.Add(i, 0);
            }

            var cityData = new DecorationGroupLocalData()
            {
                Id                   = cityId,
                CurrentBuildingIndex = 0,
                IndexToCostPaid      = idToCostPaid,
            };

            this.decorationLocalData.IdToCityData.Add(cityId, cityData);
        }

        public Dictionary<int, DecorationGroupLocalData> GetAllCityData() { return this.decorationLocalData.IdToCityData; }

        public void SetCityComplete(int cityId) { this.decorationLocalData.IdToCityData[cityId].IsComplete = true; }
    }
}