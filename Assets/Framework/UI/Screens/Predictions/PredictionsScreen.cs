using Framework.Extensions;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
namespace Framework.Screens
{
    public class PredictionsScreen : Screen
    {
        public override ScreenName screenName => ScreenName.PredictionsScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static PredictionsScreen Instance;
        
        [SerializeField] private GameweekButton gameweekButton;
        [SerializeField] private PredictionCard predictionCard;
        [SerializeField] private Transform gameweeksContent;
        [SerializeField] private Transform predictionsContent;

        private List<PredictionsService.Prediction> _existingPredictions = new List<PredictionsService.Prediction>();
        private List<FixturesService.Fixture> _premierLeagueFixtures = new List<FixturesService.Fixture>();
        private List<string> _gameweeks = new List<string>();
        private string _currentGameweek;
        private ObjectPool<PredictionCard> _predictionCardPool;
        private ObjectPool<GameweekButton> _gameweekButtonPool;
        private List<PredictionCard> _predictionCards = new List<PredictionCard>();
        private List<GameweekButton> _gameweekButtons = new List<GameweekButton>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this.gameObject);

            _predictionCardPool = new ObjectPool<PredictionCard>(predictionCard, predictionsContent, 20);
            _gameweekButtonPool = new ObjectPool<GameweekButton>(gameweekButton, gameweeksContent, 38);
            
            Initialize();
        }

        private void Start()
        {
            LoadGameweekButtons();
            TriggerGameweekButton(_currentGameweek);
        }

        private void OnApplicationQuit()
        {
            _predictionCardPool?.ReturnAll();
            _gameweekButtonPool?.ReturnAll();
        }

        private async void Initialize()
        {
            Debug.Log("Initialize predictions screen");
            var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("User ID is null, please login");
                return;
            }
            var response = await PredictionsService.GetPredictionsAsync(userId);
            if (response.success && response.data != null)
                _existingPredictions = response.data;

            var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
            var premierLeagueFixturesResponse = await FixturesService.GetPremierLeagueFixturesAsync();
            if (!premierLeagueFixturesResponse.success)
                return;
            
            _premierLeagueFixtures = premierLeagueFixturesResponse.data!;
            _currentGameweek = GetCurrentGameweek(premierLeagueFixturesResponse.data, dateTimeNowGmt);
            _gameweeks = premierLeagueFixturesResponse.data!.Select(x => x.Matchweek).Distinct().OrderBy(int.Parse).ToList();
        }

        private void TriggerGameweekButton(string gameweek)
        {
            var gameweekButton = _gameweekButtons.FirstOrDefault(x => x.Gameweek == gameweek);
            if (gameweekButton == null)
                return;
            gameweekButton.button.onClick.Invoke();
        }

        public void LoadGameweekMatches(string gameweek)
        {
            foreach (var predictionView in _predictionCards)
                _predictionCardPool.Return(predictionView);

            _predictionCards = new List<PredictionCard>();
            var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
            var fixtures = _premierLeagueFixtures.Where(x => x.Matchweek == gameweek);
            foreach (var fixture in fixtures)
            {
                var prediction = _predictionCardPool.Get();
                prediction.Fixture = fixture;
                prediction.homeTeam.text = fixture.HomeTeam;
                prediction.awayTeam.text = fixture.AwayTeam;
                prediction.dateTime.text = fixture.Kickoff.ToString("HH:mm dd/MM/yyyy");
                
                ImageLoaderService.LoadImageToRawImage(fixture.HomeTeamLogo, prediction.homeTeamLogo);
                ImageLoaderService.LoadImageToRawImage(fixture.AwayTeamLogo, prediction.awayTeamLogo);
                
                // set prediction if already submitted
                var existingPrediction = _existingPredictions.FirstOrDefault(x => x.FixtureId == fixture.Id);
                if (existingPrediction != default)
                {
                    prediction.homeScoreInput.text = existingPrediction.PredictedHomeScore.ToString();
                    prediction.awayScoreInput.text = existingPrediction.PredictedAwayScore.ToString();
                }
                
                // lock prediction if game started
                prediction.Locked = dateTimeNowGmt >= fixture.Kickoff;
                _predictionCards.Add(prediction);
            }
        }

        private void LoadGameweekButtons()
        {
            foreach (var child in _gameweekButtons)
                _gameweekButtonPool.Return(child);

            _gameweekButtons = new List<GameweekButton>();
            foreach (var gameweekNumber in _gameweeks)
            {
                var gameweekBtn = _gameweekButtonPool.Get();
                gameweekBtn.Gameweek = gameweekNumber;
                gameweekBtn.text.text = $"Gameweek {gameweekNumber}";
                _gameweekButtons.Add(gameweekBtn);
            }
        }

        public void EnableGameweekFixtures(string gameweek)
        {
            foreach (var predictionPanel in _predictionCards)
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