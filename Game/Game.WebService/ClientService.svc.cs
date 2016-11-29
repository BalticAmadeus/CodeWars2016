using System.Linq;
using System.ServiceModel;
using System.Threading;
using GameLogic;
using Game.WebService.Model;
using System;

namespace Game.WebService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientService : ServiceBase, IClientService
    {
        public CreatePlayerResp CreatePlayer(CreatePlayerReq req)
        {
            return HandleServiceCall(req, new CreatePlayerResp(), CreatePlayerImpl);
        }

        private void CreatePlayerImpl(CreatePlayerReq req, CreatePlayerResp resp)
        {
            Team team = Server.TeamRegistry.GetTeam(req.Auth.TeamName);
            PlayerInfo player = Server.GameManager.CreatePlayer(team, req.Auth.ClientName);
            resp.PlayerId = player.PlayerId;
        }

        public GetPlayerViewResp GetPlayerView(GetPlayerViewReq req)
        {
            return HandleServiceCall(req, new GetPlayerViewResp(), GetPlayerViewImpl);
        }

        private void GetPlayerViewImpl(GetPlayerViewReq req, GetPlayerViewResp resp)
        {
            GameViewInfo gv = Server.GameManager.GetPlayerView(req.PlayerId, req.Auth.GetClientCode());
            resp.Turn = gv.Turn;
            resp.Mode = gv.PlayerStates[gv.PlayerIndex].Mode.ToString();
            resp.Map = new EnMapData(gv.Map);
            resp.TecmanPosition = new EnPoint(gv.Map.TecmanPosition);
            resp.GhostPositions = gv.Map.GhostPosition.Select(p => new EnPoint(p)).ToList();
            resp.PreviousTecmanPosition = new EnPoint(gv.PreviousTecmanPosition);
            resp.PreviousGhostPositions = gv.PreviousGhostPosition.Select(p => new EnPoint(p)).ToList();
        }

        public PerformMoveResp PerformMove(PerformMoveReq req)
        {
            return HandleServiceCall(req, new PerformMoveResp(), PerformMoveImpl);
        }

        private void PerformMoveImpl(PerformMoveReq req, PerformMoveResp resp)
        {
            Point[] positions = (req.Positions != null) ? req.Positions.Select(p => p?.ToPoint() ?? new Point(-1, -1)).ToArray() : null;
            Server.GameManager.PerformMove(req.PlayerId, positions, req.Auth.GetClientCode());
        }

        public WaitNextTurnResp WaitNextTurn(WaitNextTurnReq req)
        {
            return HandleServiceCall(req, new WaitNextTurnResp(), WaitNextTurnImpl);
        }

        private void WaitNextTurnImpl(WaitNextTurnReq req, WaitNextTurnResp resp)
        {
            WaitTurnInfo wi = Server.GameManager.WaitNextTurn(req.PlayerId, req.RefTurn, req.Auth.GetClientCode(), DateTime.Now);
            resp.TurnComplete = wi.TurnComplete;
            resp.GameFinished = wi.GameFinished;
            resp.FinishCondition = wi.FinishCondition.ToString();
            resp.FinishComment = wi.FinishComment;
        }
    }
}
