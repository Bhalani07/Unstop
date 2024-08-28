using Unstop_Utility;
using Newtonsoft.Json;
using System.Text;
using Unstop.Models;
using Unstop.Services.IServices;
using System.Net.Http.Headers;
using Unstop.Models.DTO;

namespace Unstop.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse ResponseModel { get; set; }

        public IHttpClientFactory HttpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            ResponseModel = new();
            HttpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                HttpClient client = HttpClient.CreateClient("UnstopAPI");

                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }

                switch (apiRequest.ApiType)
                {
                    case StaticDetails.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;

                    case StaticDetails.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;

                    case StaticDetails.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;

                    case StaticDetails.ApiType.PATCH:
                        message.Method = HttpMethod.Patch;
                        break;

                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                }

                HttpResponseMessage apiResponse = await client.SendAsync(message);

                string apiContent = await apiResponse.Content.ReadAsStringAsync();
                T APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

                return APIResponse;
            }
            catch (Exception ex)
            {
                APIResponse apiResponse = new()
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };

                string response = JsonConvert.SerializeObject(apiResponse);
                T APIResponse = JsonConvert.DeserializeObject<T>(response);

                return APIResponse;
            }
        }
    }
}
