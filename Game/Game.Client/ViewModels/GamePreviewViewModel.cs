using Game.AdminClient.AdminService;
using Game.AdminClient.Infrastructure;
using Game.AdminClient.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GameLogic.UserManagement;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Game.AdminClient.ViewModels
{
    public class GamePreviewViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IAdministrationServiceGateway _administrationService;
        private readonly IMessageBoxDialogService _messageBoxDialogService;
        private readonly IConfirmationDialogService _confirmationDialogService;

        private int _gameId;

        public GamePreviewViewModel(
            IRegionManager regionManager,
            IAdministrationServiceGateway administrationService,
            IMessageBoxDialogService messageBoxDialogService,
            IConfirmationDialogService confirmationDialogService)
        {
            _regionManager = regionManager;
            _administrationService = administrationService;
            _messageBoxDialogService = messageBoxDialogService;
            _confirmationDialogService = confirmationDialogService;
        }

        #region Properties

        private readonly object _lock = new object();
        private bool _canEnter = true;

        private int _mapWidth;
        private const int RefreshTime = 100;
        private AutoRefreshOperation _autoRefreshOperation;

        public AutoRefreshOperation AutoRefreshOperation
        {
            get
            {
                return _autoRefreshOperation ?? (_autoRefreshOperation = new AutoRefreshOperation(async () =>
                {
                    lock (_lock)
                    {
                        if (!_canEnter)
                            return;

                        _canEnter = !_canEnter;
                    }

                    RequestCount++;

                    try
                    {
                        if (Game == null || Game.GameId != _gameId)
                        {
                            var game = await _administrationService.GetGameAsync(_gameId);
                         
                            Game = new GameViewModel
                            {
                                GameId = game.GameId,
                                Label = game.Label,
                                State = game.State,
                                PlayerCollection = new ObservableCollection<PlayerViewModel>(game.Players.Select(p => new PlayerViewModel
                                {
                                    GameId = p.GameId,
                                    Name = p.Name,
                                    PlayerId = p.PlayerId,
                                    Team = p.Team
                                }))
                            };

                            await ResetMap();

                            IsResumeEnabled = true;
                            IsPauseEnabled = false;
                        }
                        

                        var turn = await _administrationService.GetNextTurnAsync(Game.GameId);
                        LastCallTime = _administrationService.LastCallTime;

                        TurnQueueSize = turn.NumberOfQueuedTurns;

                        switch (turn.Game.State.ToUpper())
                        {
                            case "FINISH":
                                IsPauseEnabled = false;
                                IsResumeEnabled = false;
                                break;
                            case "PAUSE":
                                IsPauseEnabled = false;
                                IsResumeEnabled = true;
                                break;
                            case "PLAY":
                                IsPauseEnabled = true;
                                IsResumeEnabled = false;
                                break;
                        }

                        if (turn.TurnNumber == -1 && turn.NumberOfQueuedTurns == -1)
                        {
                            await ResetMap();
                        }
                        else if (turn.TurnNumber != -1)
                        {
                            TurnNumber = turn.TurnNumber;

                            ChangeMap(turn.MapChanges, turn.TecmanPosition, turn.GhostPositions);

                            var playerCollection = new ObservableCollection<PlayerStateViewModel>();

                            for (int i = 0; i < Game.PlayerCollection.Count; i++)
                            {
                                playerCollection.Add(new PlayerStateViewModel
                                {
                                    ColorId = i,
                                    Condition = turn.PlayerStates[i].Condition,
                                    Player = Game.PlayerCollection[i],
                                    Score = turn.PlayerStates[i].Score,
                                });
                            }

                            PlayerCollection = playerCollection;
                        }
                        else
                        {
                            // Observer call timeout, we can do more things here
                            if (UserSettings.AutoOpen)
                            {
                                var games = await _administrationService.ListGamesAsync();
                                var lastActiveGameId = games
                                            .OrderByDescending(g => g.GameId)
                                            .Where(g => "Play".Equals(g.State, StringComparison.OrdinalIgnoreCase) || "Pause".Equals(g.State, StringComparison.OrdinalIgnoreCase))
                                            .Select(g => g.GameId)
                                            .FirstOrDefault();

                                if (lastActiveGameId > 0 && lastActiveGameId != _gameId)
                                {
                                    // This will force game switch on next timer tick
                                    _gameId = lastActiveGameId;
                                }
                            }
                        }

                        for (int i = 0; i < PlayerCollection.Count; i++)
                        {
                            PlayerCollection[i].SlowTurn = turn.SlowPlayers.Contains(i);
                        }
                    }
                    catch (Exception e)
                    {
                        AutoRefreshOperation.IsAutoRefreshEnabled = false;
                        _regionManager.RequestNavigate("MainRegion", new Uri("LobbyView", UriKind.Relative));
                        //_messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }

                    lock (_lock)
                    {
                        _canEnter = !_canEnter;
                    }
                }, RefreshTime));
            }
        }

        private void ChangeMap(IList<MapChange> mapChanges, Point tecmanPosition, IList<Point> ghostPositions)
        {
            // Detect cookie changes
            foreach (var mapChange in mapChanges)
            {
                CookieSprite cookie;
                if (!_cookieSprites.TryGetValue(mapChange.Position, out cookie))
                    continue;
                if (mapChange.Value.Contains("."))
                    continue;
                cookie.Eat();
            }

            // Tecman position
            _tecmanSprite.Move(tecmanPosition);

            // Ghost positions
            for (int i = 0; i < ghostPositions.Count; i++)
            {
                _ghostSprites[i].Move(ghostPositions[i]);
            }
        }

        private async Task ResetMap()
        {
            MapResetCount++;

            var match = await _administrationService.GetMatchAsync(Game.GameId);
            _mapWidth = match.Map.Width;


            // Map background & cookies
            var geoWalls = new GeometryGroup();
            var dwCookies = new DrawingGroup();
            _cookieSprites = new Dictionary<Point, CookieSprite>();

            for (int i = 0; i < match.Map.Rows.Count; i++)
            {
                for (int j = 0; j < match.Map.Rows[i].Length; j++)
                {
                    switch (match.Map.Rows[i][j])
                    {
                        case '#':
                            geoWalls.Children.Add(new RectangleGeometry(new System.Windows.Rect(j, i, 1, 1)));
                            break;
                        case '.':
                            var cookie = new CookieSprite(j, i);
                            dwCookies.Children.Add(cookie.SpriteDrawing);
                            _cookieSprites[new Point(j, i)] = cookie;
                            break;
                    }
                }
            }

            var dwWalls = new GeometryDrawing(Brushes.Blue, null, geoWalls);

            // Tecman
            _tecmanSprite = new TecmanSprite(match.Map.TecmanPosition);

            // Ghosts
            _ghostSprites = match.Map.GhostPositions.Select((p, i) => new GhostSprite(p, i)).ToArray();

            var bgDrawing = new DrawingGroup();
            bgDrawing.Children.Add(dwWalls);
            bgDrawing.Children.Add(dwCookies);
            bgDrawing.Children.Add(_tecmanSprite.SpriteDrawing);
            foreach (GhostSprite ghost in _ghostSprites)
            {
                bgDrawing.Children.Add(ghost.SpriteDrawing);
            }

            var bgImage = new DrawingImage(bgDrawing);

            BackgroundImage = bgImage;


            // Player stats

            var playerCollection = new ObservableCollection<PlayerStateViewModel>();
            for (int i = 0; i < Game.PlayerCollection.Count; i++)
            {
                playerCollection.Add(new PlayerStateViewModel
                {
                    ColorId = i,
                    Condition = match.PlayerStates[i].Condition,
                    Player = Game.PlayerCollection[i],
                    Score = match.PlayerStates[i].Score
                });
            }

            PlayerCollection = playerCollection;
        }

        private GameViewModel _game;
        public GameViewModel Game
        {
            get { return _game; }
            set { SetProperty(ref _game, value); }
        }

        private PlayerStateViewModel _selectedPlayer;
        public PlayerStateViewModel SelectedPlayer
        {
            get { return _selectedPlayer; }
            set { SetProperty(ref _selectedPlayer, value); }
        }

        private ObservableCollection<PlayerStateViewModel> _playerCollection;
        public ObservableCollection<PlayerStateViewModel> PlayerCollection
        {
            get { return _playerCollection; }
            set { SetProperty(ref _playerCollection, value); }
        }

        private int _mapResetCount;
        public int MapResetCount
        {
            get { return _mapResetCount; }
            set { SetProperty(ref _mapResetCount, value); }
        }

        private int _turnNumber;
        public int TurnNumber
        {
            get { return _turnNumber; }
            set { SetProperty(ref _turnNumber, value); }
        }

        private double _turnQueueSize;
        public double TurnQueueSize
        {
            get { return _turnQueueSize; }
            set { SetProperty(ref _turnQueueSize, value); }
        }

        private double _requestCount;
        public double RequestCount
        {
            get { return _requestCount; }
            set { SetProperty(ref _requestCount, value); }
        }

        private bool _isPauseEnabled;
        public bool IsPauseEnabled
        {
            get { return _isPauseEnabled; }
            set { SetProperty(ref _isPauseEnabled, value); }
        }

        private bool _isResumeEnabled;
        public bool IsResumeEnabled
        {
            get { return _isResumeEnabled; }
            set { SetProperty(ref _isResumeEnabled, value); }
        }

        private double _canvasWidth;
        public double CanvasWidth
        {
            get { return _canvasWidth; }
            set { SetProperty(ref _canvasWidth, value); }
        }

        private double _canvasHeight;
        public double CanvasHeight
        {
            get { return _canvasHeight; }
            set { SetProperty(ref _canvasHeight, value); }
        }

        private double _lastCallTime;
        public double LastCallTime
        {
            get { return _lastCallTime; }
            set { SetProperty(ref _lastCallTime, value); }
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

        //public System.Windows.Controls.Canvas PlayFieldCanvas { get; set; }

        private ImageSource _backgroundImage;
        public ImageSource BackgroundImage
        {
            get { return _backgroundImage; }
            set { SetProperty(ref _backgroundImage, value); }
        }

        private Dictionary<Point, CookieSprite> _cookieSprites;
        private TecmanSprite _tecmanSprite;
        private GhostSprite[] _ghostSprites;

        #endregion

        #region Visibility

        public bool ShowResumeGame
        {
            get
            {
                return UserSettings.Role == TeamRole.Normal || UserSettings.Role == TeamRole.Power;
            }
        }

        public bool ShowPauseGame
        {
            get
            {
                return UserSettings.Role == TeamRole.Normal || UserSettings.Role == TeamRole.Power;
            }
        }

        public bool ShowDropPlayer
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

        private AsyncDelegateCommandWrapper _closeCommand;
        public AsyncDelegateCommandWrapper CloseCommand
        {
            get
            {

                return _closeCommand ?? (_closeCommand = new AsyncDelegateCommandWrapper(() =>
                {
                    UserSettings.AutoOpen = false;
                    _regionManager.RequestNavigate("MainRegion", new Uri("LobbyView", UriKind.Relative));
                }));
            }
        }

        private AsyncDelegateCommandWrapper _pauseGameCommand;
        public AsyncDelegateCommandWrapper PauseGameCommand
        {
            get
            {
                return _pauseGameCommand ?? (_pauseGameCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    if (!IsPauseEnabled)
                        return;

                    IsPauseEnabled = false;
                    IsResumeEnabled = true;

                    try
                    {
                        await _administrationService.PauseGameAsync(Game.GameId);
                    }
                    catch (Exception e)
                    {
                        IsResumeEnabled = false;
                        IsPauseEnabled = true;

                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _resumeGameCommand;
        public AsyncDelegateCommandWrapper ResumeGameCommand
        {
            get
            {
                return _resumeGameCommand ?? (_resumeGameCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    if (!IsResumeEnabled)
                        return;

                    IsResumeEnabled = false;
                    IsPauseEnabled = true;

                    try
                    {
                        await _administrationService.ResumeGameAsync(Game.GameId);
                    }
                    catch (Exception e)
                    {
                        IsPauseEnabled = false;
                        IsResumeEnabled = true;

                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _dropPlayerCommand;
        public AsyncDelegateCommandWrapper DropPlayerCommand
        {
            get
            {
                return _dropPlayerCommand ?? (_dropPlayerCommand = new AsyncDelegateCommandWrapper(async () =>
                {
                    if (SelectedPlayer == null)
                        return;

                    string message = string.Format("Are you sure to drop player {0} {1}?", SelectedPlayer.Player.Team, SelectedPlayer.Player.Name);
                    bool result = _confirmationDialogService.OpenDialog("Drop Player", message);
                    if (!result)
                        return;

                    var selectedPlayer = SelectedPlayer;
                    SelectedPlayer = null;

                    try
                    {
                        await _administrationService.DropPlayer(Game.GameId, selectedPlayer.Player.PlayerId);
                    }
                    catch (Exception e)
                    {
                        _messageBoxDialogService.OpenDialog(e.Message, "Error");
                    }
                }));
            }
        }

        private AsyncDelegateCommandWrapper _showInfoComamnd;
        public AsyncDelegateCommandWrapper ShowInfoCommand
        {
            get
            {
                return _showInfoComamnd ?? (_showInfoComamnd = new AsyncDelegateCommandWrapper(async () =>
                {
                    var response = await _administrationService.GetLiveInfo(Game.GameId);
                    string message;

                    using (var stringStream = new StringWriter())
                    {
                        var requestSerializer = new XmlSerializer(typeof(GetLiveInfoResp));
                        requestSerializer.Serialize(stringStream, response);
                        message = stringStream.ToString();
                    }

                    _messageBoxDialogService.OpenDialog(message, "Info");
                }));
            }
        }

        #endregion

        #region Navigation

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _gameId = (int)navigationContext.Parameters["SelectedGameId"];

            TurnQueueSize = 0;

            AutoRefreshOperation.Resume();

            TurnNumber = 0;
            MapResetCount = 0;

            RequestCount = 0;
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
