using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Services
{
    [Serializable]
    public class CreateMiniLeagueRequest
    {
        public Guid UserId { get; set; }
        public string LeagueName { get; set; }
    }
    
    [Serializable]
    public class JoinMiniLeagueRequest
    {
        public Guid UserId { get; set; }
        public string InviteCode { get; set; }
    }

    [Serializable]
    public class UpdateMiniLeagueRequest
    {
        public Guid MiniLeagueId { get; set; }
        public string LeagueName { get; set; }
    }
    
    public class MiniLeaguesService : ApiService
    {
        public static async Task<ApiResponse<List<MiniLeague>>> GetUsersMiniLeagues(string userId)
        {
            try
            {
                const string endpoint = "minileagues";
                return await GetAsync<ApiResponse<List<MiniLeague>>>(endpoint, 
                    new Dictionary<string, string>
                    {
                        { "userId", userId }
                    });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<List<MiniLeague>>();
            }
        }
        
        public static async Task<ApiResponse<MiniLeague>> CreateMiniLeague(Guid userId, string leagueName)
        {
            try
            {
                const string endpoint = "minileagues/create";
                return await PostAsync<CreateMiniLeagueRequest, ApiResponse<MiniLeague>>(endpoint, 
                    new CreateMiniLeagueRequest()
                    {
                        UserId = userId,
                        LeagueName = leagueName
                    });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<MiniLeague>();
            }
        }
        
        public static async Task<ApiResponse<MiniLeague>> JoinMiniLeague(Guid userId, string inviteCode)
        {
            try
            {
                const string endpoint = "minileagues/join";
                return await PostAsync<JoinMiniLeagueRequest, ApiResponse<MiniLeague>>(endpoint, 
                    new JoinMiniLeagueRequest()
                    {
                        UserId = userId,
                        InviteCode = inviteCode
                    });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<MiniLeague>();
            }
        }

        public static async Task<ApiResponse<List<MiniLeagueTableData>>> GetMiniLeagueTable(string miniLeagueId)
        {
            try
            {
                const string endpoint = "minileagues/table";
                return await GetAsync<ApiResponse<List<MiniLeagueTableData>>>(endpoint, 
                    new Dictionary<string, string>
                    {
                        { "miniLeagueId", miniLeagueId }
                    });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<List<MiniLeagueTableData>>();
            }
        }
    }
}