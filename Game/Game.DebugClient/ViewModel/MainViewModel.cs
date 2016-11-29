using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Game.DebugClient.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public ICommand InitLoginCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion", new Uri("InitLoginView", UriKind.Relative)));
            }
        }

        public ICommand CompleteLoginCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("CompleteLoginView", UriKind.Relative)));
            }
        }

        public ICommand CreatePlayerCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("CreatePlayerView", UriKind.Relative)));
            }
        }

        public ICommand GetPlayerViewCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("GetPlayerViewView", UriKind.Relative)));
            }
        }

        public ICommand GetTurnResultCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("GetTurnResultView", UriKind.Relative)));
            }
        }

        public ICommand LeaveGameCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion", new Uri("LeaveGameView", UriKind.Relative)));
            }
        }

        public ICommand PerformMoveCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion", new Uri("PerformMoveView", UriKind.Relative)));
            }
        }

        public ICommand WaitGameStartCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("WaitGameStartView", UriKind.Relative)));
            }
        }

        public ICommand WaitNextTurnCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("WaitNextTurnView", UriKind.Relative)));
            }
        }

        public ICommand CompleteLoginFlowCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("CompleteLoginFlowView", UriKind.Relative)));
            }
        }

        public ICommand CreatePlayerFlowCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("CreatePlayerFlowView", UriKind.Relative)));
            }
        }

        public ICommand WaitGameStartFlowCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("WaitGameStartFlowView", UriKind.Relative)));
            }
        }

        public ICommand PlayerModeFlowCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("PlayerModeFlowView", UriKind.Relative)));
            }
        }

        public ICommand WaitNextTurnLoopCommand
        {
            get
            {
                return
                    new DelegateCommand(
                        () =>
                        {
                            _regionManager.RequestNavigate("ContentRegion",
                                new Uri("WaitNextTurnLoopView", UriKind.Relative));
                        }
                        );
            }
        }
    }
}