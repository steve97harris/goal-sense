using Framework.Extensions;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class PredictionsScreen : Screen
    {
        public override ScreenName screenName => ScreenName.PredictionsScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static PredictionsScreen Instance;
        
        [SerializeField] private GameweekButton _gameweekButton;
        [SerializeField] private PredictionView _predictionView;
        [SerializeField] private Transform _gameweeksContent;
        [SerializeField] private Transform _predictionsContent;

        private string _currentGameweek;
        private List<PredictionView> _predictionPanels = new List<PredictionView>();
        private List<GameweekButton> _gameweekButtons = new List<GameweekButton>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this.gameObject);
            Initialize();
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(_currentGameweek))
                TriggerGameweekButton(_currentGameweek);
        }

        private void OnApplicationQuit()
        {
            foreach (Transform child in _predictionsContent)
                Destroy(child.gameObject);
            foreach (Transform child in _gameweeksContent)
                Destroy(child.gameObject);
        }

        private async void Initialize()
        {
            Debug.Log("Initialize predictions screen");
            var existingPredictions = new List<PredictionsService.Prediction>();
            var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("User ID is null, please login");
                return;
            }
            var response = await PredictionsService.GetPredictionsAsync(userId);
            if (response.success && response.data != null)
                existingPredictions = response.data;

            var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
            var premierLeagueFixtures = await FixturesService.GetPremierLeagueFixturesAsync();
            _currentGameweek = GetCurrentGameweek(premierLeagueFixtures, dateTimeNowGmt);
            var gameweeks = premierLeagueFixtures.Select(x => x.Matchweek).Distinct().OrderBy(int.Parse).ToList();
            LoadGameweekButtons(gameweeks);
            LoadMatchesForPredictions(premierLeagueFixtures, existingPredictions, dateTimeNowGmt);
        }

        private void TriggerGameweekButton(string gameweek)
        {
            var gameweekButton = _gameweekButtons.FirstOrDefault(x => x.Gameweek == gameweek);
            if (gameweekButton == null)
                return;
            gameweekButton.button.onClick.Invoke();
        }

        private void LoadMatchesForPredictions(List<FixturesService.Fixture> premierLeagueFixtures,
            List<PredictionsService.Prediction> existingPredictions, DateTime dateTimeNowGmt)
        {
            foreach (Transform child in _predictionsContent)
                Destroy(child.gameObject);

            _predictionPanels = new List<PredictionView>();
            foreach (var fixture in premierLeagueFixtures)
            {
                var predictionPanel = Instantiate(this._predictionView, _predictionsContent);
                predictionPanel.Fixture = fixture;
                predictionPanel.homeTeam.text = fixture.HomeTeam;
                predictionPanel.awayTeam.text = fixture.AwayTeam;
                predictionPanel.dateTime.text = fixture.Kickoff.ToString("HH:mm dd/MM/yyyy");
                
                // set prediction if already submitted
                var existingPrediction = existingPredictions.FirstOrDefault(x => x.FixtureId == fixture.Id);
                if (existingPrediction != default)
                {
                    predictionPanel.homeScoreInput.text = existingPrediction.PredictedHomeScore.ToString();
                    predictionPanel.awayScoreInput.text = existingPrediction.PredictedAwayScore.ToString();
                }
                
                // lock prediction if game started
                predictionPanel.Locked = fixture.Kickoff >= dateTimeNowGmt;
                _predictionPanels.Add(predictionPanel);
            }
        }

        private void LoadGameweekButtons(List<string> gameweeks)
        {
            foreach (Transform child in _gameweeksContent)
                Destroy(child.gameObject);

            _gameweekButtons = new List<GameweekButton>();
            foreach (var gameweekNumber in gameweeks)
            {
                var gameweekBtn = Instantiate(_gameweekButton, _gameweeksContent);
                gameweekBtn.Gameweek = gameweekNumber;
                gameweekBtn.text.text = $"Gameweek {gameweekNumber}";
                _gameweekButtons.Add(gameweekBtn);
            }
        }

        public void EnableGameweekFixtures(string gameweek)
        {
            foreach (var predictionPanel in _predictionPanels)
                predictionPanel.gameObject.SetActive(predictionPanel.Fixture.Matchweek == gameweek);
        }

        public void SetGameweekButtonsView(GameweekButton gameweekButton)
        {
            foreach (var btn in _gameweekButtons)
                btn.underline.gameObject.SetActive(false);
            gameweekButton.underline.gameObject.SetActive(true);
        }

        private static string GetCurrentGameweek(List<FixturesService.Fixture> fixtures, DateTime dateTimeNowGmt)
        {
            fixtures.Sort((a, b) => a.Kickoff.CompareTo(b.Kickoff));

            var currentGameweek = "1";
            foreach (var fixture in fixtures)
            {
                if (fixture.Kickoff > dateTimeNowGmt)
                {
                    if ((fixture.Kickoff - dateTimeNowGmt).TotalHours <= 2)
                    {
                        currentGameweek = fixture.Matchweek;
                    }
                    else
                    {
                        var previousFixture = fixtures
                            .Where(f => f.Kickoff < dateTimeNowGmt)
                            .OrderByDescending(f => f.Kickoff)
                            .FirstOrDefault();
                            
                        currentGameweek = previousFixture != null ? 
                            previousFixture.Matchweek : fixture.Matchweek;

                    }
                    break;
                }
                currentGameweek = fixture.Matchweek;
            }
            return currentGameweek;
        }
    }
}