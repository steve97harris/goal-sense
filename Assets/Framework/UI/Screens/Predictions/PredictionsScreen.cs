using DG.Tweening;
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
        [SerializeField] private ScrollRect gameweeksScrollRect;
        [SerializeField] private Transform gameweeksContent;
        [SerializeField] public ScrollRect predictionsScrollRect;
        [SerializeField] public Transform predictionsContent;
        
        private List<Prediction> _predictions = new List<Prediction>();
        private List<Fixture> _premierLeagueFixtures = new List<Fixture>();
        private List<Fixture> _firstFixturePerGameweeks = new List<Fixture>();
        private string _currentGameweek;
        private ObjectPool<PredictionCard> _predictionCardPool;
        private ObjectPool<GameweekButton> _gameweekButtonPool;
        private List<PredictionCard> _predictionCards = new List<PredictionCard>();
        private List<GameweekButton> _gameweekButtons = new List<GameweekButton>();
        private Tween _scrollTween;

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
                _currentGameweek = FixtureExtensions.GetCurrentGameweek(_premierLeagueFixtures, dateTimeNowGmt);
                _firstFixturePerGameweeks = _premierLeagueFixtures.GetFirstFixturePerGameweek();
                
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
            SnapToButton(gameweekButtonObj);
        }
        
        private void SnapToButton(GameweekButton targetButton)
        {
            // Kill any active scroll tween to prevent conflicts
            _scrollTween?.Kill();

            // Force layout rebuild so positions are accurate
            Canvas.ForceUpdateCanvases();

            var contentRect = gameweeksScrollRect.content;
            var viewportRect = gameweeksScrollRect.viewport != null ? 
                gameweeksScrollRect.viewport : (RectTransform)gameweeksScrollRect.transform;
            var buttonRect = targetButton.GetComponent<RectTransform>();

            // 1. Calculate the button center position relative to content
            var buttonPosInContent = contentRect.InverseTransformPoint(
                buttonRect.TransformPoint(buttonRect.rect.center));
            var targetX = buttonPosInContent.x;

            var contentWidth = contentRect.rect.width;
            var viewportWidth = viewportRect.rect.width;

            if (contentWidth <= viewportWidth) return;

            // 2. Calculate bounds for the center point
            // The normalized position logic in ScrollRect works between 0 and 1.
            // We clamp the target position so that the first/last buttons stay at the edges.
            var minX = viewportWidth / 2f;
            var maxX = contentWidth - viewportWidth / 2f;
            
            // Adjust targetX to account for the content pivot
            var clampedX = Mathf.Clamp(targetX + (contentWidth * contentRect.pivot.x), minX, maxX);
            
            // 3. Convert to 0-1 normalized value
            var targetNormalizedPos = (clampedX - minX) / (maxX - minX);

            // 4. Animate using DOTween
            _scrollTween = gameweeksScrollRect.DOHorizontalNormalizedPos(targetNormalizedPos, 0.5f)
                .SetEase(Ease.OutCubic);
        }

        public void LoadMatchesByGameweek(string gameweek)
        {
            foreach (var predictionView in _predictionCards)
                _predictionCardPool.Return(predictionView);
            foreach (var child in GetComponentsInChildren<PredictionDateText>(predictionsContent))
                Destroy(child.gameObject);
                
            _predictionCards = new List<PredictionCard>();
            var dateTimeNowGmt = DateTime.UtcNow.ConvertUtcTimeToGmt();
            var fixtures = _premierLeagueFixtures.Where(x => x.Matchweek == gameweek).ToList();
            var fixturesByDate = fixtures
                .GroupBy(x => x.Kickoff.Date)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => 
                    x.ToList());

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

            var predictionsByFixture = _predictions.ToDictionary(x => x.FixtureId);
            var fixtures = _premierLeagueFixtures
                .ToDictionary(x => x.Id);

            var predictionPointsPerGw = new Dictionary<string, int>();
            foreach (var pair in predictionsByFixture)
            {
                var fixtureId = pair.Key;
                var fixture = fixtures[fixtureId];
                var prediction = pair.Value;
                var gameweek = fixture.Matchweek;
                if (predictionPointsPerGw.ContainsKey(gameweek))
                    predictionPointsPerGw[gameweek] += prediction.PointsAwarded;
                else 
                    predictionPointsPerGw.Add(gameweek, prediction.PointsAwarded);
            }
            
            _gameweekButtons = new List<GameweekButton>();
            foreach (var fixture in _firstFixturePerGameweeks)
            {
                var gameweekBtn = _gameweekButtonPool.Get();
                gameweekBtn.Gameweek = fixture.Matchweek;
                gameweekBtn.text.text = $"Gameweek {fixture.Matchweek}";
                gameweekBtn.gameweekPoints.text =
                    predictionPointsPerGw.TryGetValue(fixture.Matchweek, out var points) && 
                    points > 0
                        ? $"{points} pts"
                        : "-";
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