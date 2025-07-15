using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
namespace Framework.Services
{
    public class RegisterService : ApiService
    {
        [Serializable]
        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }
        
        public static async Task<ApiResponse<UserAuthResponse>> RegisterAsync(string email, string password, string confirmPassword)
        {
            try
            {
                const string endpoint = "usermanagement/register";

                // Create the request data
                var registerData = new RegisterRequest
                {
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword
                };
                
                return await PostAsync<RegisterRequest, ApiResponse<UserAuthResponse>>(endpoint, registerData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<UserAuthResponse> { success = false, message = ex.Message };
            }
        }
    }
}