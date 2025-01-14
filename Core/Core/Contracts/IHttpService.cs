using Core.Shared.Responses;

namespace Core.Contracts;

public interface IHttpService
{
    Task<DataResponse<TResult>> Get<TResult>(string url, Dictionary<string, string>? headers = null);
    Task<DataResponse<TResult>> PostRequest<TResult,TModel>(string url, TModel postObject, Dictionary<string, string>? headers = null);
    Task<BaseResponse> PostRequest<TModel>(string url, TModel postObject, Dictionary<string, string>? headers = null);
    Task<TResult> PostResult<TResult, TModel>(string url, TModel postObject, Dictionary<string, string>? headers = null)
       where TResult : class;
}
