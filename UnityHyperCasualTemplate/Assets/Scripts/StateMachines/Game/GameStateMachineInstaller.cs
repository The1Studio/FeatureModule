namespace TheOneStudio.HyperCasual.StateMachines.Game
{
    using TheOneStudio.HyperCasual.StateMachines.Game.Interfaces;
    using Zenject;

    public class GameStateMachineInstaller : Installer<GameStateMachineInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();
            this.Container.Bind<IGameState>().To(convention => convention.AllNonAbstractClasses().DerivingFrom<IGameState>()).WhenInjectedInto<GameStateMachine>();
        }
    }
}