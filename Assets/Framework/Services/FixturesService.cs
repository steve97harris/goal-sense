using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Services
{
    public class FixturesService : ApiService
    {
        [Serializable]
        public class Fixture
        {
            public Guid Id { get; set; }
            public int LeagueId { get; set; } 
            public int ExternalId { get; set; } 
            public string Matchweek { get; set; } 
            public string HomeTeam { get; set; } 
            public string AwayTeam { get; set; } 
            public string HomeTeamLogo { get; set; } 
            public string AwayTeamLogo { get; set; }
            public DateTime Kickoff { get; set; } 
            public string Status { get; set; }
            public int HomeScore { get; set; }
            public int AwayScore { get; set; }
            public string Location { get; set; }
        }

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