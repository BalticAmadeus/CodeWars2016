using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Infrastructure;
using Game.DebugClient.Infrastructure;
using Prism.Commands;
using Game.ClientCommon;

namespace Game.DebugClient.ViewModel.Flows
{
    public class PlayerModeFlowViewModel : ServiceCallViewModel
    {
        private readonly IMapService _mapService;
        private readonly IMessageBoxDialogService _messageBoxDialogService;
        private ObservableCollection<CellViewModel> _cellCollection;
        private ObservableCollection<PlayerMoveModel> _moveCollection;
        private bool _isExecuting;
        private bool _canSubmit;
        private int _playerId;
        private int _turn;
        private string _positions;
        private CellViewModel _selectedCell;
        private PlayerMoveModel _selectedActor;

        public PlayerModeFlowViewModel(
            ICommonDataManager commonDataManager,
            IServiceCallInvoker serviceCallInvoker,
            IMapService mapService,
            IMessageBoxDialogService messageBoxDialogService) : base(commonDataManager, serviceCallInvoker)
        {
            _mapService = mapService;
            _messageBoxDialogService = messageBoxDialogService;

            PlayerId = CommonDataManager.PlayerId;
            Turn = CommonDataManager.Turn;
            CanSubmit = (Turn > 0);

            CommonDataManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "PlayerId")
                    PlayerId = CommonDataManager.PlayerId;

                if (args.PropertyName == "Turn")
                    Turn = CommonDataManager.Turn;
            };

            _mapService.MapChanged += (sender, args) =>
            {
                var cellViewModels = new List<CellViewModel>();

                for (int row = 0; row < _mapService.Height; row++)
                {
                    for (int col = 0; col < _mapService.Width; col++)
                    {
                        cellViewModels.Add(new CellViewModel
                        {
                            X = col,
                            Y = row,
                            State = _mapService.Map[row, col]
                        });
                    }
                }

                CellCollection = new ObservableCollection<CellViewModel>(cellViewModels);
                MoveCollection = new ObservableCollection<PlayerMoveModel>(_mapService.Actors.Select(a => new PlayerMoveModel
                {
                    Index = a.Index,
                    Name = a.Name,
                    Move = a.Position,
                    IsTecman = a.IsTecman
                }));
            };

            _mapService.CellChanged += (sender, args) =>
            {
                var index = args.Y * _mapService.Width + args.X;
                CellCollection[index].State = args.State;
            };

