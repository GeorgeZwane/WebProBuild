using Microsoft.Extensions.Options; 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebSpaceApp;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Configs; 
using WebSpaceInnovatorsWebApp.DTOs;

namespace WebSpaceInnovatorsWebApp.Services
{
   
    public class ProjectServices
    {
        private readonly HttpClient _httpClient;
       
        private readonly string _baseUrl;


        public ProjectServices(IOptions<ApiSettings> apiSettings)
        {
           
            _baseUrl = apiSettings.Value.BaseUrl;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);

            Console.WriteLine($"ProjectServices initialized. Base URL: {_baseUrl}");
        }

        public async Task<HttpResponseMessage> GetAllTaskByIDAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/getAllTaskByProjectID/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            return response;
        }

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

        public async Task<HttpResponseMessage> GetTaskCountByPIDAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/countTasksByProject/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> GetTaskCompletedByPIDAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/getAllCompletedTaskByProjectID/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> GetTaskIncompleteByPIDAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/getAllInCompleteTaskByProjectID/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> GetTaskInProgressByPIDAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/getAllProgressTaskByProjectID/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> UpdateProjectAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/updateProject/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            // Ideally use PUT or POST for updates
            HttpResponseMessage response = await _httpClient.PutAsync(url, null);

            return response;
        }

        public async Task<HttpResponseMessage> UpdateTaskAsync(string token, int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"api/webtask/updateTaskUsingMilestones/{projectId}";

            Console.WriteLine($"Requesting URL: {url}");

            // Ideally use PUT or POST for updates
            HttpResponseMessage response = await _httpClient.PutAsync(url, null);

            return response;
        }


        public async Task<HttpResponseMessage> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"Attempting to POST to: {_baseUrl}api/webauth/register");
                Console.WriteLine($"Request Body: {json}");

                HttpResponseMessage response = await _httpClient.PostAsync("api/webauth/register", content);

                Console.WriteLine($"API Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error Content: {errorContent}");
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException during registration: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during registration: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> LoginAsync(LoginRequestDto dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");


                HttpResponseMessage response = await _httpClient.PostAsync("api/webauth/login", content);

                Console.WriteLine($"API Response Status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException during login: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during login: {ex.Message}");
                throw;
            }
        }

        

        public async Task<HttpResponseMessage> GetAllProjectsAsync(int userId,string token)
        {
          
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/webprojects/getAllProjectsByUserID?id={userId}";
            Console.WriteLine($"Attempting to GET all projects from: {_baseUrl}{url}");


         //   Console.WriteLine($"Attempting to GET all projects from: {_baseUrl}api/projects");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API All Projects Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API All Projects Error Content: {errorContent}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetAllProjectViewAsync(int userId, string token)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/webprojects/getAllProjects";
            Console.WriteLine($"Attempting to GET all projects from: {_baseUrl}{url}");


            //   Console.WriteLine($"Attempting to GET all projects from: {_baseUrl}api/projects");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API All Projects Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API All Projects Error Content: {errorContent}");
            }

            return response;
        }


        public async Task<HttpResponseMessage> GetAllUsersAsync(int userId, string token)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/Admin/getAllUsers";
            Console.WriteLine($"Attempting to GET all users from: {_baseUrl}{url}");


            //   Console.WriteLine($"Attempting to GET all projects from: {_baseUrl}api/projects");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API All Users Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API All Users Error Content: {errorContent}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetAllForemenAsync(string token)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/webtask/getAllForemen";
            Console.WriteLine($"Attempting to GET all users from: {_baseUrl}{url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API All Users Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API All Users Error Content: {errorContent}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> AssignUserRoleAsync(AssignRoleDto dto, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/Admin/assignRole";
            Console.WriteLine($"Attempting to POST role assignment to: {_baseUrl}{url}");

            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            Console.WriteLine($"API Assign Role Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Assign Role Error Content: {errorContent}");
            }

            return response;
        }



        public async Task<HttpResponseMessage> GetProjectAndTaskCountsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/webprojects/countAllProjectAndTasks"; 
            Console.WriteLine($"Attempting to GET counts from: {_baseUrl}{url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API Project/Task Counts Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Counts Error Content: {errorContent}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetProjectAndTaskCountsForEachAsync(string token,int projectId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/webprojects/countAllProjectAndTasksForEach/{projectId}";
            Console.WriteLine($"Attempting to GET counts from: {_baseUrl}{url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API Project/Task Counts Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Counts Error Content: {errorContent}");
            }

            return response;
        }




        public async Task<HttpResponseMessage> AddTeamProjectsAsync(TeamDto teamDto, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/CreateProjectTeam";
            Console.WriteLine($"Attempting to Create Project Team from: {_baseUrl}{url}");

            var jsonContent = JsonConvert.SerializeObject(teamDto);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);

            Console.WriteLine($"API Create Project Team Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Create Project Team Error: {errorContent}");
            }

            return response;
        }


        public async Task<HttpResponseMessage> CreateProjectAsync(ProjectDto projectDto, string token)
        {
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            
            var jsonContent = JsonConvert.SerializeObject(projectDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            Console.WriteLine($"Attempting to POST new project to: {_baseUrl}api/webprojects");
            Console.WriteLine($"Project Data: {jsonContent}");

            HttpResponseMessage response = await _httpClient.PostAsync("api/webprojects/createProjects", content);

            Console.WriteLine($"API Create Project Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Create Project Error Content: {errorContent}");
            }

            return response;
        }

        //===================================================================================================================================================================
        public async Task<HttpResponseMessage> CreateTeamAsync(TeamDto teamDto, string token)
        {
            // Set the Bearer token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize the object
            var jsonContent = JsonConvert.SerializeObject(teamDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Logging
            Console.WriteLine($"Attempting to POST new team to: {_baseUrl}api/webprojects/CreateProjectTeam");
            Console.WriteLine($"Team Data: {jsonContent}");

            // Send the POST request
            HttpResponseMessage response = await _httpClient.PostAsync("api/webprojects/CreateProjectTeam", content);

            Console.WriteLine($"API Create Team Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Create Team Error Content: {errorContent}");
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetAllTeamsAsync(string token)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"api/webprojects/GetAllProjectTeams";
            Console.WriteLine($"Attempting to GET all teams from: {_baseUrl}{url}");


            //   Console.WriteLine($"Attempting to GET all projects from: {_baseUrl}api/projects");

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"API All Projects Response Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API All Teams Error Content: {errorContent}");
            }

            return response;
        }
    }
}