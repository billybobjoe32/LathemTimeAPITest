using LathemAPIDAL.Models;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LathemAPIDAL.DAL
{
    public class ApiCaller : IApiCaller
    {
        private const string CONTENT_TYPE = "application/json";
        private const string API_URL_EXTENSION_EMPLOYEES = "employees";
        private const string API_URL_EXTENSION_PUNCH = "punch";
        private const string API_URL_EXTENSION_VERSION = "version";
        private string _apiKey;
        private string _apiKeyValue;
        private string _baseURL;
        public ApiCaller()
        {
            _apiKey = ConfigurationManager.AppSettings["ApiKey"] ?? "";
            _apiKeyValue = ConfigurationManager.AppSettings["ApiKeyValue"] ?? "";
            _baseURL = ConfigurationManager.AppSettings["ApiBaseURL"] ?? "";
        }
        public async Task<List<Employee>> GetEmployees()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(_apiKey, _apiKeyValue);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(CONTENT_TYPE));

            HttpResponseMessage response = await client.GetAsync(_baseURL + API_URL_EXTENSION_EMPLOYEES);
            client.Dispose();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                // for some reason the string is coming back with backslashes and additional quotes
                data = data.Replace("\\", "");
                data = data.Substring(1, data.Length - 2);

                try
                {
                    List<Employee> employees = (List<Employee>)JsonSerializer.Deserialize(data, typeof(List<Employee>));
                    return employees;
                }
                catch
                {
                    throw new ApplicationException("Failed to deserialize object data.");
                }
            }
            else
            {
                throw new Exception("Api request failed with response: " + response.StatusCode);
            }
        }

        public async Task<PunchResponse> SendPunch(Punch punch)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(_apiKey, _apiKeyValue);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(CONTENT_TYPE));

            string punchJson = JsonSerializer.Serialize<Punch>(punch);

            HttpContent content = new StringContent(punchJson);

            HttpResponseMessage response = await client.PostAsync(_baseURL + API_URL_EXTENSION_PUNCH, content);
            client.Dispose();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                // for some reason the string is coming back with backslashes and additional quotes
                data = data.Replace("\\", "");
                data = data.Substring(1, data.Length - 2);

                try
                {
                    PunchResponse punchResponse = (PunchResponse)JsonSerializer.Deserialize(data, typeof(PunchResponse));
                    return punchResponse;
                }
                catch
                {
                    throw new ApplicationException("Failed to deserialize object data.");
                }
            }
            else
            {
                throw new Exception("Api request failed with response: " + response.StatusCode);
            }
        }

        public async Task<VersionResponse> GetVersion()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(_apiKey, _apiKeyValue);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(CONTENT_TYPE));

            HttpResponseMessage response = client.GetAsync(_baseURL + API_URL_EXTENSION_VERSION).Result;
            client.Dispose();
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                // for some reason the string is coming back with backslashes and additional quotes
                data = data.Replace("\\", "");
                data = data.Substring(1, data.Length - 2);

                VersionResponse versionResponse = (VersionResponse)JsonSerializer.Deserialize(data, typeof(VersionResponse));
                return versionResponse;
            }
            else
            {
                throw new Exception("Api request failed with response: " + response.StatusCode);
            }
        }
    }
}