using Game.ClientCommon.Infrastructure;
using Prism.Mvvm;

namespace Game.DebugClient.ViewModel
{
    public class LoggerViewModel : BindableBase
    {
        private long _operationTime;

        private string _request;

        private string _response;

        public LoggerViewModel(IServiceCallInvoker serviceCallInvoker)
        {
            serviceCallInvoker.InvokeBegan += (sender, args) => { Request = args.Request; };

            serviceCallInvoker.InvokeFinished += (sender, args) =>
            {
                Response = args.Response;
                OperationTime = args.OperationTime;
            };
        }

        public string Request
        {
            get { return _request; }
            set { SetProperty(ref _request, value); }
        }

        public string Response
        {
            get { return _response; }
            set { SetProperty(ref _response, value); }
        }

        public long OperationTime
        {
            get { return _operationTime; }
            set { SetProperty(ref _operationTime, value); }
        }
    }
}