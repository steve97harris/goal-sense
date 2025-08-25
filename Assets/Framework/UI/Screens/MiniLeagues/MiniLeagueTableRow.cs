using System;
using System.Linq;
using Framework.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Components
{
    public class MiniLeagueTableRow : MonoBehaviour
    {
        public TMP_Text position;
        public TMP_Text playerName;
        public TMP_Text gameweekPoints;
        public TMP_Text points;
        public Button button;

        private static StateMachine StateMachine => StateMachine.Instance;

        public void Initialize(MiniLeagueTableData data, string pos, Guid[] gwFixtures)
        {
            playerName.text = data.UserName;
            points.text = $"{data.TotalPoints.ToString()} pts";
            position.text = pos;
            LoadGameweekPredictionPoints(data.UserId.ToString(), gwFixtures);
            button.onClick.AddListener(() =>
            {
                StateMachine.ChangeState(ScreenName.PredictionResultsScreen, new UserData()
                {
                    UserId = data.UserId,
                    UserName = data.UserName
                });
            });
        }

        private async void LoadGameweekPredictionPoints(string userId, Guid[] gwFixtures)
        {
            try
            {
                var predictionsResponse = await PredictionsService.GetPredictionsAsync(userId);
                if (!predictionsResponse.success)
                {
                    Debug.LogError(predictionsResponse.message);
                    return;
                }
            
                var predictions = predictionsResponse.data!
                    .ToDictionary(x => x.FixtureId);
                var total = 0;
                foreach (var fixtureId in gwFixtures)
                {
                    if (predictions.TryGetValue(fixtureId, out var prediction))
                        total += prediction.PointsAwarded;
                }
                gameweekPoints.text = $"{total} pts";
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}