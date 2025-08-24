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
        // development
        // private const string BASE_URL = "http://localhost:5007";
        // private const string API_KEY = "dev-goalsense-7k9m2p4x8q1w5e3r7t9y2u6i8o0p3a5s";
        
        // production
        private const string BASE_URL = "https://goal-sense-api.onrender.com";
        private const string API_KEY = "prod-goalsense-9x4c7v2n8m1q5w3e7r9t2y6u4i8o0p5a3s7d9f1g4h6j8k";

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
                request.SetRequestHeader("x-api-key", API_KEY);;
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
                request.SetRequestHeader("x-api-key", API_KEY);
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