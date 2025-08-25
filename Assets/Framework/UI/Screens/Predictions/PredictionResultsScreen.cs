using System;
using System.Linq;
using Framework.Extensions;
using Framework.Services;
using Framework.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Screens
{
    public class PredictionResultsScreen : Screen, IScreenData
    {
        public override ScreenName screenName => ScreenName.PredictionResultsScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static PredictionResultsScreen instance;
        
        private static UserData UserData { get; set; }
        
        [SerializeField] private TMP_Text usernameHeader;
        [SerializeField] private PredictionResultTableRow predictionResultTableRow;
        [SerializeField] private Transform tableContent;
        
        public void SetScreenData(object data)
        {
            if (data is UserData userData)
                UserData = userData;
        }
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            usernameHeader.text = string.IsNullOrEmpty(UserData.UserName) ? 
                "Prediction results" : 
                $"{UserData.UserName}'s Prediction results";
            LoadTable();
        }

        private async void LoadTable()
        {
            try
            {
                var response = await PredictionsService.GetPredictionsAsync(UserData.UserId.ToString());
                if (!response.success)
                {
                    Debug.LogError($"Error failed to load user predictions.\n{response.message}");
                    return;
                }

                var fixturesResponse = await FixturesService.GetPremierLeagueFixturesAsync();
                if (!fixturesResponse.success)
                {
                    Debug.LogError($"Error failed to load fixtures.\n{response.message}");
                    return;
                }
                
                var dateTimeNowGmt = DateTime.UtcNow.ConvertUtcTimeToGmt();
                var fixtures = fixturesResponse.data!
                    .Where(x => x.Kickoff < dateTimeNowGmt)
                    .OrderBy(x => x.Kickoff)
                    .ToDictionary(x => x.Id);
                
                foreach (Transform child in tableContent)
                    Destroy(child.gameObject);
                
                var predictions = response.data!
                    .ToDictionary(x => x.FixtureId);
                
                foreach (var data in fixtures)   
                {
                    var fixture = data.Value;
                    var row = Instantiate(predictionResultTableRow, tableContent);
                    if (!predictions.TryGetValue(fixture.Id, out var prediction))
                    {
                        row.prediction.text = $"Prediction not found";
                        row.pointsAwarded.text = $"0 pts";
                    }
                    else
                    {
                        row.prediction.text = $"{prediction.PredictedHomeScore} - {prediction.PredictedAwayScore}";
                        row.pointsAwarded.text = $"{prediction.PointsAwarded} pts";
                    }
                    row.match.text = $"{fixture.HomeTeam.ToFriendlyTeamName()} vs {fixture.AwayTeam.ToFriendlyTeamName()}\nResult: {fixture.HomeScore} - {fixture.AwayScore}";
                    row.date.text = $"Gw {fixture.Matchweek}\n{fixture.Kickoff.Date:dd/MM/yyyy}\n{fixture.Kickoff:hh:mm tt}";
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}