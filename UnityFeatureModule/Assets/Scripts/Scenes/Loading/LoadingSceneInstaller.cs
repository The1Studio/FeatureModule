namespace TheOneStudio.HyperCasual.Scenes.Loading
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;

    public class LoadingSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.InitScreenManually<LoadingScenePresenter>();
        }
    }
}