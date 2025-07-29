using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Services;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Framework.Screens.MiniLeagues
{
    public class JoinMiniLeagueScreen : Screen
    {
        public override ScreenName screenName => ScreenName.JoinMiniLeagueScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static JoinMiniLeagueScreen instance;

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button joinLeagueButton;
        [SerializeField] private TMP_Text errorText;
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            
            joinLeagueButton.onClick.AddListener(JoinLeague);   
        }

        private void OnDestroy()
        {
            joinLeagueButton.onClick.RemoveListener(JoinLeague); 
        }

        private async void JoinLeague()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User ID is null, please login");
                    errorText.text = "*Error joining mini league, please login and try again";
                    errorText.gameObject.SetActive(true);
                    return;
                }

                if (string.IsNullOrEmpty(inputField.text))
                {
                    Debug.LogError("League name cannot be empty");
                    errorText.text = "*Error joining mini league, invite code is empty";
                    errorText.gameObject.SetActive(true);
                    return;
                }
                
                var response = await MiniLeaguesService.JoinMiniLeague(Guid.Parse(userId), inputField.text);
                if (!response.success)
                {
                    Debug.LogError(response.message);
                    errorText.text = $"*Error joining mini league.\n{response.message}";
                    errorText.gameObject.SetActive(true);
                    return;
                }
                
                stateMachine.ChangeState(ScreenName.MiniLeaguesScreen);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}