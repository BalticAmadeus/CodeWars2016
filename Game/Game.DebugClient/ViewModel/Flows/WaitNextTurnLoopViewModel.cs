using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Infrastructure;
using Game.DebugClient.Infrastructure;
using Prism.Commands;

namespace Game.DebugClient.ViewModel.Flows
{
    public class WaitNextTurnLoopViewModel : ServiceCallViewModel
    {
        private readonly IMessageBoxDialogService _messageBoxDialogService;

        private bool _isExecuting;

        private int _playerId;

        private int _turn;

        public WaitNextTurnLoopViewModel(
            ICommonDataManager commonDataManager,
            IServiceCallInvoker serviceCallInvoker,
            IMessageBoxDialogService messageBoxDialogService)
            : base(commonDataManager, serviceCallInvoker)
        {
            _messageBoxDialogService = messageBoxDialogService;
            PlayerId = CommonDataManager.PlayerId;
            Turn = CommonDataManager.Turn;

            CommonDataManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Turn")
                    Turn = CommonDataManager.Turn;

                if (args.PropertyName == "PlayerId")
                    PlayerId = CommonDataManager.PlayerId;
            };
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
                            _isExecuting = true;
                            var isTurnComplete = false;
                            while (!isTurnComplete && _isExecuting)
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

                                var waitNextTurnResp =
                                    await
                                        ServiceCallInvoker.InvokeAsync<WaitNextTurnReq, WaitNextTurnResp>(
                                            ServiceUrl.TrimEnd('/') + "/json/WaitNextTurn", waitNextTurnReq);

                                CommonDataManager.SequenceNumber++;

                                if (!waitNextTurnResp.IsOk())
                                    return;
                                
                                isTurnComplete = waitNextTurnResp.TurnComplete;
                                if (waitNextTurnResp.GameFinished)
                                    CommonDataManager.Turn = 0;
                            }

                            _isExecuting = false;
                        }
                        catch (Exception e)
                        {
                            _messageBoxDialogService.OpenDialog(e.Message, "Exception occurred");
                        }
                    });
                });
            }
        }


        public ICommand CancelCommand
        {
            get { return new DelegateCommand(async () => { await Task.Run(() => { _isExecuting = false; }); }); }
        }

        public override string Title
        {
            get { return "Wait Next Turn Loop"; }
        }
    }
}