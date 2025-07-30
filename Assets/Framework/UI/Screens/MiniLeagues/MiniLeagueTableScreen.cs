using Framework.Services;
using System;
using UnityEngine;
namespace Framework.Screens.MiniLeagues
{
    public class MiniLeagueTableScreen : Screen, IScreenData
    {
        public override ScreenName screenName => ScreenName.MiniLeagueTableScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
           
        public static MiniLeagueTableScreen instance;

        private MiniLeague MiniLeague { get; set; }

        public void SetScreenData(object data)
        {
            if (data is MiniLeague miniLeague)
                MiniLeague = miniLeague;
            LoadMiniLeague();
        }
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private async void LoadMiniLeague()
        {
            var response = await MiniLeaguesService.GetMiniLeagueTable(MiniLeague.Id.ToString());
            if (!response.success)
            {
                Debug.LogError($"Error failed to load mini league table.\n{response.message}");
                return;
            }
            // TODO load mini league table
        }
    }
}