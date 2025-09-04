using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Configs;

namespace WebSpaceApp.Services
{
    public class MilestoneService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public MilestoneService(IOptions<ApiSettings> apiSettings)
        {
            _baseUrl = apiSettings.Value.BaseUrl;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        // Existing method to add a new milestone
        public async Task<HttpResponseMessage> AddMilestoneAsync(MilestoneDTO dto)
        {
            // The rest of this method remains the same as your existing code...
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync($"api/webmilestone/{dto.TaskEntityId}/addMilestone", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: Status Code: {response.StatusCode}, Content: {errorContent}");
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}: {errorContent}");
                }
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException during milestone creation: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException during milestone creation: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during milestone creation: {ex.Message}");
                throw;
            }
        }

        // NEW METHOD to get milestones by task ID  
        public async Task<List<MilestoneViewModel>?> GetMilestonesAsync(Guid taskId)
        {
            // Construct the full API URL based on your Web API's endpoint
            string requestUrl = $"api/webmilestone/{taskId}/GetMilestonesByTaskID";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                // Handle HTTP success/failure codes
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    // Deserialize the JSON response into a list of MilestoneViewModel objects.
                    // This assumes your API returns a JSON array of milestones.
                    var milestones = JsonConvert.DeserializeObject<List<MilestoneViewModel>>(jsonResponse);
                    return milestones;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Return null if no milestones are found.
                    return null;
                }
                else
                {
                    // Log the error and throw an exception for other failure cases.
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching milestones: {ex.Message}");
                // Rethrow or handle the exception as appropriate for your application.
                throw;
            }
        }

        //public async Task<MilestoneViewModel?> GetMilestoneByIdAsync(Guid milestoneId)
        //{
        //    string requestUrl = $"api/webtasks/{milestoneId}/GetMilestonesByTaskID";

        //    try
        //    {
        //        var response = await _httpClient.GetAsync(requestUrl);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string jsonResponse = await response.Content.ReadAsStringAsync();
        //            var milestone = JsonConvert.DeserializeObject<MilestoneViewModel>(jsonResponse);
        //            return milestone;
        //        }
        //        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            string errorContent = await response.Content.ReadAsStringAsync();
        //            throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred while fetching milestone: {ex.Message}");
        //        throw;
        //    }
        //}
        // NEW METHOD to get milestones by task ID
        public async Task<List<MilestoneViewModel>?> GetMilestonesByTaskIdAsync(Guid taskId)
        {
            string requestUrl = $"api/webmilestone/{taskId}/GetMilestonesByTaskID";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var milestones = JsonConvert.DeserializeObject<List<MilestoneViewModel>>(jsonResponse);
                    return milestones;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching milestones: {ex.Message}");
                throw;
            }
        }

        // NEW METHOD: Fetches a single milestone's details by its ID.
        public async Task<MilestoneViewModel?> GetMilestoneByIdAsync(Guid milestoneId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/webmilestone/{milestoneId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var milestone = JsonConvert.DeserializeObject<MilestoneViewModel>(jsonResponse);
                    return milestone;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching milestone: {ex.Message}");
                throw;
            }
        }

        // NEW METHOD: Sends updated milestone data to the API.
        public async Task<HttpResponseMessage> UpdateMilestoneAsync(UpdateMilestoneDTO dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/webmilestone/{dto.Id}/updateMilestone", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: Status Code: {response.StatusCode}, Content: {errorContent}");
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}: {errorContent}");
                }
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException during milestone update: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException during milestone update: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during milestone update: {ex.Message}");
                throw;
            }
        }




    }
}
