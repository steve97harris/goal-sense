using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Services
{
    public class UserAuthService : ApiService
    {
        public static async Task<ApiResponse<UserAuthResponse>> GetUserProfileAsync(string userId)
        {
            try
            {
                const string endpoint = "usermanagement/profile";
                
                return await GetAsync<ApiResponse<UserAuthResponse>>(endpoint, new Dictionary<string, string>()
                {
                    { "userId", userId }
                });;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<UserAuthResponse> { success = false, message = ex.Message };
            }
        }
    }
}