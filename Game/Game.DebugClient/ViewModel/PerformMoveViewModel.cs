using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Infrastructure;
using Game.DebugClient.Infrastructure;
using Game.DebugClient.ViewModel.Flows;
using Prism.Commands;
using Game.ClientCommon;

namespace Game.DebugClient.ViewModel
{
    public class PerformMoveViewModel : ServiceCallViewModel
    {
        private readonly IMapService _mapService;
        private readonly IMessageBoxDialogService _messageBoxDialogService;

        private ObservableCollection<CellViewModel> _cellCollection;
        private ObservableCollection<PlayerMoveModel> _moveCollection;
        private CellViewModel _selectedCell;
        private PlayerMoveModel _selectedActor;

        private int _playerId;
        private string _positions;

        public PerformMoveViewModel(
            ICommonDataManager commonDataManager,
            IServiceCallInvoker serviceCallInvoker,
            IMapService mapService,
            IMessageBoxDialogService messageBoxDialogService)
            : base(commonDataManager, serviceCallInvoker)
        {
            CommonDataManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "PlayerId")
                    PlayerId = CommonDataManager.PlayerId;
            };

            _mapService = mapService;
            _messageBoxDialogService = messageBoxDialogService;
            PlayerId = CommonDataManager.PlayerId;

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
            };

        }

        public ICommand ExecuteCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await Task.Run(async () =>
                    {
                        try
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
                            var req = new PerformMoveReq
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

                            var performMoveResp =
                                await
                                    ServiceCallInvoker.InvokeAsync<PerformMoveReq, PerformMoveResp>(
                                        ServiceUrl.TrimEnd('/') + "/json/PerformMove", req);

                            CommonDataManager.SequenceNumber++;

                            if (!performMoveResp.IsOk())
                                return;
                        }
                        catch (Exception e)
                        {
                            _messageBoxDialogService.OpenDialog(e.Message, "Exception occurred");
                        }
                    });
                });
            }
        }

        public int PlayerId
        {
            get { return _playerId; }
            set { SetProperty(ref _playerId, value); }
        }

        public string Positions
        {
            get { return _positions; }
            set { SetProperty(ref _positions, value); }
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

        public ObservableCollection<PlayerMoveModel> MoveCollection
        {
            get { return _moveCollection; }
            set { SetProperty(ref _moveCollection, value); }
        }

        public ObservableCollection<CellViewModel> CellCollection
        {
            get { return _cellCollection; }
            set { SetProperty(ref _cellCollection, value); }
        }

        public override string Title
        {
            get { return "Perform Move"; }
        }
    }
}