using System.Windows;

namespace Game.AdminClient
{
    public partial class App : Application
    {
        private readonly Bootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new Bootstrapper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bootstrapper.Dispose();

            base.OnExit(e);
        }
    }
}
