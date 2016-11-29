using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Game.ClientCommon.DataContracts;
using Game.ClientCommon.Utilites;
using Newtonsoft.Json;
using Prism.Logging;

namespace Game.ClientCommon.Infrastructure
{
    public static class BaseResponseExtensions
    {
        public static bool IsOk(this BaseResp resp)
        {
            if (ReferenceEquals(resp, null))
                return false;

            if (resp.Status != "OK")
                return false;

            return true;
        }
    }

    public class ServiceCallInvoker : IServiceCallInvoker
    {
        private readonly ILoggerFacade _logger;
        private readonly IWebServiceClient _webServiceClient;

        public ServiceCallInvoker(ILoggerFacade logger, IWebServiceClient webServiceClient)
        {
            _logger = logger;
            _webServiceClient = webServiceClient;
        }

        public async Task<TResp> InvokeAsync<TReq, TResp>(string url, TReq req)
            where TReq : BaseReq
            where TResp : BaseResp, new()
        {
            var requestString = JsonConvert.SerializeObject(req, Formatting.Indented);

            var reqArgs = new InvokeEventArgs
            {
                Request = requestString
            };

            OnInvokeBegan(reqArgs);

            _logger.Log(requestString, Category.Info, Priority.Medium);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            TResp resp = null;
            try
            {
                resp = await _webServiceClient.Post<TReq, TResp>(url, req);
            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                sb.AppendLine(e.Message);
                sb.AppendLine();
                sb.AppendLine(e.StackTrace);

                var errorArgs = new InvokeEventArgs
                {
                    Response = sb.ToString(),
                    OperationTime = stopwatch.ElapsedMilliseconds
                };

                _logger.Log(sb.ToString(), Category.Info, Priority.Medium);

                OnInvokeFinished(errorArgs);

                return resp;
            }

            stopwatch.Stop();

            var responseString = JsonConvert.SerializeObject(resp, Formatting.Indented);

            var respArgs = new InvokeEventArgs
            {
                Response = responseString,
                OperationTime = stopwatch.ElapsedMilliseconds
            };

            _logger.Log(responseString, Category.Info, Priority.Medium);
            _logger.Log($"{url}: {stopwatch.ElapsedMilliseconds} ms", Category.Info, Priority.Medium);

            OnInvokeFinished(respArgs);

            return resp;
        }

        public event InvokedEventHandler InvokeBegan;
        public event InvokedEventHandler InvokeFinished;

        protected virtual void OnInvokeBegan(InvokeEventArgs e)
        {
            var handler = InvokeBegan;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnInvokeFinished(InvokeEventArgs e)
        {
            var handler = InvokeFinished;
            if (handler != null)
                handler(this, e);
        }
    }
}