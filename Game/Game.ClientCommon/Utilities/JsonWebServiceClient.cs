using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game.ClientCommon.Utilites
{
    public class JsonWebServiceClient : IWebServiceClient
    {
        public Task<TResponse> Post<TRequest, TResponse>(string serviceUrl, TRequest request)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(serviceUrl);
            webRequest.Method = WebRequestMethods.Http.Post;

            var content = new UTF8Encoding().GetBytes(JsonConvert.SerializeObject(request));

            webRequest.ContentLength = content.Length;
            webRequest.ContentType = "application/json";

            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(content, 0, content.Length);
            }

            using (var webResponse = (HttpWebResponse) webRequest.GetResponse())
            using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                var response = JsonConvert.DeserializeObject<TResponse>(streamReader.ReadToEnd());
                return Task.FromResult(response);
            }
        }
    }
}