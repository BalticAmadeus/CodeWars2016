using Game.AdminClient.AdminService;
using Game.AdminClient.Infrastructure;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using GameLogic;
using GameLogic.UserManagement;

namespace Game.AdminClient.ViewModels
{
    public class LobbyViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IAdministrationServiceGateway _administrationService;
        private readonly IConfirmationDialogService _confirmationDialogService;
        private readonly IMessageBoxDialogService _messageBoxDialogService;

        public LobbyViewModel(
            IAdministrationServiceGateway administrationService,
            IRegionManager regionManager,
            IConfirmationDialogService confirmationDialogService,
            IMessageBoxDialogService messageBoxDialogService)
        {
            _administrationService = administrationService;
            _regionManager = regionManager;
            _confirmationDialogService = confirmationDialogService;
            _messageBoxDialogService = messageBoxDialogService;
        }

        #region Properties

        private const int RefreshTime = 2000;
        private AutoRefreshOperation _autoRefreshOperation;
        public AutoRefreshOperation AutoRefreshOperation
        {
            get
            {
                return _autoRefreshOperation ?? (_autoRefreshOperation = new AutoRefreshOperation(async () =>
                {
                    try
                    {
                        var games = await _administrationService.ListGamesAsync();

                        GameCollection = new ObservableCollection<GameViewModel>(games.Select(p => new GameViewModel
                        {
                            GameId = p.GameId,
                            Label = p.Label,
                            State = p.State,
                            PlayerCollection =
                                new ObservableCollection<PlayerViewModel>(p.Players.Select(pl => new PlayerViewModel
                                {
                                    GameId = pl.GameId,
                                    Name = pl.Name,
                                    PlayerId = pl.PlayerId,
                                    Team = pl.Team,
                                }))
                        }).OrderByDescending(p => p.GameId));

                        if (UserSettings.AutoOpen)
                        {
                            var lastActiveGameId = GameCollection
                                .OrderByDescending(g => g.GameId)
                                        .Where(g => "Play".Equals(g.State, StringComparison.OrdinalIgnoreCase) || "Pause".Equals(g.State, StringComparison.OrdinalIgnoreCase))
                                        .Select(g => g.GameId)
                                        .FirstOrDefault();

                            if (lastActiveGameId > 0)
                            {
                                AutoRefreshOperation.Pause();

                                var parameters = new NavigationParameters { { "SelectedGameId", lastActiveGameId } };
                                _regionManager.RequestNavigate("MainRegion", new Uri("GamePreviewView", UriKind.Relative), parameters);
                                return;
                            }
                        }

                        if (SelectedGame != null)
                        {
                            var game = await _administrationService.GetGameAsync(SelectedGame.GameId);
                            SelectedGame.Label = game.Label;
                            SelectedGame.State = game.State;
                            SelectedGame.PlayerCollection =
                                new ObservableCollection<PlayerViewModel>(game.Players.Select(pl => new PlayerViewModel
                                {
                                    GameId = pl.GameId,
                                    Name = pl.Name,
                                    PlayerId = pl.PlayerId,
                                    Team = pl.Team,
                                }));
                        }
                    }
                    catch (Exception e)
                    {
                        //Relogin 
                        if (e.Message.StartsWith("No active session for this client"))
                        {
                            //AutoRefreshOperation.Pause();

                            try
                            {
                                _administrationService.LoginAsync().GetAwaiter();
                            }
                            catch { }

                            //AutoRefreshOperation.Resume();
                        }
                    }
                }, RefreshTime));
            }
        }

        private ObservableCollection<GameViewModel> _gameCollection;
        public ObservableCollection<GameViewModel> GameCollection
        {
            get { return _gameCollection; }
            set { SetProperty(ref _gameCollection, value); }
        }

        private GameViewModel _selectedGame;
        public GameViewModel SelectedGame
        {
            get { return _selectedGame; }
            set { SetProperty(ref _selectedGame, value); }
        }

        public bool _isAutoOpenEnabled;
        public bool IsAutoOpenEnabled
        {
            get
            {
                _isAutoOpenEnabled = UserSettings.AutoOpen;
                return _isAutoOpenEnabled;
            }
            set
            {
                UserSettings.AutoOpen = value;
                SetProperty(ref _isAutoOpenEnabled, UserSettings.AutoOpen);
            }
        }

        #endregion

        #region Visibility

        public bool ShowCreateGame
        {
            get
            {
                return UserSettings.Role == TeamRole.Normal || UserSettings.Role == TeamRole.Power;
            }
        }

        public bool ShowOpenGame
        {
            get
            {
                return true;
            }
        }

        public bool ShowDeleteGame
        {
            get
            {
                return UserSettings.Role == TeamRole.Normal || UserSettings.Role == TeamRole.Power;
            }
        }

        public bool ShowAutoOpen
        {
            get
            {
                return UserSettings.Role == TeamRole.Observer;
            }
        }

        #endregion

        #region Commands

        private AsyncDelegateCommandWrapper _createGameCommand;
        public AsyncDelegateCommandWrapper CreateGameCommand
        {
            get
            {
                return _createGameCommand ?? (_createGameCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    try
                    {
                        var game = await _administrationService.CreateGameAsync();

                        var parameters = new NavigationParameters { { "SelectedGameId", game.GameId } };
                        _regionManager.RequestNavigate("MainRegion", new Uri("OpenGameView", UriKind.Relative), parameters);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _openGameCommand;
        public AsyncDelegateCommandWrapper OpenGameCommand
        {
            get
            {
                return _openGameCommand ?? (_openGameCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    try
                    {
                        if (SelectedGame == null)
                            return;

                        var game = await _administrationService.GetGameAsync(SelectedGame.GameId);

                        var parameters = new NavigationParameters { { "SelectedGameId", SelectedGame.GameId } };

                        if ("SETUP".Equals(game.State, StringComparison.OrdinalIgnoreCase))
                        {
                            if (UserSettings.Role == TeamRole.Observer)
                            {
                                _messageBoxDialogService.OpenDialog("Game is in setup state.", "Please wait");
                                return;
                            }

                            _regionManager.RequestNavigate("MainRegion", new Uri("OpenGameView", UriKind.Relative), parameters);
                            return;
                        }

                        _regionManager.RequestNavigate("MainRegion", new Uri("GamePreviewView", UriKind.Relative), parameters);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _deleteGameCommand;
        public AsyncDelegateCommandWrapper DeleteGameCommand
        {
            get
            {
                return _deleteGameCommand ?? (_deleteGameCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    if (SelectedGame == null)
                        return;

                    string message = string.Format("Are you sure to delete game {0}?", SelectedGame.Label);
                    bool result = _confirmationDialogService.OpenDialog("Delete Game", message);
                    if (!result)
                        return;

                    try
                    {
                        await _administrationService.DeleteGameAsync(SelectedGame.GameId);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        #endregion

        #region Navigation

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AutoRefreshOperation.Resume();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            AutoRefreshOperation.Pause();
        }

        #endregion
    }
}
