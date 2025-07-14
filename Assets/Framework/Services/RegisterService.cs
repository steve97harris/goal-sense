using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Services
{
    public class RegisterService : ApiService
    {
        [Serializable]
        public class RegisterRequest
        {
            public string Email;
            public string Password;
            public string ConfirmPassword;
        }

        [Serializable]
        public class RegisterResponse
        {
            public string Token;
            public string UserId;
            public string Email;
        }

        public static async Task<ApiResponse<RegisterResponse>> RegisterAsync(string email, string password, string confirmPassword)
        {
            try
            {
                const string endpoint = "/usermanagement/register";

                // Create the request data
                var registerData = new RegisterRequest
                {
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword
                };
                
                return await PostAsync<RegisterRequest, ApiResponse<RegisterResponse>>(endpoint, registerData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<RegisterResponse> { Success = false, Message = ex.Message };
            }
        }
    }
}