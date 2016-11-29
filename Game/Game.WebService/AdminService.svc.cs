using System.Linq;
using System.ServiceModel;
using GameLogic;
using Game.WebService.Model;
using GameLogic.UserManagement;

namespace Game.WebService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AdminService : ServiceBase, IAdminService
    {
        public InitLoginResp InitLogin(InitLoginReq req)
        {
            return HandleServiceCall(req, new InitLoginResp(), InitLoginImpl, new SessionAuthOptions { IsLoginFlow = true });
        }

        private void InitLoginImpl(InitLoginReq req, InitLoginResp resp)
        {
            SessionChallenge challenge = Server.SessionManager.CreateChallenge(req.Auth.GetClientCode());
            resp.Challenge = challenge.Challenge;
        }

        public CompleteLoginResp CompleteLogin(CompleteLoginReq req)
        {
            return HandleServiceCall(req, new CompleteLoginResp(), CompleteLoginImpl, new SessionAuthOptions { IsLoginFlow = true });
        }

        private void CompleteLoginImpl(CompleteLoginReq req, CompleteLoginResp resp)
        {
            ClientSession session = Server.SessionManager.CreateSession(req.Auth.GetClientCode(), req.ChallengeResponse);
            resp.SessionId = session.SessionId;
        }

        public ListGamesResp ListGames(ListGamesReq req)
        {
            return HandleServiceCall(req, new ListGamesResp(), ListGamesImpl);
        }

        private void ListGamesImpl(ListGamesReq req, ListGamesResp resp)
        {
            resp.Games = Server.GameManager
                .ListGames(Server.TeamRegistry.GetTeam(req.Auth.TeamName))
                .Select(g => new EnGameInfo(g)).ToList();
        }

        public CreateGameResp CreateGame(CreateGameReq req)
        {
            return HandleServiceCall(req, new CreateGameResp(), CreateGameImpl);
        }

        private void CreateGameImpl(CreateGameReq req, CreateGameResp resp)
        {
            Team owner = Server.TeamRegistry.GetTeam(req.Auth.TeamName);
            GameInfo gameInfo = Server.GameManager.CreateGame(owner);
            resp.GameInfo = new EnGameInfo(gameInfo);
        }

        public GetGameDetailsResp GetGameDetails(GetGameDetailsReq req)
        {
            return HandleServiceCall(req, new GetGameDetailsResp(), GetGameDetailsImpl);
        }

        private void GetGameDetailsImpl(GetGameDetailsReq req, GetGameDetailsResp resp)
        {
            GameDetails gameDetails = Server.GameManager.GetGameDetails(req.GameId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
            resp.GameDetails = new EnGameDetails(gameDetails);
        }

        public ListPlayersResp ListPlayers(ListPlayersReq req)
        {
            return HandleServiceCall(req, new ListPlayersResp(), ListPlayersImpl);
        }

        private void ListPlayersImpl(ListPlayersReq req, ListPlayersResp resp)
        {
            resp.Players = Server.GameManager
                .ListPlayers(Server.TeamRegistry.GetTeam(req.Auth.TeamName))
                .Select(p => new EnPlayerInfo(p))
                .ToList();
        }

        public AddGamePlayerResp AddGamePlayer(AddGamePlayerReq req)
        {
            return HandleServiceCall(req, new AddGamePlayerResp(), AddGamePlayerImpl);
        }

        private void AddGamePlayerImpl(AddGamePlayerReq req, AddGamePlayerResp resp)
        {
            Server.GameManager.AddGamePlayer(req.GameId, req.PlayerId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public RemoveGamePlayerResp RemoveGamePlayer(RemoveGamePlayerReq req)
        {
            return HandleServiceCall(req, new RemoveGamePlayerResp(), RemoveGamePlayerImpl);
        }

        private void RemoveGamePlayerImpl(RemoveGamePlayerReq req, RemoveGamePlayerResp resp)
        {
            Server.GameManager.RemoveGamePlayer(req.GameId, req.PlayerId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public SetGameMapResp SetGameMap(SetGameMapReq req)
        {
            return HandleServiceCall(req, new SetGameMapResp(), SetGameMapImpl);
        }

        private void SetGameMapImpl(SetGameMapReq req, SetGameMapResp resp)
        {
            Server.GameManager.SetGameMap(req.GameId, req.MapData.ToMapData(), Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public StartGameResp StartGame(StartGameReq req)
        {
            return HandleServiceCall(req, new StartGameResp(), StartGameImpl);
        }

        private void StartGameImpl(StartGameReq req, StartGameResp resp)
        {
            Server.GameManager.StartGame(req.GameId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public PauseGameResp PauseGame(PauseGameReq req)
        {
            return HandleServiceCall(req, new PauseGameResp(), PauseGameImpl);
        }

        private void PauseGameImpl(PauseGameReq req, PauseGameResp resp)
        {
            Server.GameManager.PauseGame(req.GameId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public ResumeGameResp ResumeGame(ResumeGameReq req)
        {
            return HandleServiceCall(req, new ResumeGameResp(), ResumeGameImpl);
        }

        private void ResumeGameImpl(ResumeGameReq req, ResumeGameResp resp)
        {
            Server.GameManager.ResumeGame(req.GameId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public CreateObserverResp CreateObserver(CreateObserverReq req)
        {
            return HandleServiceCall(req, new CreateObserverResp(), CreateObserverImpl);
        }

        private void CreateObserverImpl(CreateObserverReq req, CreateObserverResp resp)
        {
            ObserverInfo oi = Server.GameManager.CreateObserver(Server.TeamRegistry.GetTeam(req.Auth.TeamName), req.Auth.ClientName);
            resp.ObserverId = oi.ObserverId;
            // FIXME This is probably used by admin client to enter observer mode and/or disable some controls
            resp.Role = oi.Team.Role;
        }

        public StartObservingResp StartObserving(StartObservingReq req)
        {
            return HandleServiceCall(req, new StartObservingResp(), StartObservingImpl);
        }

        private void StartObservingImpl(StartObservingReq req, StartObservingResp resp)
        {
            Team team = Server.TeamRegistry.GetTeam(req.Auth.TeamName);
            GameDetails gd = Server.GameManager.GetGameDetails(req.GameId, team);
            GameViewInfo gv = Server.GameManager.StartObserving(req.ObserverId, req.GameId, req.Auth.GetClientCode(), team);
            resp.GameDetails = new EnGameDetails(gd);
            resp.Turn = gv.Turn;
            resp.MapData = new EnMapData(gv.Map);
            resp.TecmanPosition = new EnPoint(gv.Map.TecmanPosition);
            resp.GhostPositions = gv.Map.GhostPosition.Select(p => new EnPoint(p)).ToArray();
            resp.PlayerStates = gv.PlayerStates.Select(ps => new EnPlayerState(ps)).ToArray();
        }

        public ObserveNextTurnResp ObserveNextTurn(ObserveNextTurnReq req)
        {
            return HandleServiceCall(req, new ObserveNextTurnResp(), ObserveNextTurnImpl);
        }

        private void ObserveNextTurnImpl(ObserveNextTurnReq req, ObserveNextTurnResp resp)
        {
            Team team = Server.TeamRegistry.GetTeam(req.Auth.TeamName);
            ObservedGameInfo gi = Server.GameManager.ObserveNextTurn(req.ObserverId, req.GameId, req.Auth.GetClientCode(), team);
            resp.GameInfo = new EnObsGameInfo(gi);
            resp.TurnInfo = (gi.TurnInfo == null) ? null : new EnObsTurnInfo(gi.TurnInfo);
        }


        public DropPlayerResp DropPlayer(DropPlayerReq req)
        {
            return HandleServiceCall(req, new DropPlayerResp(), DropPlayerImpl);
        }

        private void DropPlayerImpl(DropPlayerReq req, DropPlayerResp resp)
        {
            Server.GameManager.DropPlayer(req.PlayerId, req.GameId, req.Auth.GetClientCode(), Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public DeleteGameResp DeleteGame(DeleteGameReq req)
        {
            return HandleServiceCall(req, new DeleteGameResp(), DeleteGameImpl);
        }

        private void DeleteGameImpl(DeleteGameReq req, DeleteGameResp resp)
        {
            Server.GameManager.DeleteGame(req.GameId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
        }

        public GetLiveInfoResp GetLiveInfo(GetLiveInfoReq req)
        {
            return HandleServiceCall(req, new GetLiveInfoResp(), GetLiveInfoImpl);
        }

        private void GetLiveInfoImpl(GetLiveInfoReq req, GetLiveInfoResp resp)
        {
            GameLiveInfo gi = Server.GameManager.GetLiveInfo(req.GameId, Server.TeamRegistry.GetTeam(req.Auth.TeamName));
            resp.GameLiveInfo = new EnGameLiveInfo(gi);
        }
    }
}
