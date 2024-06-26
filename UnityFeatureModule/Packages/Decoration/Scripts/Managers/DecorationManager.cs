namespace TheOneStudio.GameFeature.Decoration.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.GameFeature.Decoration.Blueprints;
    using TheOneStudio.GameFeature.Decoration.Elements;
    using TheOneStudio.GameFeature.Decoration.LocalData.Controllers;
    using TheOneStudio.GameFeature.Decoration.LocalData.Data;
    using UnityEngine;

    public class DecorationManager
    {
        #region Inject

        private readonly DecorationLocalDataController    decorationLocalDataController;
        private readonly DecorationBlueprint              decorationBlueprint;
        private readonly DecorationGroupPresenter.Factory cityFactory;
        private readonly DecorationView              decorationView;

        public DecorationManager(DecorationLocalDataController decorationLocalDataController,
                                 DecorationBlueprint decorationBlueprint,
                                 DecorationGroupPresenter.Factory cityFactory,
                                 DecorationView decorationView)
        {
            this.decorationLocalDataController = decorationLocalDataController;
            this.decorationBlueprint           = decorationBlueprint;
            this.cityFactory                   = cityFactory;
            this.decorationView           = decorationView;
        }

        #endregion

        #region Params

        private          bool                                      isInitialized;
        private          Camera                                    mainCamera;
        private readonly Dictionary<int, DecorationGroupPresenter> idToLoadedCity = new();
        public           int                                       PreviewCityId { get; private set; }

        public int                      CostLeft               => this.CurrentDecorationGroup.GetCostLeft();
        public RectTransform            Attractor              => this.CurrentDecorationGroup.Attractor;
        public DecorationGroupPresenter CurrentDecorationGroup { get; private set; }

        #endregion

        #region Load

        public void LoadDecoration()
        {
            if (!this.isInitialized)
            {
                this.mainCamera = Camera.main;
                this.InitializeDecoration().Forget();
                this.isInitialized = true;
            }

            this.SetUpCamera();
            this.decorationView.gameObject.SetActive(true);
        }

        public void UnloadDecoration() { this.decorationView.gameObject.SetActive(false); }

        private async UniTask<DecorationGroupPresenter> LoadDecorationGroupAsync(DecorationGroupModel model)
        {
            if (this.idToLoadedCity.TryGetValue(model.Id, out var cachedCity)) return cachedCity;

            var cityPresenter = this.cityFactory.Create(model);
            await cityPresenter.InitView();
            this.idToLoadedCity.TryAdd(cityPresenter.Model.Id, cityPresenter);
            return cityPresenter;
        }

        private DecorationGroupModel GetDecorationGroupModel(int decorationGroupId, DecorationGroupDirection decorationGroupDirection = DecorationGroupDirection.Center, bool isForce = false)
        {
            var cityRecord = this.decorationBlueprint[decorationGroupId];
            var cityData   = this.decorationLocalDataController.GetCityData(decorationGroupId, isForce);
            return new DecorationGroupModel()
            {
                Id                       = cityRecord.CityId,
                CityName                 = cityRecord.CityName,
                AddressableName          = cityRecord.PrefabName,
                Parent                   = this.decorationView.cityGroup.transform,
                Position                 = cityRecord.Position,
                Rotation                 = cityRecord.Rotation,
                Scale                    = cityRecord.Scale,
                CurrentBuildingIndex     = cityData.CurrentBuildingIndex,
                DecorationGroupDirection = decorationGroupDirection,
                IndexToCostPaid          = cityData.IndexToCostPaid.ToDictionary(x => x.Key, x => x.Value)
            };
        }

        private async UniTask InitializeDecoration()
        {
            var currentCityData = this.GetCurrentCityData();
            var cityId          = currentCityData.Id;
            this.CurrentDecorationGroup = await this.LoadDecorationGroupAsync(this.GetDecorationGroupModel(cityId));
        }

        private void SetUpCamera()
        {
            var camera    = this.mainCamera;
            var transform = camera.transform;
            transform.position    = new Vector3(0, -5, -10);
            transform.eulerAngles = new Vector3(-30, 0, 0);
            camera.orthographic   = false;
        }

        private DecorationGroupLocalData GetCurrentCityData()
        {
            var currentCityId = this.decorationLocalDataController.GetCurrentCityId();
            if (ShouldUpdateCityId())
            {
                UpdateCityId();
            }

            return this.decorationLocalDataController.GetCurrentCityData();

            bool ShouldUpdateCityId()
            {
                if (currentCityId <= 0) return true;
                if (currentCityId == this.decorationBlueprint.Count) return false;

                var cityData       = this.decorationLocalDataController.GetCurrentCityData();
                var cityRecord     = this.decorationBlueprint[currentCityId];
                var buildingRecord = cityRecord.BuildingRecords;
                return cityData.IndexToCostPaid.All(kv => kv.Value >= buildingRecord[kv.Key].Cost);
            }

            void UpdateCityId()
            {
                var cityId = currentCityId + 1;
                if (!this.decorationBlueprint.TryGetValue(cityId, out var newCityRecord)) return;
                this.decorationLocalDataController.UnlockNextCity(cityId, newCityRecord.BuildingRecords.Count);
            }
        }

        #endregion

        #region Build

        public void Build(int costSpend)
        {
            this.CurrentDecorationGroup.Build(costSpend);
            this.decorationLocalDataController.UpdateProgress(costSpend);
        }

        public void UnlockNextDecorationItem()
        {
            this.decorationLocalDataController.UnlockNextBuilding();
            this.CurrentDecorationGroup.UnlockNextBuilding();
        }

        public async UniTask UnlockNextDecorationGroup()
        {
            this.CurrentDecorationGroup.BuildCityComplete();
            var currentCityId = this.decorationLocalDataController.GetCurrentCityId();
            this.decorationLocalDataController.SetCityComplete(currentCityId);
            if (currentCityId >= this.decorationBlueprint.Count) return;

            currentCityId++;
            var record = this.decorationBlueprint[currentCityId];
            this.decorationLocalDataController.UnlockNextCity(currentCityId, record.BuildingRecords.Count);
            var newCity = await this.LoadDecorationGroupAsync(this.GetDecorationGroupModel(currentCityId, DecorationGroupDirection.Right));

            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Left);

            this.CurrentDecorationGroup = newCity;
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Center);
            this.CurrentDecorationGroup.ShowCurrentBuildCost();
        }

        #endregion

        #region Preview Mode

        public async UniTask EnterPreviewMode()
        {
            var currentCityId = this.CurrentDecorationGroup.Model.Id;
            this.PreviewCityId = currentCityId;
            var preloadTask = new List<UniTask>();
            if (currentCityId > 1)
            {
                var prevCityId = currentCityId - 1;
                if (!this.idToLoadedCity.ContainsKey(prevCityId))
                {
                    preloadTask.Add(this.LoadDecorationGroupAsync(this.GetDecorationGroupModel(prevCityId, DecorationGroupDirection.Left, true)));
                }
                else
                {
                    this.idToLoadedCity[prevCityId].Model.DecorationGroupDirection = DecorationGroupDirection.Left;
                    this.idToLoadedCity[prevCityId].SwitchPosition();
                }
            }

            if (currentCityId < this.decorationBlueprint.Count)
            {
                var nextCityId = currentCityId + 1;
                if (!this.idToLoadedCity.ContainsKey(nextCityId))
                {
                    preloadTask.Add(this.LoadDecorationGroupAsync(this.GetDecorationGroupModel(nextCityId, DecorationGroupDirection.Right, true)));
                }
                else
                {
                    this.idToLoadedCity[nextCityId].Model.DecorationGroupDirection = DecorationGroupDirection.Right;
                    this.idToLoadedCity[nextCityId].SwitchPosition();
                }
            }

            await UniTask.WhenAll(preloadTask);
        }

        public async UniTask SwitchLeft()
        {
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Right);
            this.PreviewCityId--;
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Center);

            if (this.PreviewCityId > 1)
            {
                var prevCityId = this.PreviewCityId - 1;
                if (!this.idToLoadedCity.ContainsKey(prevCityId))
                {
                    await this.LoadDecorationGroupAsync(this.GetDecorationGroupModel(prevCityId, DecorationGroupDirection.Left, true));
                }
                else
                {
                    this.idToLoadedCity[prevCityId].Model.DecorationGroupDirection = DecorationGroupDirection.Left;
                    this.idToLoadedCity[prevCityId].SwitchPosition();
                }
            }
        }

        public async UniTask SwitchRight()
        {
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Left);
            this.PreviewCityId++;
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Center);

            if (this.PreviewCityId < this.decorationBlueprint.Count)
            {
                var nextCityId = this.PreviewCityId + 1;
                if (!this.idToLoadedCity.ContainsKey(nextCityId))
                {
                    await this.LoadDecorationGroupAsync(this.GetDecorationGroupModel(nextCityId, DecorationGroupDirection.Right, true));
                }
                else
                {
                    this.SetDecorationGroupDirection(nextCityId, DecorationGroupDirection.Right);
                }
            }
        }

        public void ExitPreviewMode()
        {
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Right);
            this.PreviewCityId = this.CurrentDecorationGroup.Model.Id;
            this.SetDecorationGroupDirection(this.PreviewCityId, DecorationGroupDirection.Center);
        }

        private void SetDecorationGroupDirection(int decorationGroupId, DecorationGroupDirection direction) { this.idToLoadedCity[decorationGroupId].Switch(direction); }

        #endregion

        public bool IsCompleteAllDecor()
        {
            var allCityData = this.decorationLocalDataController.GetAllCityData();
            if (allCityData.Count < this.decorationBlueprint.Count) return false;
            foreach (var (_, cityLocalData) in allCityData)
            {
                if (!cityLocalData.IsComplete) return false;
            }

            return true;
        }
    }
}