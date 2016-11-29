using Autofac;
using Game.AdminClient.Infrastructure;
using Game.AdminClient.ViewModels;
using Game.AdminClient.Views;

namespace Game.AdminClient
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SettingsManager>().As<ISettingsManager>();
            builder.RegisterType<FileDialogService>().As<IFileDialogService>().InstancePerDependency();
            builder.RegisterType<ConfirmationDialogService>().As<IConfirmationDialogService>().InstancePerDependency();
            builder.RegisterType<MapService>().As<IMapService>().InstancePerDependency();
            builder.RegisterType<MessageBoxDialogService>().As<IMessageBoxDialogService>().InstancePerDependency();
            builder.RegisterType<AdministrationServiceGateway>().As<IAdministrationServiceGateway>().SingleInstance();

            builder.RegisterType<LoginViewModel>().AsSelf();
            builder.RegisterType<LobbyViewModel>().AsSelf();
            builder.RegisterType<OpenGameViewModel>().AsSelf();
            builder.RegisterType<GamePreviewViewModel>().AsSelf();

            builder.RegisterType<LoginView>().Named<object>("LoginView");
            builder.RegisterType<LobbyView>().Named<object>("LobbyView");
            builder.RegisterType<OpenGameView>().Named<object>("OpenGameView");
            builder.RegisterType<GamePreviewView>().Named<object>("GamePreviewView");
        }
    }
}
