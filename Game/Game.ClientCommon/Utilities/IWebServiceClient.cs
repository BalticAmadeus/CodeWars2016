using System.Threading.Tasks;

namespace Game.ClientCommon.Utilites
{
    public interface IWebServiceClient
    {
        Task<TResponse> Post<TRequest, TResponse>(string serviceUrl, TRequest request);
    }
}