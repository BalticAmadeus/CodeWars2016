using System.Threading.Tasks;
using Game.AdminClient.Models;

namespace Game.AdminClient.Infrastructure
{
    public interface IMapService
    {
        Task<Map> LoadMapAsync(string path);
    }
}
