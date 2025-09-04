
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebSpaceApp.DTOs;
using WebSpaceInnovatorsWebApp.Configs;
public class TaskService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TaskService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _baseUrl = apiSettings.Value.BaseUrl; 
        _httpClient.BaseAddress = new Uri(_baseUrl); 
    }

   
    
    public async Task<HttpResponseMessage> GetAllTaskAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine($"Attempting to GET all tasks from: {_baseUrl}api/task");
        HttpResponseMessage response = await _httpClient.GetAsync("api/webtask/getAllTask"); // Adjust API route
        return response;
    }

    public async Task<HttpResponseMessage> GetAllTaskByIDAsync(string token, int projectId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var url = $"api/webtask/getAllTaskByProjectID/{projectId}";

        Console.WriteLine($"Requesting URL: {url}");

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        return response;
    }

    public async Task<HttpResponseMessage> GetAllTaskByUserIDAsync(string token, int userId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var url = $"api/webtask/getAllTaskByUserID/{userId}";

        Console.WriteLine($"Requesting URL: {url}");

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        return response;
    }

    public async Task<HttpResponseMessage> GetLeaderboardAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var url = $"api/webtask/getLeaderboard/";

        Console.WriteLine($"Requesting URL: {url}");

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        return response;
    }

    public async Task<HttpResponseMessage> GetNotificationsAsync(string token,int foremanid)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var url = $"api/webtask/getMessagesforforeman/{foremanid}";

        Console.WriteLine($"Requesting URL: {url}");

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        return response;
    }

    public async Task<HttpResponseMessage> SendNotificationAsync(string token, SendNotificationDTO dto)
    {
        // Set the bearer token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = JsonConvert.SerializeObject(dto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        Console.WriteLine($"Attempting to POST new notification to: {_baseUrl}api/webtask");
        Console.WriteLine($"Notification Data: {jsonContent}");

        HttpResponseMessage response = await _httpClient.PostAsync("api/webtask/sendmessage", content);

        Console.WriteLine($"API Send Notification Response Status: {response.StatusCode}");
        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Send Notification Error Content: {errorContent}");
        }

        return response;

    }


    public async Task<HttpResponseMessage> GetTaskCountsByProjectIdAsync(string token, int projectId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        string url = $"api/projects/countTasksByProject/{projectId}";
        Console.WriteLine($"Attempting to GET task counts for Project ID {projectId} from: {_baseUrl}{url}");

        HttpResponseMessage response = await _httpClient.GetAsync($"{_baseUrl}/{url}");

        Console.WriteLine($"API Task Count Response Status: {response.StatusCode}");
        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Task Count Error Content: {errorContent}");
        }

        return response;
    }

    //public async Task<HttpResponseMessage> CreateTaskAsync(AddTaskDTO taskDto, string token)
    //{
    //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    //    var jsonContent = JsonConvert.SerializeObject(taskDto);
    //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


    //    HttpResponseMessage response = await _httpClient.PostAsync("api/task/createTask", content);

    //    return response;
    //}



    public async Task<HttpResponseMessage> CreateTaskAsync(AddTaskDTO taskDto, string token)
    {

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


        var jsonContent = JsonConvert.SerializeObject(taskDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        Console.WriteLine($"Attempting to POST new task to: {_baseUrl}api/task");
        Console.WriteLine($"Project Data: {jsonContent}");

        HttpResponseMessage response = await _httpClient.PostAsync("api/webtask/createTask", content);

        Console.WriteLine($"API Create Task Response Status: {response.StatusCode}");
        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Create Task Error Content: {errorContent}");
        }

        return response;
    }

    public async Task<HttpResponseMessage> UpdateTaskAsync(TaskDTO taskDto, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var jsonContent = JsonConvert.SerializeObject(taskDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        Console.WriteLine($"Attempting to PUT task {taskDto.Id} to: {_baseUrl}api/tasks/{taskDto.Id}. Data: {jsonContent}");
        HttpResponseMessage response = await _httpClient.PutAsync($"api/webtask{taskDto.Id}", content); // Adjust API route
        return response;
    }

    public async Task<HttpResponseMessage> DeleteTaskAsync(int id, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine($"Attempting to DELETE task {id} from: {_baseUrl}api/tasks/{id}");
        HttpResponseMessage response = await _httpClient.DeleteAsync($"api/webtask/{id}"); // Adjust API route
        return response;
    }

    public async Task<HttpResponseMessage> GetAllUsersAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine($"Attempting to GET all tasks from: {_baseUrl}api/webtask");
        HttpResponseMessage response = await _httpClient.GetAsync("api/webtask/getAllUsers"); // Adjust API route
        return response;
    }



    public async Task<HttpResponseMessage> GetMilestonesByTaskIdAsync(Guid taskId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");

     
        var url = $"api/webtasks/{taskId}/GetMilestonesByTaskID";

        Console.WriteLine($"Attempting to GET Milestone for Task ID {taskId} from: {url}");

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        return response;

    }

    public async Task<HttpResponseMessage> AddMilestoneAsync(MilestoneDTO dto)
    {
        try
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            Console.WriteLine($"Attempting to POST to: {_baseUrl}api/webtasks/{dto.TaskEntityId}/addmilestone");
            Console.WriteLine($"Request Body: {json}");

            HttpResponseMessage response = await _httpClient.PostAsync($"api/webtasks/{dto.TaskEntityId}/addmilestone", content);

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
            Console.WriteLine($"HttpRequestException during milestone: {ex.Message}");
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

    public async Task<List<SelectListItem>> GetUsersForDropdownAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await _httpClient.GetAsync("api/webtask/getAllForemen");

        if (!response.IsSuccessStatusCode)
            return new List<SelectListItem>(); // or handle error

        var json = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<UserDto>>(json);

        var selectList = users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name
        }).ToList();

        return selectList;
    }

} 