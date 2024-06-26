namespace TheOneStudio.GameFeature.Decoration
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.GameFeature.Decoration.Blueprints;
    using TheOneStudio.GameFeature.Decoration.Elements;
    using TheOneStudio.GameFeature.Decoration.LocalData.Controllers;
    using UnityEngine;
    using UnityEngine.Serialization;
    using Zenject;

    public class DecorationGroupModel
    {
        public int                      Id;
        public string                   CityName;
        public string                   AddressableName;
        public Transform                Parent;
        public Vector3                  Position;
        public Vector3                  Rotation;
        public Vector3                  Scale;
        public int                      CurrentBuildingIndex;
        public Dictionary<int, int>     IndexToCostPaid;
        public DecorationGroupDirection DecorationGroupDirection = DecorationGroupDirection.Center;
    }

    public class DecorationGroupView : MonoBehaviour
    {
        public IReadOnlyList<DecorationItemView> DecorationItems { get; private set; }

        public void Awake() { this.DecorationItems = this.GetComponentsInChildren<DecorationItemView>(); }
    }

    public class DecorationGroupPresenter
    {
        public DecorationGroupModel Model;
        public DecorationGroupView  View;

        private int                  currentBuildingIndex;
        private Dictionary<int, int> indexToCostPaid;
        private GameObject           cityParent;
        private Tween                switchTween;

        private const string CityParentPrefabName = "city_parent";

        private readonly ObjectPoolManager             objectPoolManager;
        private readonly DecorationBlueprint           decorationBlueprint;
        private readonly DecorationLocalDataController decorationLocalDataController;
        private readonly DecorationConfig              decorationConfig;

        public DecorationGroupPresenter(ObjectPoolManager objectPoolManager,
                                        DecorationBlueprint decorationBlueprint,
                                        DecorationLocalDataController decorationLocalDataController,
                                        DecorationConfig decorationConfig)
        {
            this.objectPoolManager             = objectPoolManager;
            this.decorationBlueprint           = decorationBlueprint;
            this.decorationLocalDataController = decorationLocalDataController;
            this.decorationConfig              = decorationConfig;
        }

        public RectTransform Attractor => this.View.DecorationItems[this.currentBuildingIndex].Attractor;

        private void BindData(DecorationGroupModel model)
        {
            this.Model                = model;
            this.currentBuildingIndex = model.CurrentBuildingIndex;
            this.indexToCostPaid      = model.IndexToCostPaid;
        }

        public async UniTask InitView()
        {
            this.cityParent = await this.objectPoolManager.Spawn(CityParentPrefabName);
            var viewObj = await this.objectPoolManager.Spawn(this.Model.AddressableName, this.cityParent.transform);
            this.View = viewObj.GetComponent<DecorationGroupView>();

            var position = this.GetPosition();

            var viewTransform = this.cityParent.transform;
            viewTransform.position    = position;
            viewTransform.eulerAngles = this.Model.Rotation;
            viewTransform.localScale  = this.Model.Scale;

            this.cityParent.transform.SetParent(this.Model.Parent);

            this.LoadBuildingProgress();
        }

        public void SwitchPosition()
        {
            var position = this.GetPosition();
            this.cityParent.transform.position = position;
        }

        #region Preview mode

        public void Switch(DecorationGroupDirection direction)
        {
            this.Model.DecorationGroupDirection = direction;
            this.SwitchAnim();
        }

        private void SwitchAnim()
        {
            var position = this.GetPosition();
            this.switchTween?.Kill();
            this.switchTween = this.cityParent.transform.DOMove(position, .55f);
        }

        #endregion

        private Vector3 GetPosition()
        {
            var position = this.Model.Position;
            switch (this.Model.DecorationGroupDirection)
            {
                case DecorationGroupDirection.Left:
                    position -= Vector3.right * 15;
                    break;
                case DecorationGroupDirection.Right:
                    position += Vector3.right * 15;
                    break;
            }

            return position;
        }

        private void LoadBuildingProgress()
        {
            for (var i = 0; i < this.View.DecorationItems.Count; i++)
            {
                var building = this.View.DecorationItems[i];
                building.gameObject.SetActive(i <= this.currentBuildingIndex);

                var record   = this.decorationBlueprint[this.Model.Id];
                var cost     = record.BuildingRecords[i].Cost;
                var costPaid = this.indexToCostPaid[i];
                var costLeft = cost - costPaid;
                building.SetProgress((float)costPaid / cost, costLeft);

                if (i == this.currentBuildingIndex)
                {
                    building.BindProgress(this.GetCostLeft(), this.decorationConfig.CameraRotation);
                    if (this.Model.DecorationGroupDirection == DecorationGroupDirection.Center &&
                        !this.decorationLocalDataController.GetCityData(this.Model.Id).IsComplete)
                    {
                        building.ShowCost();
                    }
                    else
                    {
                        building.HideCost();
                    }
                }

                else
                {
                    building.HideCost();
                }
            }
        }

        public int GetCostLeft()
        {
            var record   = this.decorationBlueprint[this.Model.Id];
            var cost     = record.BuildingRecords[this.currentBuildingIndex].Cost;
            var costPaid = this.indexToCostPaid[this.currentBuildingIndex];
            return cost - costPaid;
        }

        public void Build(int costSpend)
        {
            var currentCostPaid = this.indexToCostPaid[this.currentBuildingIndex];
            currentCostPaid                                 += costSpend;
            this.indexToCostPaid[this.currentBuildingIndex] =  currentCostPaid;
        }

        public void DoBuildProgress()
        {
            var currentCostPaid = this.indexToCostPaid[this.currentBuildingIndex];
            var cost            = this.decorationBlueprint[this.Model.Id].BuildingRecords[this.currentBuildingIndex].Cost;
            this.View.DecorationItems[this.currentBuildingIndex].DoBuildProgress((float)currentCostPaid / cost, cost - currentCostPaid);
        }

        public void ScaleBuildingAnim() { this.View.DecorationItems[this.currentBuildingIndex].ScaleAnim(); }

        public void ShowCurrentBuildCost() { this.View.DecorationItems[this.currentBuildingIndex].ShowCost(); }

        public void UnlockNextBuilding()
        {
            this.View.DecorationItems[this.currentBuildingIndex].HideCost();

            this.currentBuildingIndex++;

            if (this.currentBuildingIndex == this.View.DecorationItems.Count) return;
            this.View.DecorationItems[this.currentBuildingIndex].BindProgress(this.GetCostLeft(), this.decorationConfig.CameraRotation);
            this.View.DecorationItems[this.currentBuildingIndex].gameObject.SetActive(true);
            this.View.DecorationItems[this.currentBuildingIndex].ShowCost();
        }

        public void BuildCityComplete() { this.View.DecorationItems[this.currentBuildingIndex].HideCost(); }

        public class Factory : PlaceholderFactory<DecorationGroupModel, DecorationGroupPresenter>
        {
            private readonly DiContainer diContainer;

            public Factory(DiContainer diContainer) { this.diContainer = diContainer; }

            public override DecorationGroupPresenter Create(DecorationGroupModel param)
            {
                var city = this.diContainer.Instantiate<DecorationGroupPresenter>();
                city.BindData(param);
                return city;
            }
        }
    }

    public enum DecorationGroupDirection
    {
        Left,
        Center,
        Right
    }
}