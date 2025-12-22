using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Services;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Screens.MiniLeagues
{
    public class MiniLeaguesScreen : Screen
    {
        public override ScreenName screenName => ScreenName.MiniLeaguesScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static MiniLeaguesScreen instance;

        [SerializeField] private Button createLeagueButton;
        [SerializeField] private Button joinLeagueButton;
        [SerializeField] private MiniLeagueButton miniLeagueButton;
        [SerializeField] private Transform miniLeaguesContent;
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            
            createLeagueButton.onClick.AddListener(() => 
                stateMachine.ChangeState(ScreenName.CreateMiniLeagueScreen));
            joinLeagueButton.onClick.AddListener(() => 
                stateMachine.ChangeState(ScreenName.JoinMiniLeagueScreen));
            
            Initialize();
        }

        private void OnDestroy()
        {
            createLeagueButton.onClick.RemoveListener(() => 
                stateMachine.ChangeState(ScreenName.CreateMiniLeagueScreen));
            joinLeagueButton.onClick.RemoveListener(() => 
                stateMachine.ChangeState(ScreenName.JoinMiniLeagueScreen));
        }

        private void Initialize()
        {
            LoadUsersMiniLeagues();
        }

        private async void LoadUsersMiniLeagues()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User ID is null, please login");
                    return;
                }

                var response = await MiniLeaguesService.GetUsersMiniLeagues(userId);
                if (!response.success)
                {
                    Debug.LogError(response.message);
                    return;
                }
            
                var leagues = response.data!;

                foreach (Transform child in miniLeaguesContent)
                    Destroy(child.gameObject);
            
                foreach (var miniLeague in leagues)
                {
                    var btn = Instantiate(this.miniLeagueButton, miniLeaguesContent);
                    btn.MiniLeague = miniLeague;
                    btn.text.text = miniLeague.Name;
                
                    var position = await GetUsersMiniLeagueTablePosition(
                        miniLeague.Id.ToString(), userId);
                    btn.leaguePosition.text = position;
                
                    btn.gameObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private async Task<string> GetUsersMiniLeagueTablePosition(
            string miniLeagueId, string userId)
        {
            var tableResponse = await MiniLeaguesService.GetMiniLeagueTable(miniLeagueId);
            if (!tableResponse.success)
            {
                Debug.LogError($"Error, failed to load mini league table\n{tableResponse.message}");
                return null;
            }
            
            var miniLeagueTable = tableResponse.data!;   
            var userLeagueTableData = miniLeagueTable
                .FirstOrDefault(x =>
                    x.UserId.ToString() == userId);
            return userLeagueTableData == null ? "-" : 
                (miniLeagueTable.IndexOf(userLeagueTableData) + 1).ToString();
        }
    }
}