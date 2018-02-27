using System.Threading.Tasks;

namespace LineSharp.Rest
{
    public interface IRestClient
    {
        Task<TResult> PostAsync<TResult>(string url, object msg);

        Task PostAsync(string url, object msg);

        Task<TResponse> GetAsync<TResponse>(string url);

        Task<byte[]> GetAsyncAsByteArray(string url);

        Task DeleteAsync(string url);
    }
}