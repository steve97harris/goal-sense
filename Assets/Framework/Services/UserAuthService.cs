using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Services
{
    public class UserAuthService : ApiService
    {
        [Serializable]
        public class UpdateUserProfileRequest
        {
            public Guid UserId { get; set; }
            public string Email { get; set; }
            public string UserFullName { get; set; }
        }
        
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
        
        public static async Task<ApiResponse<UserAuthResponse>> UpdateProfileAsync(string userId, string email, string userFullName)
        {
            try
            {
                const string endpoint = "usermanagement/update-profile";
                var updateUserData = new UpdateUserProfileRequest
                {
                    UserId = Guid.Parse(userId),
                    Email = email,
                    UserFullName = userFullName
                };
                
                return await PostAsync<UpdateUserProfileRequest, ApiResponse<UserAuthResponse>>(endpoint, updateUserData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<UserAuthResponse> { success = false, message = ex.Message };
            }
        }
        
        public static async Task<ApiResponse<UserAuthResponse>> DeleteUserAccountAsync(string userId)
        {
            try
            {
                const string endpoint = "usermanagement/delete-account";
                return await GetAsync<ApiResponse<UserAuthResponse>>(endpoint, new Dictionary<string, string>()
                {
                    { "userId", userId }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<UserAuthResponse> { success = false, message = ex.Message };
            }
        }
    }
}