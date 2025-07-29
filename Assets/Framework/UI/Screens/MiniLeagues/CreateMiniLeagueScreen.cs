using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Screens.MiniLeagues
{
    public class CreateMiniLeagueScreen : Screen
    {
        public override ScreenName screenName => ScreenName.CreateMiniLeagueScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        public static CreateMiniLeagueScreen instance;

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button createLeagueButton;
        [SerializeField] private TMP_Text errorText;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            
            createLeagueButton.onClick.AddListener(CreateLeague);   
        }

        private void OnDestroy()
        {
            createLeagueButton.onClick.RemoveListener(CreateLeague); 
        }

        private async void CreateLeague()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User ID is null, please login");
                    errorText.text = "*Error creating mini league, please login and try again";
                    errorText.gameObject.SetActive(true);
                    return;
                }

                if (string.IsNullOrEmpty(inputField.text))
                {
                    Debug.LogError("League name cannot be empty");
                    errorText.text = "*Error creating mini league, league name cannot be empty";
                    errorText.gameObject.SetActive(true);
                    return;
                }
                
                var response = await MiniLeaguesService.CreateMiniLeague(Guid.Parse(userId), inputField.text);
                if (!response.success)
                {
                    Debug.LogError(response.message);
                    errorText.text = $"*Error creating mini league.\n{response.message}";
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