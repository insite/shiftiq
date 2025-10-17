using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Common
{
    public abstract class ApiClientBase
    {
        protected string HttpRequestFailure = "The status code in the HTTP response is outside the 200-299 range.";

        protected string JsonDeserializationError = "The content in the API response cannot be deserialized to the requested type.";

        protected readonly IHttpClientFactory _httpClientFactory;

        protected readonly IJsonSerializerBase _serializer;

        public ApiClientBase(IHttpClientFactory httpClientFactory, IJsonSerializerBase serializer)
        {
            _httpClientFactory = httpClientFactory;

            _serializer = serializer;
        }

        protected Problem CreateProblem(string summary, Exception ex, string endpoint, string responseContent)
        {
            var problem = new Problem()
            {
                Title = summary,
                Detail = ex.GetFormattedMessages()
            };

            problem.Extensions["Endpoint"] = endpoint;
            problem.Extensions["Response"] = responseContent;

            return problem;
        }

        protected Problem CreateProblem(HttpStatusCode statusCode, string endpoint, string responseContent)
        {
            try
            {
                return _serializer.Deserialize<Problem>(responseContent);
            }
            catch
            {
                return ProblemFactory.Create((int)statusCode, responseContent, endpoint);
            }
        }

        protected HttpClient CreateHttpClient()
            => _httpClientFactory.Create();

        protected string CreateUrl(string endpoint, string[] segments = null, Dictionary<string, string> parameters = null)
        {
            var url = endpoint.ToString();

            if (segments != null && segments.Length > 0)
            {
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }

                url += string.Join("/", segments);
            }

            if (parameters != null && parameters.Count > 0)
            {
                url += "?";

                var index = 0;

                foreach (var parameter in parameters)
                {
                    if (index > 0)
                    {
                        url += "&";
                    }

                    url += $"{parameter.Key}={parameter.Value}";

                    index++;
                }
            }

            return url;
        }

        protected void SetPagination(ApiResult status)
        {
            if (status.Headers.TryGetValues(QueryPagination.HeaderKey, out IEnumerable<string> values))
                status.Pagination = _serializer.Deserialize<QueryPagination>(values.First());
        }

        public Dictionary<string, string> ToDictionary(object criteria)
        {
            var reflector = new SimpleReflector();
            return reflector.CreateDictionary(criteria);
        }
    }

    public class ApiClient : ApiClientBase
    {
        public ApiClient(IHttpClientFactory httpClientFactory, IJsonSerializerBase serializer)
            : base(httpClientFactory, serializer)
        {

        }

        #region Requests (GET, POST, PUT, DELETE)

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, string[] segments = null, Dictionary<string, string> parameters = null)
        {
            var url = CreateUrl(endpoint, segments, parameters);

            using (var client = CreateHttpClient())
            {
                var response = await client.GetAsync(url);

                var status = new ApiResult<T>(response.StatusCode, response.Headers);

                if (response.IsSuccessStatusCode)
                {
                    SetPagination(status);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        try
                        {
                            status.Data = _serializer.Deserialize<T>(responseContent);
                        }
                        catch
                        {
                            // If deserialization fails, and if the expected data type for the
                            // response is a string, then we can use the raw response content.

                            if (typeof(T) == typeof(string) && responseContent is T castedToString)
                                status.Data = castedToString;
                            else
                                throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        status.Problem = CreateProblem(JsonDeserializationError, ex, endpoint, responseContent);
                    }
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    status.Problem = CreateProblem(HttpRequestFailure, null, endpoint, responseContent);
                }

                return status;
            }
        }

        public async Task<ApiResult<T>> HttpPost<T>(string endpoint, object payload, string mediaType = "application/json")
        {
            var url = CreateUrl(endpoint);

            var data = mediaType == "plain/text" && payload is string
                ? (string)payload
                : _serializer.Serialize(payload);

            var content = new StringContent(data, Encoding.UTF8, mediaType);

            using (var client = CreateHttpClient())
            {
                var response = await client.PostAsync(url, content);

                var status = new ApiResult<T>(response.StatusCode, response.Headers);

                if (response.IsSuccessStatusCode)
                {
                    SetPagination(status);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        try
                        {
                            status.Data = _serializer.Deserialize<T>(responseContent);
                        }
                        catch
                        {
                            // If deserialization fails, and if the expected data type for the
                            // response is a string, then we can use the raw response content.

                            if (typeof(T) == typeof(string) && responseContent is T castedToString)
                                status.Data = castedToString;
                            else
                                throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        status.Problem = CreateProblem(JsonDeserializationError, ex, endpoint, responseContent);
                    }
                }

                return status;
            }
        }

        public async Task<ApiResult> HttpPost(string endpoint, object payload)
        {
            var url = CreateUrl(endpoint);

            var data = _serializer.Serialize(payload);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using (var client = CreateHttpClient())
            {
                var response = await client.PostAsync(url, content);

                var status = new ApiResult(response.StatusCode, response.Headers);

                if (response.IsSuccessStatusCode)
                    return status;

                var responseContent = await response.Content.ReadAsStringAsync();
                status.Problem = CreateProblem(response.StatusCode, endpoint, responseContent);

                return status;
            }
        }

        public async Task<ApiResult> HttpPut(string endpoint, string[] segments, object payload)
        {
            var url = CreateUrl(endpoint, segments);

            var data = _serializer.Serialize(payload);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using (var client = CreateHttpClient())
            {
                var response = await client.PutAsync(url, content);

                var status = new ApiResult(response.StatusCode, response.Headers);

                return status;
            }
        }

        public async Task<ApiResult> HttpDelete(string endpoint, string[] segments)
        {
            var url = CreateUrl(endpoint, segments);

            using (var client = CreateHttpClient())
            {
                var response = await client.DeleteAsync(url);

                var status = new ApiResult(response.StatusCode, response.Headers);

                return status;
            }
        }

        #endregion

        #region Wrappers (GET, PUT, DELETE)

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, string id)
            => await HttpGet<T>(endpoint, new[] { id });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id)
            => await HttpGet<T>(endpoint, id.ToString());

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, Guid id2)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, Guid id2, Guid id3)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, Guid id2, int id3)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, int id2)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, int id2, Guid id3)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, int id2, string id3)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3 });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, string id2)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2 });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, Guid id1, string id2, int id3)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2, id3.ToString() });

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, int id)
            => await HttpGet<T>(endpoint, id.ToString());

        public async Task<ApiResult<T>> HttpGet<T>(string endpoint, int id1, Guid id2)
            => await HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult> HttpPut(string endpoint, string id, object payload)
            => await HttpPut(endpoint, new[] { id }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, int id, object payload)
            => await HttpPut(endpoint, new[] { id.ToString() }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id, object payload)
            => await HttpPut(endpoint, new[] { id.ToString() }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, Guid id2, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2.ToString() }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, Guid id2, Guid id3, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, Guid id2, int id3, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, int id2, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2.ToString() }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, int id2, string id3, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2.ToString(), id3 }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, string id2, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2 }, payload);

        public async Task<ApiResult> HttpPut(string endpoint, Guid id1, string id2, int id3, object payload)
            => await HttpPut(endpoint, new[] { id1.ToString(), id2, id3.ToString() }, payload);

        public async Task<ApiResult> HttpDelete(string endpoint, string id)
            => await HttpDelete(endpoint, new[] { id });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id)
            => await HttpDelete(endpoint, id.ToString());

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, Guid id2)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, Guid id2, Guid id3)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, Guid id2, int id3)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, int id2)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, int id2, string id3)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString(), id3 });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, string id2)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2 });

        public async Task<ApiResult> HttpDelete(string endpoint, Guid id1, string id2, int id3)
            => await HttpDelete(endpoint, new[] { id1.ToString(), id2, id3.ToString() });

        public async Task<ApiResult> HttpDelete(string endpoint, int id)
            => await HttpDelete(endpoint, id.ToString());

        #endregion

        #region Wrappers (Assert, Count)

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id)
            => await HttpGet<bool>(endpoint, id.ToString());

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id1, Guid id2)
            => await HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id1, Guid id2, Guid id3)
            => await HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id1, int id2)
            => await HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString() });

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id1, int id2, Guid id3)
            => await HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id1, int id2, string id3)
            => await HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString(), id3 });

        public async Task<ApiResult<bool>> Assert(string endpoint, Guid id1, string id2)
            => await HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2 });

        public async Task<ApiResult<bool>> Assert(string endpoint, int id)
            => await HttpGet<bool>(endpoint, id);

        public async Task<ApiResult<bool>> Assert(string endpoint, int id1, Guid id2)
            => await HttpGet<bool>(endpoint, id1, id2);

        public async Task<ApiResult<bool>> Assert(string endpoint, string id)
            => await HttpGet<bool>(endpoint, id);

        public async Task<ApiResult<int>> Count(string endpoint, Dictionary<string, string> parameters)
            => await HttpGet<int>(endpoint, null, parameters);

        #endregion
    }

    public class ApiClientSynchronous : ApiClientBase
    {
        public ApiClientSynchronous(IHttpClientFactory httpClientFactory, IJsonSerializerBase serializer)
            : base(httpClientFactory, serializer)
        {

        }

        #region Requests (GET, POST, PUT, DELETE)

        public ApiResult<T> HttpGet<T>(string endpoint, string[] segments = null, Dictionary<string, string> parameters = null)
        {
            return TaskRunner.RunSync(() =>
            {
                var client = new ApiClient(_httpClientFactory, _serializer);

                return client.HttpGet<T>(endpoint, segments, parameters);
            });
        }

        public ApiResult<T> HttpPost<T>(string endpoint, object payload)
        {
            return TaskRunner.RunSync(() =>
            {
                var client = new ApiClient(_httpClientFactory, _serializer);

                return client.HttpPost<T>(endpoint, payload);
            });
        }

        public ApiResult HttpPost(string endpoint, object payload)
        {
            return TaskRunner.RunSync(() =>
            {
                var client = new ApiClient(_httpClientFactory, _serializer);

                return client.HttpPost(endpoint, payload);
            });
        }

        public ApiResult HttpPut(string endpoint, string[] segments, object payload)
        {
            return TaskRunner.RunSync(() =>
            {
                var client = new ApiClient(_httpClientFactory, _serializer);

                return client.HttpPut(endpoint, segments, payload);
            });
        }

        public ApiResult HttpDelete(string endpoint, string[] segments)
        {
            return TaskRunner.RunSync(() =>
            {
                var client = new ApiClient(_httpClientFactory, _serializer);

                return client.HttpDelete(endpoint, segments);
            });
        }

        #endregion

        #region Wrappers (GET, PUT, DELETE)

        public ApiResult<T> HttpGet<T>(string endpoint, string id)
            => HttpGet<T>(endpoint, new[] { id });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id)
            => HttpGet<T>(endpoint, id.ToString());

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, Guid id2)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString() });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, Guid id2, Guid id3)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, Guid id2, int id3)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, int id2)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString() });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, int id2, Guid id3)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, int id2, string id3)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2.ToString(), id3 });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, string id2)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2 });

        public ApiResult<T> HttpGet<T>(string endpoint, Guid id1, string id2, int id3)
            => HttpGet<T>(endpoint, new[] { id1.ToString(), id2, id3.ToString() });

        public ApiResult<T> HttpGet<T>(string endpoint, int id)
            => HttpGet<T>(endpoint, id.ToString());

        public void HttpPut(string endpoint, string id, object payload)
            => HttpPut(endpoint, new[] { id }, payload);

        public void HttpPut(string endpoint, int id, object payload)
            => HttpPut(endpoint, id.ToString(), payload);

        public void HttpPut(string endpoint, Guid id, object payload)
            => HttpPut(endpoint, id.ToString(), payload);

        public void HttpPut(string endpoint, Guid id1, Guid id2, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2.ToString() }, payload);

        public void HttpPut(string endpoint, Guid id1, Guid id2, Guid id3, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() }, payload);

        public void HttpPut(string endpoint, Guid id1, Guid id2, int id3, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() }, payload);

        public void HttpPut(string endpoint, Guid id1, int id2, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2.ToString() }, payload);

        public void HttpPut(string endpoint, Guid id1, int id2, string id3, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2.ToString(), id3 }, payload);

        public void HttpPut(string endpoint, Guid id1, string id2, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2 }, payload);

        public void HttpPut(string endpoint, Guid id1, string id2, int id3, object payload)
            => HttpPut(endpoint, new[] { id1.ToString(), id2, id3.ToString() }, payload);

        public void HttpDelete(string endpoint, string id)
            => HttpDelete(endpoint, new[] { id });

        public void HttpDelete(string endpoint, Guid id)
            => HttpDelete(endpoint, id.ToString());

        public void HttpDelete(string endpoint, Guid id1, Guid id2)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString() });

        public void HttpDelete(string endpoint, Guid id1, Guid id2, Guid id3)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public void HttpDelete(string endpoint, Guid id1, Guid id2, int id3)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public void HttpDelete(string endpoint, Guid id1, int id2)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString() });

        public void HttpDelete(string endpoint, Guid id1, int id2, string id3)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2.ToString(), id3 });

        public void HttpDelete(string endpoint, Guid id1, string id2)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2 });

        public void HttpDelete(string endpoint, Guid id1, string id2, int id3)
            => HttpDelete(endpoint, new[] { id1.ToString(), id2, id3.ToString() });

        public void HttpDelete(string endpoint, int id)
            => HttpDelete(endpoint, id.ToString());

        #endregion

        #region Wrappers (Assert, Count)

        public ApiResult<bool> Assert(string endpoint, Guid id)
            => HttpGet<bool>(endpoint, id.ToString());

        public ApiResult<bool> Assert(string endpoint, Guid id1, Guid id2)
            => HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString() });

        public ApiResult<bool> Assert(string endpoint, Guid id1, Guid id2, Guid id3)
            => HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public ApiResult<bool> Assert(string endpoint, Guid id1, int id2)
            => HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString() });

        public ApiResult<bool> Assert(string endpoint, Guid id1, int id2, Guid id3)
            => HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString(), id3.ToString() });

        public ApiResult<bool> Assert(string endpoint, Guid id1, int id2, string id3)
            => HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2.ToString(), id3 });

        public ApiResult<bool> Assert(string endpoint, Guid id1, string id2)
            => HttpGet<bool>(endpoint, new string[] { id1.ToString(), id2 });

        public ApiResult<bool> Assert(string endpoint, int id)
            => HttpGet<bool>(endpoint, id);

        public ApiResult<bool> Assert(string endpoint, string id)
            => HttpGet<bool>(endpoint, id);

        public ApiResult<int> Count(string endpoint, Dictionary<string, string> parameters)
            => HttpGet<int>(endpoint, null, parameters);

        #endregion
    }
}