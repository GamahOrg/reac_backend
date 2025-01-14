using Core.Contracts;
using Core.Shared.Responses;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace Core.Services;

public class HttpService : IHttpService
{
    public async Task<DataResponse<TResult>> Get<TResult>(string url, Dictionary<string, string>? headers = null)
    {
        DataResponse<TResult> result = new DataResponse<TResult> { Message = "Internal Server Error" };

        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var httpClient = new HttpClient(handler))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var response = await httpClient.GetAsync(new Uri(url));
                var responseString = response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = new DataResponse<TResult>
                    {
                        IsDone = true,
                        Data = await response.Content.ReadFromJsonAsync<TResult>()
                    };
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.ToString();
        }

        return result;
    }

    public async Task<DataResponse<TResult>> PostRequest<TResult, TModel>(string url, TModel postObject, Dictionary<string, string>? headers = null)
    {
        DataResponse<TResult> result = new DataResponse<TResult> { Message = "Internal Server Error" };

        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var httpClient = new HttpClient(handler))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var body = JsonConvert.SerializeObject(postObject);
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, content);
                var responseString = response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = new DataResponse<TResult>
                    {
                        IsDone = true,
                        Data = await response.Content.ReadFromJsonAsync<TResult>()
                    };
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.ToString();
        }

        return result;
    }

    public async Task<TResult> PostResult<TResult, TModel>(string url, TModel postObject, Dictionary<string, string>? headers = null)
        where TResult : class
    {
        TResult? result = null;

        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var httpClient = new HttpClient(handler))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var response = await httpClient.PostAsJsonAsync(url, postObject);
                var responseString = response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = await response.Content.ReadFromJsonAsync<TResult>();
                }
            }
        }
        catch (Exception ex)
        {
            result = null;
        }

        return result;
    }

    public async Task<BaseResponse> PostRequest<TModel>(string url, TModel postObject, Dictionary<string, string>? headers = null)
    {
        BaseResponse result = new BaseResponse { Message = "Internal Server Error" };

        try
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var httpClient = new HttpClient(handler))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var response = await httpClient.PostAsJsonAsync(url, postObject);

                response.EnsureSuccessStatusCode();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = new BaseResponse
                    {
                        IsDone = true
                    };
                }
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.ToString();
        }

        return result;
    }
}
