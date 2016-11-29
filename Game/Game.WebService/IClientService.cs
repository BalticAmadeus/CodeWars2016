using System.ServiceModel;
using System.ServiceModel.Web;
using Game.WebService.Model;

namespace Game.WebService
{
    [ServiceContract]
    public interface IClientService
    {
        /* Session setup */

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CreatePlayerResp CreatePlayer(CreatePlayerReq req);

        /* Game progress */

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GetPlayerViewResp GetPlayerView(GetPlayerViewReq req);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        PerformMoveResp PerformMove(PerformMoveReq req);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WaitNextTurnResp WaitNextTurn(WaitNextTurnReq req);
    }

}
