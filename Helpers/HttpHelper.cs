using Newtonsoft.Json;
using System.Text;

namespace Asis_Batia.Helpers;

public class HttpHelper {

    readonly HttpClient _httpClient;

    public HttpHelper() {
        _httpClient = new HttpClient {
            BaseAddress = new Uri(Constants.API_BASE_URL)
        };

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<Response> GetAsync<Response>(string url, CancellationToken cancellationToken = default) {
        Response response = default;

        try {
            HttpResponseMessage result = await _httpClient.GetAsync(url, cancellationToken);

            if(result.IsSuccessStatusCode) {
                response = JsonConvert.DeserializeObject<Response>(result.Content.ReadAsStringAsync().Result);
            }
        } catch(Exception) { }

        return response;
    }

    public async Task<Response> PostBodyAsync<Request, Response>(string uri, Request objet) {
        Response response = default;

        try {
            string json = JsonConvert.SerializeObject(objet);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await _httpClient.PostAsync(uri, stringContent);

            if(result.IsSuccessStatusCode) {
                response = JsonConvert.DeserializeObject<Response>(result.Content.ReadAsStringAsync().Result);
            }
        } catch(Exception) { }

        return response;
    }

    public async Task<Response> PostMultipartAsync<Response>(string url, MultipartFormDataContent content) {
        Response response = default;

        try {
            HttpResponseMessage result = await _httpClient.PostAsync(url, content);

            if(result.IsSuccessStatusCode) {
                response = JsonConvert.DeserializeObject<Response>(result.Content.ReadAsStringAsync().Result);
            }
        } catch(Exception) { }

        return response;
    }
}

