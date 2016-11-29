using Game.AdminClient.Infrastructure;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Game.AdminClient.ViewModels
{
    public class OpenGameViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IAdministrationServiceGateway _administrationService;
        private readonly IFileDialogService _dialogService;
        private readonly IMapService _mapService;
        private readonly IMessageBoxDialogService _messageBoxDialogService;

        private int _gameId;

        public OpenGameViewModel(
            IRegionManager regionManager, 
            IAdministrationServiceGateway administrationService, 
            IFileDialogService dialogService,
            IMapService mapService,
            IMessageBoxDialogService messageBoxDialogService)
        {
            _regionManager = regionManager;
            _administrationService = administrationService;
            _dialogService = dialogService;
            _mapService = mapService;
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
                        var game = await _administrationService.GetGameAsync(_gameId);
                        Game = new GameViewModel
                        {
                            GameId = game.GameId,
                            Label = game.Label,
                            State = game.State,
                            PlayerCollection =
                                new ObservableCollection<PlayerViewModel>(
                                    game.Players.Select(p => new PlayerViewModel
                                    {
                                        GameId = p.GameId,
                                        Name = p.Name,
                                        PlayerId = p.PlayerId,
                                        Team = p.Team,
                                    }))
                        };

                        var players = await _administrationService.ListPlayersAsync();
                        AvailablePlayerCollection =
                            new ObservableCollection<PlayerViewModel>(players.Select(p => new PlayerViewModel
                            {
                                GameId = p.GameId,
                                Name = p.Name,
                                PlayerId = p.PlayerId,
                                Team = p.Team,
                            }).Where(p => !p.GameId.HasValue));
                    }
                    catch (Exception e)
                    {
                        AutoRefreshOperation.IsAutoRefreshEnabled = false;
                        _regionManager.RequestNavigate("MainRegion", new Uri("LobbyView", UriKind.Relative));
                    }
                }, RefreshTime));
            }
        }

        private GameViewModel _game;
        public GameViewModel Game
        {
            get { return _game; }
            set { SetProperty(ref _game, value); }
        }
        
        private PlayerViewModel _selectedGamePlayer;
        public PlayerViewModel SelectedGamePlayer
        {
            get { return _selectedGamePlayer; }
            set { SetProperty(ref _selectedGamePlayer, value); }
        }

        private PlayerViewModel _selectedAvailablePlayer;
        public PlayerViewModel SelectedAvailablePlayer
        {
            get { return _selectedAvailablePlayer; }
            set { SetProperty(ref _selectedAvailablePlayer, value); }
        }

        private ObservableCollection<PlayerViewModel> _availablePlayerCollection;
        public ObservableCollection<PlayerViewModel> AvailablePlayerCollection
        {
            get { return _availablePlayerCollection; }
            set { SetProperty(ref _availablePlayerCollection, value); }
        }

        #endregion

        #region Commands

        private AsyncDelegateCommandWrapper _pickFileCommand;
        public AsyncDelegateCommandWrapper PickFileCommand
        {
            get
            {
                return _pickFileCommand ?? (_pickFileCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    string mapFilePath = _dialogService.OpenFileDialog();
                    if (mapFilePath == null)
                        return;

                    var map = await _mapService.LoadMapAsync(mapFilePath);

                    try
                    {
                        await _administrationService.SetMapAsync(Game.GameId, map);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }
        
        private AsyncDelegateCommandWrapper _addPlayerCommand;
        public AsyncDelegateCommandWrapper AddPlayerCommand
        {
            get
            {
                return _addPlayerCommand ?? (_addPlayerCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    if (SelectedAvailablePlayer == null)
                        return;

                    var selectedAvailablePlayer = SelectedAvailablePlayer;
                    SelectedAvailablePlayer = null;

                    try
                    {
                        await _administrationService.AddPlayerAsync(Game.GameId, selectedAvailablePlayer.PlayerId);

                        AvailablePlayerCollection.Remove(selectedAvailablePlayer);
                        Game.PlayerCollection.Add(selectedAvailablePlayer);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _removePlayerCommand;
        public AsyncDelegateCommandWrapper RemovePlayerCommand
        {
            get
            {
                return _removePlayerCommand ?? (_removePlayerCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    if (SelectedGamePlayer == null)
                        return;

                    var selectedGamePlayer = SelectedGamePlayer;
                    SelectedGamePlayer = null;

                    try
                    {
                        await _administrationService.RemovePlayerAsync(Game.GameId, selectedGamePlayer.PlayerId);

                        AvailablePlayerCollection.Add(selectedGamePlayer);
                        Game.PlayerCollection.Remove(selectedGamePlayer);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _backCommand;
        public AsyncDelegateCommandWrapper BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new AsyncDelegateCommandWrapper(() => _regionManager.RequestNavigate("MainRegion", new Uri("LobbyView", UriKind.Relative))));
            }
        }

        private AsyncDelegateCommandWrapper _startGameCommand;
        public AsyncDelegateCommandWrapper StartGameCommand
        {
            get
            {
                return _startGameCommand ?? (_startGameCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    try
                    {
                        await _administrationService.StartGameAsync(Game.GameId);

                        var parameters = new NavigationParameters { { "SelectedGameId", Game.GameId } };

                        _regionManager.RequestNavigate("MainRegion", new Uri("GamePreviewView", UriKind.Relative), parameters);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _swapPlayersCommand;
        public AsyncDelegateCommandWrapper SwapPlayersCommand
        {
            get
            {
                return _swapPlayersCommand ?? (_swapPlayersCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    try
                    {
                        if (Game.PlayerCollection?.Count != 2)
                            throw new ApplicationException("Number of players in the game must be 2");

                        SelectedAvailablePlayer = null;
                        SelectedGamePlayer = null;

                        var player1 = Game.PlayerCollection[0];
                        var player2 = Game.PlayerCollection[1];

                        // Remove
                        await _administrationService.RemovePlayerAsync(Game.GameId, player1.PlayerId);
                        AvailablePlayerCollection.Add(player1);
                        Game.PlayerCollection.Remove(player1);

                        await _administrationService.RemovePlayerAsync(Game.GameId, player2.PlayerId);
                        AvailablePlayerCollection.Add(player2);
                        Game.PlayerCollection.Remove(player2);

                        // Add
                        await _administrationService.AddPlayerAsync(Game.GameId, player2.PlayerId);
                        AvailablePlayerCollection.Remove(player2);
                        Game.PlayerCollection.Add(player2);

                        await _administrationService.AddPlayerAsync(Game.GameId, player1.PlayerId);
                        AvailablePlayerCollection.Remove(player1);
                        Game.PlayerCollection.Add(player1);
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
            _gameId = (int)navigationContext.Parameters["SelectedGameId"];

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