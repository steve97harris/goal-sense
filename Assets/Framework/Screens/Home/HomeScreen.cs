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
        
        public static HomeScreen Instance;
        
        [SerializeField] private DateButton _dateButton;
        [SerializeField] private MatchView _matchView;
        [SerializeField] private Transform _datesContent;
        [SerializeField] private Transform _matchesContent;

        private List<MatchView> _matches = new List<MatchView>();
        private List<DateButton> _matchDayButtons = new List<DateButton>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this.gameObject);
            Initialize();
        }

        private void OnApplicationQuit()
        {
            foreach (Transform child in _matchesContent)
                Destroy(child.gameObject);
            foreach (Transform child in _datesContent)
                Destroy(child.gameObject);
        }

        private async void Initialize()
        {
            Debug.Log("Initialize home screen");
            
            var dateTimeNowGmt = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow);
            var yesterdayGmt = dateTimeNowGmt.AddDays(-1);
            var dateTimeNowPlus2Days = dateTimeNowGmt.AddDays(2);
            var fixturesResponse = await FixturesService.GetFixturesAsync(yesterdayGmt.ToString("yyyy-MM-dd"), dateTimeNowPlus2Days.ToString("yyyy-MM-dd"));
            if (!fixturesResponse.success)
                return;
            var dates = Enumerable.Range(-1, 4)
                .Select(offset => dateTimeNowGmt.AddDays(offset))
                .ToList();
            LoadDateButtons(dates);
            LoadMatches(fixturesResponse.data);
            TriggerDateButton(dateTimeNowGmt);
        }

        private void TriggerDateButton(DateTime date)
        {
            var gameweekButton = _matchDayButtons.FirstOrDefault(x => x.DateTime.Date == date.Date);
            if (gameweekButton == null)
                return;
            gameweekButton.button.onClick.Invoke();
        }

        private void LoadMatches(List<FixturesService.Fixture> fixtures)
        {
            foreach (Transform child in _matchesContent)
                Destroy(child.gameObject);

            _matches = new List<MatchView>();
            foreach (var fixture in fixtures)
            {
                var matchView = Instantiate(this._matchView, _matchesContent);
                
                matchView.Fixture = fixture;
                
                matchView.homeTeam.text = fixture.HomeTeam;
                matchView.awayTeam.text = fixture.AwayTeam;
                matchView.dateTime.text = fixture.Kickoff.ToString("dd/MM/yyyy");
                
                if (fixture.Status is "TIMED" or "SCHEDULED")
                    matchView.result.text = fixture.Kickoff.ToString("t", new CultureInfo("en-US"));
                else
                    matchView.result.text = $"{fixture.HomeScore} : {fixture.AwayScore}";
                
                ImageLoaderService.LoadImageToRawImage(fixture.HomeTeamLogo, matchView.homeTeamLogo);
                ImageLoaderService.LoadImageToRawImage(fixture.AwayTeamLogo, matchView.awayTeamLogo);
                
                _matches.Add(matchView);
            }
        }

        private void LoadDateButtons(List<DateTime> dates)
        {
            foreach (Transform child in _datesContent)
                Destroy(child.gameObject);

            _matchDayButtons = new List<DateButton>();
            var todaysDate = DateTimeExtensions.ConvertUtcTimeToGmt(DateTime.UtcNow).Date;
            foreach (var date in dates)
            {
                var btn = Instantiate(_dateButton, _datesContent);
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