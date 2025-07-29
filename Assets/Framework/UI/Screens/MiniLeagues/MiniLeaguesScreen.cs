using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Services;
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
            
            Initialize();
        }

        private void Initialize()
        {
            LoadUsersMiniLeagues();
        }

        private async void LoadUsersMiniLeagues()
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
                var miniLeagueButton = Instantiate(this.miniLeagueButton, miniLeaguesContent);
                miniLeagueButton.MiniLeague = miniLeague;
                miniLeagueButton.text.text = miniLeague.Name;
                miniLeagueButton.gameObject.SetActive(true);
            }
        }
    }
}