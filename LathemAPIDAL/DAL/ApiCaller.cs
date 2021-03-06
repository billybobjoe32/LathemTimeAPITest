using LathemAPIDAL.Models;
using System.Configuration;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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
                    List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(data);
                    return employees;
                }
                catch
                {
                    throw new ApplicationException("Failed to deserialize employee data.");
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

            string punchJson = JsonConvert.SerializeObject(punch);

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
                    PunchResponse punchResponse = JsonConvert.DeserializeObject<PunchResponse>(data);
                    return punchResponse;
                }
                catch
                {
                    throw new ApplicationException("Failed to deserialize punch data.");
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
                try
                {
                    VersionResponse versionResponse = JsonConvert.DeserializeObject<VersionResponse>(data);
                    return versionResponse;
                }
                catch
                {
                    throw new ApplicationException("Failed to deserialize version data.");
                }
            }
            else
            {
                throw new Exception("Api request failed with response: " + response.StatusCode);
            }
        }
    }
}