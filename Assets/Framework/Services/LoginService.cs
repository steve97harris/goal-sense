using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Services
{
    public class LoginService : ApiService
    {
        [Serializable]
        public class LoginRequest
        {
            public string Email;
            public string Password;
        }

        [Serializable]
        public class LoginResponse
        {
            public string Token;
            public string UserId;
            public string Email;
        }

        public static async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
        {
            try
            {
                const string endpoint = "/usermanagement/login";

                // Create the request data
                var loginData = new LoginRequest
                {
                    Email = email,
                    Password = password
                };
                
                return await PostAsync<LoginRequest, ApiResponse<LoginResponse>>(endpoint, loginData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<LoginResponse> { Success = false, Message = ex.Message };
            }
        }
    }
}