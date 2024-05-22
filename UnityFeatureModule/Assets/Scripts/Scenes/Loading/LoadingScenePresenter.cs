namespace TheOneStudio.HyperCasual.Scenes.Loading
{
    using BlueprintFlow.BlueprintControlFlow;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Loading;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using Zenject;

    [ScreenInfo("LoadingScreenView")]
    public class LoadingScenePresenter : UITemplateLoadingScreenPresenter
    {
        protected LoadingScenePresenter(SignalBus signalBus, UITemplateAdServiceWrapper adService, BlueprintReaderManager blueprintManager, UserDataManager userDataManager, IGameAssets gameAssets, ObjectPoolManager objectPoolManager, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper) : base(signalBus, adService, blueprintManager, userDataManager, gameAssets, objectPoolManager, uiTemplateAdServiceWrapper)
        {
        }
    }
}