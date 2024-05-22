namespace TheOneStudio.HyperCasual.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using TheOneStudio.HyperCasual.StateMachines.Game;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;

    public class MainSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            if (this.Container.Resolve<GameFeaturesSetting>().enableInitHomeScreenManually)
            {
                this.Container.InitScreenManually<HomeSimpleScreenPresenter>();
            }
            
            GameStateMachineInstaller.Install(this.Container);
        }
    }
}