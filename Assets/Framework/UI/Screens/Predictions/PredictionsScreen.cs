using Framework.Extensions;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework.UI.Components.PopUps;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Framework.Screens
{
    public class PredictionsScreen : Screen
    {
        public override ScreenName screenName => ScreenName.PredictionsScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static PredictionsScreen instance;
        
        [SerializeField] private GameweekButton gameweekButton;
        [SerializeField] private PredictionDateText predictionDateText;
        [SerializeField] private PredictionCard predictionCard;
        [SerializeField] private Transform gameweeksContent;
        [SerializeField] public Transform predictionsContent;
        [SerializeField] public ScrollRect predictionsScrollRect;
        
        private List<Prediction> _predictions = new List<Prediction>();
        private List<Fixture> _premierLeagueFixtures = new List<Fixture>();
        private List<Fixture> _firstFixturePerGameweeks = new List<Fixture>();
        private string _currentGameweek;
        private ObjectPool<PredictionCard> _predictionCardPool;
        private ObjectPool<GameweekButton> _gameweekButtonPool;
        private List<PredictionCard> _predictionCards = new List<PredictionCard>();
        private List<GameweekButton> _gameweekButtons = new List<GameweekButton>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else 
                Destroy(this.gameObject);

            _predictionCardPool = new ObjectPool<PredictionCard>(predictionCard, predictionsContent, 20);
            _gameweekButtonPool = new ObjectPool<GameweekButton>(gameweekButton, gameweeksContent, 38);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnApplicationQuit()
        {
            _predictionCardPool?.ReturnAll();
            _gameweekButtonPool?.ReturnAll();
        }

        private async void Initialize()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User ID is null, please login");
                    return;
                }
                var response = await PredictionsService.GetPredictionsAsync(userId);
                if (response.success && response.data != null)
                    _predictions = response.data;

                var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
                var premierLeagueFixturesResponse = await FixturesService.GetPremierLeagueFixturesAsync();
                if (!premierLeagueFixturesResponse.success)
                    return;
            
                _premierLeagueFixtures = premierLeagueFixturesResponse.data!;
                _currentGameweek = FixtureExtensions.GetCurrentGameweek(premierLeagueFixturesResponse.data, dateTimeNowGmt);
                _firstFixturePerGameweeks = premierLeagueFixturesResponse.data!.GetFirstFixturePerGameweek();
            
                LoadGameweekButtons();
                TriggerGameweekButton(_currentGameweek);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void TriggerGameweekButton(string gameweek)
        {
            var gameweekButtonObj = _gameweekButtons.FirstOrDefault(x => x.Gameweek == gameweek);
            if (gameweekButtonObj == null)
                return;
            gameweekButtonObj.button.onClick.Invoke();
        }

        public void LoadMatchesByGameweek(string gameweek)
        {
            foreach (var predictionView in _predictionCards)
                _predictionCardPool.Return(predictionView);
            foreach (var child in GetComponentsInChildren<PredictionDateText>(predictionsContent))
                Destroy(child.gameObject);
                
            _predictionCards = new List<PredictionCard>();
            var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
            var fixtures = _premierLeagueFixtures.Where(x => x.Matchweek == gameweek).ToList();
            var fixturesByDate = fixtures
                .GroupBy(x => x.Kickoff.Date)
                .ToDictionary(x => x.Key, x => x.ToList());;

            var siblingIdx = 0;
            foreach (var kvp in fixturesByDate)
            {
                var date = kvp.Key;
                var dateDisplay = Instantiate(predictionDateText, predictionsContent);
                dateDisplay.text.text = date.ToString("ddd d MMM");
                dateDisplay.transform.SetSiblingIndex(siblingIdx);
                
                foreach (var fixture in kvp.Value.OrderBy(x => x.Kickoff))
                {
                    var prediction = _predictionCardPool.Get();
                    var existingPredictionData = _predictions.FirstOrDefault(x => x.FixtureId == fixture.Id);
                    prediction.Initialize(fixture, dateTimeNowGmt, existingPredictionData);
                    siblingIdx = prediction.transform.GetSiblingIndex();
                    _predictionCards.Add(prediction);
                }
                siblingIdx++;
            }
        }

        public void UpdatePredictionListWithNewPrediction(Prediction prediction)
        {
            var existingPrediction = _predictions.FirstOrDefault(x => x.Id == prediction.Id);
            if (existingPrediction != null)
            {
                existingPrediction.PredictedHomeScore = prediction.PredictedHomeScore;
                existingPrediction.PredictedAwayScore = prediction.PredictedAwayScore;
            }
            else 
                _predictions.Add(prediction);
        }

        private void LoadGameweekButtons()
        {
            foreach (var child in _gameweekButtons)
                _gameweekButtonPool.Return(child);

            _gameweekButtons = new List<GameweekButton>();
            foreach (var fixture in _firstFixturePerGameweeks)
            {
                var gameweekBtn = _gameweekButtonPool.Get();
                gameweekBtn.Gameweek = fixture.Matchweek;
                gameweekBtn.text.text = $"Gameweek {fixture.Matchweek}\n{fixture.Kickoff.Date:dd/MM/yy}";
                gameweekBtn.gameObject.SetActive(true);
                _gameweekButtons.Add(gameweekBtn);
            }
        }

        public void EnableGameweekFixtures(string gameweek)
        {
            foreach (var predictionPanel in _predictionCards)
                predictionPanel.gameObject.SetActive(predictionPanel.Fixture.Matchweek == gameweek);
        }

        public void SetGameweekButtonsView(GameweekButton gameweekBtn)
        {
            foreach (var btn in _gameweekButtons)
            {
                btn.underline.gameObject.SetActive(false);
                btn.canvasGroup.alpha = 0.5f;
            }
            gameweekBtn.underline.gameObject.SetActive(true);
            gameweekBtn.canvasGroup.alpha = 1f;
        }
    }
}