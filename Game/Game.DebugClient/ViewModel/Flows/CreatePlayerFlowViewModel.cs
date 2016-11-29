using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Infrastructure;
using Game.DebugClient.Infrastructure;
using Prism.Commands;

namespace Game.DebugClient.ViewModel.Flows
{
    public class CreatePlayerFlowViewModel : ServiceCallViewModel
    {
        private readonly IMessageBoxDialogService _messageBoxDialogService;
        private readonly ISessionIdGenerator _sessionIdGenerator;

        public CreatePlayerFlowViewModel(
            ICommonDataManager commonDataManager,
            IServiceCallInvoker serviceCallInvoker,
            IMessageBoxDialogService messageBoxDialogService,
            ISessionIdGenerator sessionIdGenerator)
            : base(commonDataManager, serviceCallInvoker)
        {
            _messageBoxDialogService = messageBoxDialogService;
            _sessionIdGenerator = sessionIdGenerator;
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
                            SessionId = _sessionIdGenerator.NextSessionId();
                            SequenceNumber = 0;

                            var createPlayerReq = new CreatePlayerReq
                            {
                                Auth = new ReqAuth
                                {
                                    TeamName = TeamName,
                                    AuthCode = AuthCode,
                                    ClientName = Username,
                                    SequenceNumber = SequenceNumber,
                                    SessionId = SessionId
                                }
                            };

                            var createPlayerResp =
                                await
                                    ServiceCallInvoker.InvokeAsync<CreatePlayerReq, CreatePlayerResp>(
                                        ServiceUrl.TrimEnd('/') + "/json/CreatePlayer", createPlayerReq);

                            if (!createPlayerResp.IsOk())
                                return;

                            CommonDataManager.SessionId = SessionId;
                            CommonDataManager.SequenceNumber = SequenceNumber + 1;
                            CommonDataManager.PlayerId = createPlayerResp.PlayerId;
                            CommonDataManager.Turn = 0;
                        }
                        catch (Exception e)
                        {
                            _messageBoxDialogService.OpenDialog(e.Message, "Exception occurred");
                        }
                    });
                });
            }
        }

        public override string Title
        {
            get { return "CreatePlayer with session (re)start"; }
        }
    }
}