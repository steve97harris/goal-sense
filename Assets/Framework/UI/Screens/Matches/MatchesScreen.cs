using Framework.Extensions;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class MatchesScreen : Screen
    {
        public override ScreenName screenName => ScreenName.HomeScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static MatchesScreen instance;
        
        [SerializeField] private DateButton dateButton;
        [SerializeField] private MatchCardHolder matchCardHolder;
        [SerializeField] private MatchCard matchCard;
        [SerializeField] private GameObject noMatchesCard;
        [SerializeField] private Transform datesContent;
        [SerializeField] private Transform matchesContent;

        private ObjectPool<MatchCardHolder> _matchCardHolderPool;
        private ObjectPool<MatchCard> _matchCardPool;
        private List<MatchCardHolder> _matchCardHolders = new List<MatchCardHolder>();
        private List<MatchCard> _matches = new List<MatchCard>();
        private List<DateButton> _matchDayButtons = new List<DateButton>();
        private List<Fixture> _fixtures = new List<Fixture>();
        private List<Competition> _competitions = new List<Competition>();
        private GameObject _noMatchesCard;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else 
                Destroy(this.gameObject);
            
            _matchCardHolderPool = new ObjectPool<MatchCardHolder>(matchCardHolder, matchesContent, 20);
            _matchCardPool = new ObjectPool<MatchCard>(matchCard, matchesContent, 50);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnApplicationQuit()
        {
            foreach (var card in _matches)
                _matchCardPool.Return(card);
            foreach (var cardHolder in _matchCardHolders)
                _matchCardHolderPool.Return(cardHolder);
            foreach (Transform child in datesContent)
                Destroy(child.gameObject);
        }

        private async void Initialize()
        {
            try
            {
                var competitionsResponse = await CompetitionsService.GetCompetitions();
                if (!competitionsResponse.success)
                {
                    Debug.LogError($"Competitions response failed\n{competitionsResponse.message}");
                    return;
                }
            
                _competitions = competitionsResponse.data!;
            
                var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
                var yesterdayGmt = dateTimeNowGmt.AddDays(-1);
                var dateTimeNowPlus2Days = dateTimeNowGmt.AddDays(2);
                var fixturesResponse = await FixturesService.GetFixturesAsync(yesterdayGmt.ToString("yyyy-MM-dd"), dateTimeNowPlus2Days.ToString("yyyy-MM-dd"));
                if (!fixturesResponse.success)
                {
                    Debug.LogError($"Fixtures response failed\n{fixturesResponse.message}");
                    return;
                }

                _fixtures = fixturesResponse.data!;
                _noMatchesCard = Instantiate(noMatchesCard, matchesContent);
                _noMatchesCard.SetActive(false);
            
                var dates = Enumerable.Range(-1, 4)
                    .Select(offset => dateTimeNowGmt.AddDays(offset))
                    .ToList();
                LoadDateButtons(dates);
                TriggerDateButton(dateTimeNowGmt);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void TriggerDateButton(DateTime date)
        {
            var gameweekButton = _matchDayButtons.FirstOrDefault(x => x.DateTime.Date == date.Date);
            if (gameweekButton == null)
                return;
            gameweekButton.button.onClick.Invoke();
        }

        public void LoadMatchesByDate(DateTime dateTime)
        {
            foreach (var card in _matches)
                _matchCardPool.Return(card);
            foreach (var cardHolder in _matchCardHolders)
                _matchCardHolderPool.Return(cardHolder);

            _matches = new List<MatchCard>();
            _matchCardHolders = new List<MatchCardHolder>();
            
            var fixturesByLeagueId = _fixtures
                .Where(x => x.Kickoff.Date == dateTime.Date)
                .GroupBy(x => x.LeagueId)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            var competitionsById = _competitions.ToDictionary(x => x.Id);
            
            foreach (var kvp in fixturesByLeagueId)
            {
                var matchHolder = _matchCardHolderPool.Get();
                matchHolder.headerText.text = competitionsById[kvp.Key].Name;;
                
                foreach (var fixture in kvp.Value.OrderBy(x => x.Kickoff))
                {
                    var matchCardObj = _matchCardPool.Get(matchHolder.matchesContent);
                    matchCardObj.Initialize(fixture);
                    
                    _matches.Add(matchCardObj);
                }
                _matchCardHolders.Add(matchHolder);
            }

            _noMatchesCard.SetActive(fixturesByLeagueId.Count == 0);
        }

        private void LoadDateButtons(List<DateTime> dates)
        {
            foreach (Transform child in datesContent)
                Destroy(child.gameObject);

            _matchDayButtons = new List<DateButton>();
            var todaysDate = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow).Date;
            foreach (var date in dates)
            {
                var btn = Instantiate(dateButton, datesContent);
                btn.DateTime = date;
                var dateDisplay = date.Date.ToString("d MMM");
                if (date.Date == todaysDate)
                    dateDisplay = "Today";
                btn.text.text = dateDisplay;
                _matchDayButtons.Add(btn);
            }
        }

        public void SetDateButtonsView(DateButton dateButton)
        {
            foreach (var btn in _matchDayButtons)
            {
                btn.underline.gameObject.SetActive(false);
                btn.canvasGroup.alpha = 0.5f;
            }
            dateButton.underline.gameObject.SetActive(true);
            dateButton.canvasGroup.alpha = 1f;
        }

        public void RebuildMatchCardLayouts()
        {
            foreach (var cardHolder in _matchCardHolders)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(cardHolder.matchesContent as RectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(cardHolder.matchesContent.parent as RectTransform);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(matchesContent as RectTransform);
        }
    }
}