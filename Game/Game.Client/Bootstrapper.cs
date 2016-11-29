using System;
using System.Windows;
using Autofac;
using Prism.Autofac;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Game.AdminClient
{
    class Bootstrapper : AutofacBootstrapper, IDisposable
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();

            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("MainRegion", () => Container.ResolveNamed<object>("LoginView"));
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new TextLogger();
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);

            builder.RegisterModule<Module>();
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
