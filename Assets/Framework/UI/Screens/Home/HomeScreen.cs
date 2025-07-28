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
        [SerializeField] private Transform datesContent;
        [SerializeField] private Transform matchesContent;

        private List<MatchCard> _matches = new List<MatchCard>();
        private List<DateButton> _matchDayButtons = new List<DateButton>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else 
                Destroy(this.gameObject);
            Initialize();
        }

        private void OnApplicationQuit()
        {
            foreach (Transform child in matchesContent)
                Destroy(child.gameObject);
            foreach (Transform child in datesContent)
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
            foreach (Transform child in matchesContent)
                Destroy(child.gameObject);

            _matches = new List<MatchCard>();
            foreach (var fixture in fixtures)
            {
                var matchCardObj = Instantiate(this.matchCard, matchesContent);
                
                matchCardObj.Fixture = fixture;
                
                matchCardObj.homeTeam.text = fixture.HomeTeam;
                matchCardObj.awayTeam.text = fixture.AwayTeam;
                matchCardObj.dateTime.text = fixture.Kickoff.ToString("dd/MM/yyyy");
                
                if (fixture.Status is "TIMED" or "SCHEDULED")
                    matchCardObj.result.text = fixture.Kickoff.ToString("t", new CultureInfo("en-US")).ToLower();
                else
                    matchCardObj.result.text = $"{fixture.HomeScore} : {fixture.AwayScore}";
                
                ImageLoaderService.LoadImageToRawImage(fixture.HomeTeamLogo, matchCardObj.homeTeamLogo);
                ImageLoaderService.LoadImageToRawImage(fixture.AwayTeamLogo, matchCardObj.awayTeamLogo);
                
                _matches.Add(matchCardObj);
            }
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