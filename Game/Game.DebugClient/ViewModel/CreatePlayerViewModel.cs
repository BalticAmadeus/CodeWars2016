using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Infrastructure;
using Game.DebugClient.Infrastructure;
using Prism.Commands;

namespace Game.DebugClient.ViewModel
{
    public class CreatePlayerViewModel : ServiceCallViewModel
    {
        private readonly IMessageBoxDialogService _messageBoxDialogService;

        public CreatePlayerViewModel(
            ICommonDataManager commonDataManager,
            IServiceCallInvoker serviceCallInvoker,
            IMessageBoxDialogService messageBoxDialogService)
            : base(commonDataManager, serviceCallInvoker)
        {
            _messageBoxDialogService = messageBoxDialogService;
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
                            var req = new CreatePlayerReq
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
                                        ServiceUrl.TrimEnd('/') + "/json/CreatePlayer", req);

                            CommonDataManager.SequenceNumber++;

                            if (!createPlayerResp.IsOk())
                                return;

                            CommonDataManager.PlayerId = createPlayerResp.PlayerId;
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
            get { return "CreatePlayer"; }
        }
    }
}