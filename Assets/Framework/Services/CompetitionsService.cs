using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Services
{
    public class CompetitionsService : ApiService
    {
        public static async Task<ApiResponse<List<Competition>>> GetCompetitions()
        {
            try
            {
                const string endpoint = "competitions";
                return await GetAsync<ApiResponse<List<Competition>>>(endpoint);;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ApiResponse<List<Competition>>();
            }
        }
    }
}