using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Services
{
    public class PredictionsService : ApiService
    {
        [Serializable]
        public class PredictionRequest
        {
            public Guid UserId { get; set; }
            public Guid FixtureId { get; set; }
            public int HomeScore { get; set; }
            public int AwayScore { get; set; }
        }
        
        public static async Task<ApiResponse<Prediction>> GetPredictionAsync(string userId, string fixtureId)
        {
            try
            {
                const string endpoint = "predictions/get-prediction";

                var response = await GetAsync<ApiResponse<Prediction>>(endpoint, 
                    new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "fixtureId", fixtureId }
                    });
                return response;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<Prediction> { success = false, message = ex.Message };
            }
        }
        
        public static async Task<ApiResponse<List<Prediction>>> GetPredictionsAsync(string userId)
        {
            try
            {
                const string endpoint = "predictions/get-predictions";

                return await GetAsync<ApiResponse<List<Prediction>>>(endpoint, 
                    new Dictionary<string, string>
                    {
                        { "userId", userId }
                    });;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<List<Prediction>> { success = false, message = ex.Message };
            }
        }

        public static async Task<ApiResponse<Prediction>> SubmitPredictionAsync(string userId, 
            string fixtureId, int homeScore, int awayScore)
        {
            try
            {
                const string endpoint = "predictions/submit-prediction";

                // Create the request data
                var requestData = new PredictionRequest
                {
                    UserId = Guid.Parse(userId),
                    FixtureId = Guid.Parse(fixtureId),
                    HomeScore = homeScore,
                    AwayScore = awayScore
                };
                
                return await PostAsync<PredictionRequest, ApiResponse<Prediction>>(endpoint, requestData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<Prediction> { success = false, message = ex.Message };
            }
        }
        
        public static async Task<ApiResponse<Prediction>> UpdatePredictionAsync(string userId, 
            string fixtureId, int homeScore, int awayScore)
        {
            try
            {
                const string endpoint = "predictions/update-prediction";

                // Create the request data
                var requestData = new PredictionRequest
                {
                    UserId = Guid.Parse(userId),
                    FixtureId = Guid.Parse(fixtureId),
                    HomeScore = homeScore,
                    AwayScore = awayScore
                };
                
                return await PostAsync<PredictionRequest, ApiResponse<Prediction>>(endpoint, requestData);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<Prediction> { success = false, message = ex.Message };
            }
        }
    }
}