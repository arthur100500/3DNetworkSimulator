using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Requests;
using Newtonsoft.Json;

namespace Requests
{
    public class HttpRequests : IRequestMaker
    {
        private readonly HttpClient _httpClient;
        private readonly string _addrBegin;
        private readonly string _base64Authorization;
    
        public HttpRequests(string addressBegin, string user, string password)
        {
            _httpClient = new HttpClient();
            _addrBegin = addressBegin;
            _base64Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        }
    
        public void MakeGetRequest(string url)
        {
            var task = MakeRequestAsync(url, "GET");
            task.Wait();
        }

        public T MakeGetRequest<T>(string url, string data)
        {
            var task = MakeRequestAsync(url, "GET", data);
            return JsonConvert.DeserializeObject<T>(task.GetAwaiter().GetResult());
        }

        public void MakePostRequest(string url, string data)
        {
            var task = MakeRequestAsync(url, "POST", data);
            task.Wait();
        }

        public T MakeGetRequest<T>(string url)
        {
            var task = MakeRequestAsync(url, "GET");
            return JsonConvert.DeserializeObject<T>(task.GetAwaiter().GetResult());
        }

        public T MakePostRequest<T>(string url, string data)
        {
            var task = MakeRequestAsync(url, "POST", data);
            return JsonConvert.DeserializeObject<T>(task.GetAwaiter().GetResult());
        }

        public void MakeDeleteRequest(string url, string data)
        {
            var task = MakeRequestAsync(url, "DELETE");
            task.Wait();
        }
    
        private async Task<string> MakeRequestAsync(string endpoint, string type)
        {
            using var request = new HttpRequestMessage(new HttpMethod(type), _addrBegin + endpoint);
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {_base64Authorization}");
        
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async Task<string> MakeRequestAsync(string endpoint, string type, string data)
        {
            using var request = new HttpRequestMessage(new HttpMethod(type), _addrBegin + endpoint);
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {_base64Authorization}");

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
