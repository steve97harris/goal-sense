using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Framework.Services
{
    public class ApiService
    {
        private const string BASE_URL = "https://goal-sense-api.onrender.com";

        [Serializable]
        public class ApiResponse<T>
        {
            public bool success { get; set; }
            public string? message { get; set; }
            public T? data { get; set; }
        }
        
        [Serializable]
        public class UserAuthResponse
        {
            public string token { get; set; }
            public string userId { get; set; }
            public string email { get; set; }
            public string userFullName { get; set; }
        }
        
        // GET request method
        public static async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null)
        {
            var queryString = "";
            if (queryParams is {Count: > 0})
            {
                queryString = "?" + string.Join("&", queryParams.Select(kvp => 
                    $"{UnityWebRequest.EscapeURL(kvp.Key)}={UnityWebRequest.EscapeURL(kvp.Value)}"));
            }

            using (UnityWebRequest request = UnityWebRequest.Get($"{BASE_URL}/{endpoint}{queryString}"))
            {
                // Set headers
                request.SetRequestHeader("Content-Type", "application/json");
                
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                    throw new Exception(request.error);
                
                var response = request.downloadHandler.text;
                return JsonConvert.DeserializeObject<T>(response);
            }
        }

        // POST request method
        public static async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            
            using (UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/{endpoint}", "POST"))
            {
                var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                // Set headers
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                    throw new Exception(request.error);
                
                var response = request.downloadHandler.text;
                return JsonConvert.DeserializeObject<TResponse>(response);
            }
        }
    }
}