using System.Threading.Tasks;
using Game.ClientCommon.DataContracts;

namespace Game.ClientCommon.Infrastructure
{
    public interface IServiceCallInvoker
    {
        Task<TResp> InvokeAsync<TReq, TResp>(string serviceUrl, TReq req)
            where TReq : BaseReq
            where TResp : BaseResp, new();

        event InvokedEventHandler InvokeBegan;
        event InvokedEventHandler InvokeFinished;
    }

    public delegate void InvokedEventHandler(object sender, InvokeEventArgs args);
}