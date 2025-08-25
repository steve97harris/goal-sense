using Framework.Services;
using System;
using System.Linq;
using Framework.Extensions;
using Framework.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Screens.MiniLeagues
{
    public class MiniLeagueTableScreen : Screen, IScreenData
    {
        public override ScreenName screenName => ScreenName.MiniLeagueTableScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
           
        public static MiniLeagueTableScreen instance;
        
        private static MiniLeague MiniLeague { get; set; }
        
        [SerializeField] private TMP_Text miniLeagueName;
        [SerializeField] private MiniLeagueTableRow miniLeagueTableRow;
        [SerializeField] private Transform miniLeagueTableContent;
        [SerializeField] private TMP_Text inviteCode;
        [SerializeField] private TMP_Text tableGameweekHeader;
        
        public void SetScreenData(object data)
        {
            if (data is MiniLeague miniLeague)
                MiniLeague = miniLeague;
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
            miniLeagueName.text = MiniLeague.Name;
            inviteCode.text = $"Invite code: {MiniLeague.InviteCode}";
            LoadTable();
        }

        private async void LoadTable()
        {
            try
            {
                var fixturesResponse = await FixturesService.GetPremierLeagueFixturesAsync();
                if (!fixturesResponse.success)
                {
                    Debug.LogError($"Error, failed to load premier league fixtures\n{fixturesResponse.message}");
                    return;
                }
                
                var currentGameweek = FixtureExtensions.GetCurrentGameweek(fixturesResponse.data, 
                    DateTime.UtcNow.ConvertUtcTimeToGmt());
                tableGameweekHeader.text = $"Gw {currentGameweek}";
                
                var tableResponse = await MiniLeaguesService.GetMiniLeagueTable(MiniLeague.Id.ToString());
                if (!tableResponse.success)
                {
                    Debug.LogError($"Error, failed to load mini league table\n{tableResponse.message}");
                    return;
                }
                
                Debug.Log(tableResponse.data?.Count + " players in mini league table");
                
                foreach (Transform child in miniLeagueTableContent)
                    Destroy(child.gameObject);

                var miniLeagueTable = tableResponse.data!;
                var gwFixtures = fixturesResponse.data!
                    .Where(x => x.Matchweek == currentGameweek)
                    .Select(x => x.Id)
                    .ToArray();
                
                var i = 1;
                foreach (var data in miniLeagueTable)   
                {
                    var row = Instantiate(miniLeagueTableRow, miniLeagueTableContent);
                    row.Initialize(data, i.ToOrdinal(), gwFixtures);
                    i++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}