using System.Threading.Tasks;

namespace LineSharp.Rest
{
    public interface IRestClient
    {
        Task PostAsync<TMessage>(string url, TMessage msg);

        Task<TResponse> GetAsync<TResponse>(string url);

        Task<byte[]> GetAsyncAsByteArray(string url);
    }
}
