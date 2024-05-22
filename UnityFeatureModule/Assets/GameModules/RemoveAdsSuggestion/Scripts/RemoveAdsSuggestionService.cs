namespace TheOneStudio.HyperCasual.RemoveAdsSuggestion.Scripts
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities.GameQueueAction;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using Zenject;

    public abstract class RemoveAdsSuggestionService : IInitializable, ITickable
    {
        #region inject

        protected readonly UITemplateAdsController             uiTemplateAdsController;
        protected readonly SignalBus                           signalBus;
        protected readonly IScreenManager                      screenManager;
        protected readonly IRemoteConfig                       remoteConfig;
        protected readonly GameQueueActionContext              gameQueueActionContext;
        protected readonly UITemplateGameSessionDataController gameSessionDataController;

        public RemoveAdsSuggestionService(UITemplateAdsController             uiTemplateAdsController, SignalBus              signalBus, IScreenManager screenManager,
                                          IRemoteConfig                       remoteConfig,            GameQueueActionContext gameQueueActionContext,
                                          UITemplateGameSessionDataController gameSessionDataController)
        {
            this.uiTemplateAdsController   = uiTemplateAdsController;
            this.signalBus                 = signalBus;
            this.screenManager             = screenManager;
            this.remoteConfig              = remoteConfig;
            this.gameQueueActionContext    = gameQueueActionContext;
            this.gameSessionDataController = gameSessionDataController;
        }

        #endregion

        protected Dictionary<string, RemoveAdsParamConfig> RemoveAdsActionToConfig = new();
        private   float                                    totalPlayingTime;

        public void Initialize()
        {
            this.signalBus.Subscribe<RemoteConfigFetchedSucceededSignal>(this.OnRemoteConfigFetchedSucceededSignal);
            this.InitRemoveAdsSuggestAction();
        }

        // initialize when remote config doesn't fetch yet
        protected abstract void InitRemoveAdsSuggestAction();

        // use this function when you want to open remove ads popup with queue
        public virtual void SuggestRemoveAdsPopupWithQueue<TView>() where TView : BaseView { }

        // use this function when you want to open remove ads popup without any queue
        public abstract void SuggestRemoveAdsPopup();

        // must be init before fetch from remote config
        protected abstract void FetchRemoveAdsConfig();
        public             void Tick() { this.totalPlayingTime += Time.unscaledDeltaTime; }

        private void OnRemoteConfigFetchedSucceededSignal(RemoteConfigFetchedSucceededSignal obj) { this.FetchRemoveAdsConfig(); }

        public bool IsSuggestRemoveAdsEligible(string action)
        {
            if (!this.RemoveAdsActionToConfig.TryGetValue(action, out var config))
            {
                Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: False, action {action} is not registered");
                return false;
            }

            Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: Checking remove ads eligibility {action}.");
            if (this.gameSessionDataController.OpenTime < config.EnableAfterSession)
            {
                Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: False, Suggestions are disabled for this session: {this.gameSessionDataController.OpenTime}," +
                          $"required session: {config.EnableAfterSession}");
                return false;
            }

            if (config.EnableAfterSession == -1)
            {
                Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: False, Suggestions are disabled for this action: {action}");
                return false;
            }

            if (this.totalPlayingTime < config.SuggestCappingTime)
            {
                Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: False, Not enough playing time: {this.totalPlayingTime}, " +
                          $"capping time: {config.SuggestCappingTime}");
                return false;
            }

            if (this.uiTemplateAdsController.WatchInterstitialAds == 0)
            {
                Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: False, No interstitial ads watched yet.");
                return false;
            }

            var isAdWatchedAtRightTime = this.uiTemplateAdsController.WatchInterstitialAds == config.FirstEnableAfterInterAds ||
                                         this.uiTemplateAdsController.WatchInterstitialAds % config.InterAdsInterval == 0;
            if (!isAdWatchedAtRightTime)
            {
                Debug.Log($"oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: False, Not suggesting remove ads." +
                          $"Interstitial ads watched: {this.uiTemplateAdsController.WatchInterstitialAds}, " +
                          $"FirstEnableAfterInterAds: {config.FirstEnableAfterInterAds}, " +
                          $"InterAdsInterval: {config.InterAdsInterval}");
                return false;
            }

            Debug.Log("oneLog: RemoveAdsService.IsSuggestRemoveAdsEligible: True, Suggesting remove ads.");
            this.totalPlayingTime = 0;
            return true;
        }
    }
}