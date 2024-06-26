namespace TheOneStudio.GameFeature.Decoration.Installers
{
    using TheOneStudio.GameFeature.Decoration.Elements;
    using TheOneStudio.GameFeature.Decoration.Managers;
    using Zenject;

    public class DecorationInstaller : Installer<DecorationView,DecorationInstaller>
    {
        private readonly DecorationView decorationView;

        public DecorationInstaller(DecorationView decorationView) { this.decorationView = decorationView; }
        
        public override void InstallBindings()
        {
            this.Container.Bind<DecorationManager>().AsCached();
            this.Container.BindFactory<DecorationGroupModel, DecorationGroupPresenter, DecorationGroupPresenter.Factory>().AsCached();
            this.Container.Bind<DecorationView>().FromInstance(this.decorationView).AsCached();
            this.Container.BindInterfacesAndSelfTo<DecorationConfig>().FromScriptableObjectResource(nameof(DecorationConfig)).AsSingle();
        }
    }
}