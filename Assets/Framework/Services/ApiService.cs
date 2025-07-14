using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Framework.Services
{
    public class ApiService
    {
        private const string BASE_URL = "http://localhost:5007";

        [Serializable]
        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }
        }
        
        // GET request method
        public static async Task<T> GetAsync<T>(string endpoint)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{BASE_URL}/{endpoint}"))
            {
                // Set headers
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                    throw new Exception(request.error);
                
                var response = request.downloadHandler.text;
                return JsonUtility.FromJson<T>(response);
            }
        }

        // POST request method
        public static async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var jsonData = JsonUtility.ToJson(data);

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
                return JsonUtility.FromJson<TResponse>(response);
            }
        }
    }
}