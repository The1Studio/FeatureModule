namespace TheOneStudio.HyperCasual.GameModules.RemoveAdsSuggestion.Scripts
{
    using System;
    using System.Linq;
    using Zenject;

    // Need to install this before using RemoveAdsSuggestionService
    public class RemoveAdsSuggestionInstaller : Installer<RemoveAdsSuggestionInstaller>
    {
        public override void InstallBindings()
        {
            var baseType = typeof(RemoveAdsSuggestionService);
            var type = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(asm => !asm.IsDynamic)
                                .SelectMany(asm => asm.GetTypes())
                                .Where(type => !type.IsAbstract && baseType.IsAssignableFrom(type))
                                .Single();
            this.Container.BindInterfacesAndSelfTo(type).AsSingle();
        }
    }
}