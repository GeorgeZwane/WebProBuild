using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Configs;

namespace WebSpaceApp.Services
{
    public class MaterialService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        private const string CreateMaterialPath = "api/materials";
        private const string GetAllMaterialsPath = "api/materials/api/Getmaterials";

        public MaterialService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _baseUrl = apiSettings.Value.BaseUrl;
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        private void SetAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<HttpResponseMessage> CreateMaterialAsync(AddMaterialsProjectDTO materialDTO, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonContent = JsonConvert.SerializeObject(materialDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            Console.WriteLine($"Attempting to POST new Material to: {_baseUrl}api/material/createMaterial");
            Console.WriteLine($"Material Data: {jsonContent}");
            HttpResponseMessage response = await _httpClient.PostAsync("api/web/material/createMaterial", content); // Fixed endpoint
            Console.WriteLine($"API Create Material Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Create Material Error Content: {errorContent}");
            }
            return response;
        }

        public async Task<HttpResponseMessage> GetAllMaterialsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Attempting to GET all materials from: {_baseUrl}api/material/getAllMaterials");
            HttpResponseMessage response = await _httpClient.GetAsync("api/web/material/getAllMaterials");

            
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response Content: {content}");
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
            }

            return response;
        }
    }
}
