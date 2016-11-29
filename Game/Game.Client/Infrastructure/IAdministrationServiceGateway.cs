using Game.AdminClient.AdminService;
using Game.AdminClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameLogic.UserManagement;

namespace Game.AdminClient.Infrastructure
{
    public interface IAdministrationServiceGateway
    {
        Task<TeamRole> LoginAsync();
        Task<IEnumerable<Player>> ListPlayersAsync();
        Task<IEnumerable<Models.Game>> ListGamesAsync();
        Task<Models.Game> CreateGameAsync();
        Task<Models.Game> GetGameAsync(int gameId);
        Task StartGameAsync(int gameId);
        Task AddPlayerAsync(int gameId, int playerId);
        Task RemovePlayerAsync(int gameId, int playerId);
        Task SetMapAsync(int gameId, Map map);
        Task DeleteGameAsync(int gameId);
        Task ResumeGameAsync(int gameId);
        Task PauseGameAsync(int gameId);
        Task<Match> GetMatchAsync(int gameId);
        Task<Turn> GetNextTurnAsync(int gameId);
        Task DropPlayer(int gameId, int playerId);
        Task<GetLiveInfoResp> GetLiveInfo(int gameId);

        ConnectionData ConnectionData { get; set; }
        long LastCallTime { get; set; }
    }
}