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
    public class HomeScreen : Screen
    {
        public override ScreenName screenName => ScreenName.HomeScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static HomeScreen instance;
        
        [SerializeField] private DateButton dateButton;
        [SerializeField] private MatchCard matchCard;
        [SerializeField] private GameObject noMatchesCard;
        [SerializeField] private Transform datesContent;
        [SerializeField] private Transform matchesContent;

        private ObjectPool<MatchCard> _matchCardPool;
        private List<MatchCard> _matches = new List<MatchCard>();
        private List<DateButton> _matchDayButtons = new List<DateButton>();
        private List<Fixture> _fixtures = new List<Fixture>();
        private GameObject _noMatchesCard;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else 
                Destroy(this.gameObject);
            
            _matchCardPool = new ObjectPool<MatchCard>(matchCard, matchesContent, 50);
            
            Initialize();
        }

        private void OnApplicationQuit()
        {
            foreach (var card in _matches)
                _matchCardPool.Return(card);
            foreach (Transform child in datesContent)
                Destroy(child.gameObject);
        }

        private async void Initialize()
        {
            var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
            var yesterdayGmt = dateTimeNowGmt.AddDays(-1);
            var dateTimeNowPlus2Days = dateTimeNowGmt.AddDays(2);
            var fixturesResponse = await FixturesService.GetFixturesAsync(yesterdayGmt.ToString("yyyy-MM-dd"), dateTimeNowPlus2Days.ToString("yyyy-MM-dd"));
            if (!fixturesResponse.success)
                return;

            _fixtures = fixturesResponse.data!;
            _noMatchesCard = Instantiate(noMatchesCard, matchesContent);
            _noMatchesCard.SetActive(false);
            
            var dates = Enumerable.Range(-1, 4)
                .Select(offset => dateTimeNowGmt.AddDays(offset))
                .ToList();
            LoadDateButtons(dates);
            TriggerDateButton(dateTimeNowGmt);
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

            _matches = new List<MatchCard>();
            var fixturesByDate = _fixtures.Where(x => x.Kickoff.Date == dateTime.Date).ToList();
            
            foreach (var fixture in fixturesByDate)
            {
                var matchCardObj = _matchCardPool.Get();
                
                matchCardObj.Fixture = fixture;
                
                matchCardObj.homeTeam.text = fixture.HomeTeam;
                matchCardObj.awayTeam.text = fixture.AwayTeam;
                matchCardObj.dateTime.text = fixture.Kickoff.ToString("HH:mm dd/MM/yyyy");
                matchCardObj.result.text = $"{fixture.HomeScore} : {fixture.AwayScore}";
                matchCardObj.status.text = $"Status: {fixture.Status}";
                
                ImageLoaderService.LoadImageToRawImage(fixture.HomeTeamLogo, matchCardObj.homeTeamLogo);
                ImageLoaderService.LoadImageToRawImage(fixture.AwayTeamLogo, matchCardObj.awayTeamLogo);
                
                _matches.Add(matchCardObj);
            }

            _noMatchesCard.SetActive(fixturesByDate.Count == 0);
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

        public void EnableMatchesByDate(DateTime dateTime)
        {
            foreach (var matchView in _matches)
                matchView.gameObject.SetActive(matchView.Fixture.Kickoff.Date == dateTime.Date);
        }

        public void SetDateButtonsView(DateButton dateButton)
        {
            foreach (var btn in _matchDayButtons)
                btn.underline.gameObject.SetActive(false);
            dateButton.underline.gameObject.SetActive(true);
        }
    }
}