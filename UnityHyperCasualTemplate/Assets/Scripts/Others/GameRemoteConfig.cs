namespace TheOneStudio.HyperCasual.Others
{
    using System;
    using ServiceImplementation.FireBaseRemoteConfig;
    using Zenject;

    // must bind in HyperCasualProjectContext
    public class GameRemoteConfig : IInitializable, IDisposable
    {
        #region Inject

        private readonly SignalBus     signalBus;
        private readonly IRemoteConfig remoteConfig;

        public GameRemoteConfig(SignalBus signalBus, IRemoteConfig remoteConfig)
        {
            this.signalBus    = signalBus;
            this.remoteConfig = remoteConfig;
        }

        #endregion

        #region Subscription

        public void Initialize() { this.signalBus.Subscribe<RemoteConfigFetchedSucceededSignal>(this.OnRemoteConfigFetchedSucceeded); }

        public void Dispose() { this.signalBus.Unsubscribe<RemoteConfigFetchedSucceededSignal>(this.OnRemoteConfigFetchedSucceeded); }

        #endregion

#if UNITY_IOS
        public string RemoteConfigVariable = "default_value_ios";
#else
        public string RemoteConfigVariable = "default_value_android";
#endif

        private void OnRemoteConfigFetchedSucceeded() { this.RemoteConfigVariable = this.remoteConfig.GetRemoteConfigStringValue("remote_config_key"); }
    }
}