using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
namespace Framework.Services
{
    public class LoginService : ApiService
    {
        [Serializable]
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public static async Task<ApiResponse<UserAuthResponse>> LoginAsync(string email, string password)
        {
            try
            {
                const string endpoint = "usermanagement/login";

                // Create the request data
                var loginData = new LoginRequest
                {
                    Email = email,
                    Password = password
                };
                
                return await PostAsync<LoginRequest, ApiResponse<UserAuthResponse>>(endpoint, loginData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<UserAuthResponse> { success = false, message = ex.Message };
            }
        }
    }
}