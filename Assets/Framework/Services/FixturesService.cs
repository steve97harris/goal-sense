using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Services
{
    public class FixturesService : ApiService
    {
        public static async Task<ApiResponse<List<Fixture>>> GetFixturesAsync(string dateFrom, string dateTo)
        {
            try
            {
                const string endpoint = "fixtures";
                return await GetAsync<ApiResponse<List<Fixture>>>(endpoint, 
                    new Dictionary<string, string>
                    {
                        { "dateFrom", dateFrom },
                        { "dateTo", dateTo }
                    });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<List<Fixture>>();
            }
        }
        
        public static async Task<ApiResponse<List<Fixture>>> GetPremierLeagueFixturesAsync()
        {
            try
            {
                const string endpoint = "fixtures/season-fixtures";
                const string premierLeagueId = "2021";
                return await GetAsync<ApiResponse<List<Fixture>>>(endpoint, 
                    new Dictionary<string, string>
                    {
                        { "leagueId", premierLeagueId }
                    });;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<List<Fixture>>();
            }
        }
    }
}