            if (_mapService.Map != null)
            {
                var cellViewModels = new List<CellViewModel>();

                for (int row = 0; row < _mapService.Height; row++)
                {
                    for (int col = 0; col < _mapService.Width; col++)
                    {
                        cellViewModels.Add(new CellViewModel
                        {
                            X = col,
                            Y = row,
                            State = _mapService.Map[row, col]
                        });
                    }
                }

                CellCollection = new ObservableCollection<CellViewModel>(cellViewModels);
                MoveCollection = new ObservableCollection<PlayerMoveModel>(_mapService.Actors.Select(a => new PlayerMoveModel
                {
                    Index = a.Index,
                    Name = a.Name,
                    Move = a.Position,
                    IsTecman = a.IsTecman
                }));
            }
            else
            {
                CellCollection = new ObservableCollection<CellViewModel>();
                MoveCollection = new ObservableCollection<PlayerMoveModel>();
            }

            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SelectedCell")
                {
                    if (SelectedCell != null && SelectedActor != null)
                    {
                        SelectedActor.Move = new Point(SelectedCell.Y, SelectedCell.X);
                        Positions = string.Concat(MoveCollection.Select(m => $"{m.Move.Row},{m.Move.Col},"));
                    }
                }
                else if (args.PropertyName == "Turn")
                {
                    if (Turn > 0)
                        CanSubmit = true;
                }
            };
        }

        public CellViewModel SelectedCell
        {
            get { return _selectedCell; }
            set { SetProperty(ref _selectedCell, value); }
        }

        public PlayerMoveModel SelectedActor
        {
            get { return _selectedActor; }
            set
            {
                SetProperty(ref _selectedActor, value);
            }
        }

        public ICommand CancelCommand
        {
            get { return new DelegateCommand(() => { _isExecuting = false; }); }
        }
        public ICommand SubmitCommand
        {
            get
            {
                return new DelegateCommand(SubmitMove);
            }
        }

        public bool CanSubmit
        {
            get { return _canSubmit; }
            set { SetProperty(ref _canSubmit, value); }
        }

        public int PlayerId
        {
            get { return _playerId; }
            set { SetProperty(ref _playerId, value); }
        }

        public int Turn
        {
            get { return _turn; }
            set { SetProperty(ref _turn, value); }
        }

        public string Positions
        {
            get { return _positions; }
            set { SetProperty(ref _positions, value); }
        }

        public ObservableCollection<CellViewModel> CellCollection
        {
            get { return _cellCollection; }
            set { SetProperty(ref _cellCollection, value); }
        }

        public ObservableCollection<PlayerMoveModel> MoveCollection
        {
            get { return _moveCollection; }
            set { SetProperty(ref _moveCollection, value); }
        }

        public override string Title => "Player Mode";

        private async void SubmitMove()
        {
            try
            {
                CanSubmit = false;

                var performMoveResp = await PerformMove();

                if (!performMoveResp.IsOk())
                    return;


                _isExecuting = true;
                bool isTurnComplete = false;
                while (isTurnComplete == false && _isExecuting)
                {
                    var waitNextTurnResp = await WaitNextTurn();

                    if (waitNextTurnResp.IsOk() == false)
                        break;

                    isTurnComplete = waitNextTurnResp.TurnComplete;

                    if (waitNextTurnResp.GameFinished)
                    {
                        CommonDataManager.Turn = 0;
                    }
                        
                }
                if (!_isExecuting || !isTurnComplete)
                    return;

                _isExecuting = false;

                await GetPlayerView();
            }
            finally
            {
                CanSubmit = true;
            }
        }

        private async Task<PerformMoveResp> PerformMove()
        {
            string[] entries = Positions.Split(',');
            int count = entries.Length / 2;
            List<EnPoint> positions = new List<EnPoint>(count);
            for (int i = 0; i < count; i++)
            {
                positions.Add(new EnPoint()
                {
                    Row = int.Parse(entries[i * 2]),
                    Col = int.Parse(entries[i * 2 + 1])
                });
            }

            var performMoveReq = new PerformMoveReq
            {
                Auth = new ReqAuth
                {
                    TeamName = TeamName,
                    AuthCode = AuthCode,
                    ClientName = Username,
                    SequenceNumber = SequenceNumber,
                    SessionId = SessionId
                },

                PlayerId = PlayerId,
                Positions = positions
            };

            var performMoveResp = await ServiceCallInvoker.InvokeAsync<PerformMoveReq, PerformMoveResp>(ServiceUrl.TrimEnd('/') + "/json/PerformMove", performMoveReq);
            CommonDataManager.SequenceNumber++;

            return performMoveResp;
        }

        private async Task<WaitNextTurnResp> WaitNextTurn()
        {
            var waitNextTurnReq = new WaitNextTurnReq
            {
                Auth = new ReqAuth
                {
                    TeamName = TeamName,
                    AuthCode = AuthCode,
                    ClientName = Username,
                    SequenceNumber = SequenceNumber,
                    SessionId = SessionId
                },
                PlayerId = PlayerId,
                RefTurn = Turn
            };

            var waitNextTurnResp = await ServiceCallInvoker.InvokeAsync<WaitNextTurnReq, WaitNextTurnResp>(ServiceUrl.TrimEnd('/') + "/json/WaitNextTurn", waitNextTurnReq);
            CommonDataManager.SequenceNumber++;
            return waitNextTurnResp;
        }

        private async Task GetPlayerView()
        {
            var req = new GetPlayerViewReq
            {
                Auth = new ReqAuth
                {
                    TeamName = TeamName,
                    AuthCode = AuthCode,
                    ClientName = Username,
                    SequenceNumber = SequenceNumber,
                    SessionId = SessionId
                },
                PlayerId = PlayerId
            };

            var getPlayerViewResp = await ServiceCallInvoker.InvokeAsync<GetPlayerViewReq, GetPlayerViewResp>(ServiceUrl.TrimEnd('/') + "/json/GetPlayerView", req);
            CommonDataManager.SequenceNumber++;

            if (!getPlayerViewResp.IsOk())
                return;

            CommonDataManager.Turn = getPlayerViewResp.Turn;
            CommonDataManager.IAmTecman = getPlayerViewResp.Mode == "TECMAN";

            _mapService.UpdateMap(getPlayerViewResp.Map.Rows,
                getPlayerViewResp.TecmanPosition.ToPoint(),
                getPlayerViewResp.GhostPositions.Select(p => p.ToPoint()).ToList(),
                getPlayerViewResp.PreviousGhostPositions.Select(p => p.ToPoint()).ToList(),
                CommonDataManager.IAmTecman);
        }
    }
}