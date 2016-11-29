using System.Collections.Generic;
using System.Windows;
using Autofac;
using Game.DebugClient.Utilites;
using Prism.Autofac;
using Prism.Logging;
using Prism.Regions;

namespace Game.DebugClient
{
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window) Shell;
            Application.Current.MainWindow.Show();

            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("MainRegion", () => Container.ResolveNamed<object>("MainView"));
            regionManager.RegisterViewWithRegion("AuthRegion", () => Container.ResolveNamed<object>("CommonDataView"));
            regionManager.RegisterViewWithRegion("LoggerRegion", () => Container.ResolveNamed<object>("LoggerView"));

            regionManager.RegisterViewWithRegion("ContentRegion", () => Container.ResolveNamed<object>("EmptyView"));
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new CompositeLogger(new List<ILoggerFacade>
            {
                //new TextLogger(),
                new FileLogger()
            });
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