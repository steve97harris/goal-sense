using Framework.Services;
using System;
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
        
        private MiniLeague MiniLeague { get; set; }
        
        [SerializeField] private TMP_Text miniLeagueName;
        [SerializeField] private MiniLeagueTableRow miniLeagueTableRow;
        [SerializeField] private Transform miniLeagueTableContent;
        [SerializeField] private TMP_Text inviteCode;
        
        public void SetScreenData(object data)
        {
            if (data is MiniLeague miniLeague)
                MiniLeague = miniLeague;
            
            miniLeagueName.text = MiniLeague.Name;
            inviteCode.text = $"Invite code: {MiniLeague.InviteCode}";
            LoadMiniLeagueTable();
        }
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private async void LoadMiniLeagueTable()
        {
            try
            {
                var response = await MiniLeaguesService.GetMiniLeagueTable(MiniLeague.Id.ToString());
                if (!response.success)
                {
                    Debug.LogError($"Error failed to load mini league table.\n{response.message}");
                    return;
                }
            
                Debug.Log(response.data?.Count + " players in mini league table");
                
                foreach (Transform child in miniLeagueTableContent)
                    Destroy(child.gameObject);

                var miniLeagueTable = response.data!;
                var i = 1;
                foreach (var data in miniLeagueTable)   
                {
                    var row = Instantiate(miniLeagueTableRow, miniLeagueTableContent);
                    row.playerName.text = data.UserName;
                    row.points.text = data.TotalPoints.ToString();
                    row.position.text = i.ToOrdinal();
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