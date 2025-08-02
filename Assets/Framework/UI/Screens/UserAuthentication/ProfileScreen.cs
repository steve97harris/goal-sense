using Framework.Extensions;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class ProfileScreen : Screen
    {
        public override ScreenName screenName => ScreenName.ProfileScreen;
        public override ScreenViewport screenViewport => ScreenViewport.MainView;
        
        [SerializeField] private TMP_Text emailText;
        [SerializeField] private TMP_InputField playerNameInputField;
        [SerializeField] private Button savePlayerNameButton;
        [SerializeField] private Button logoutButton;

        private void Awake()
        {
            logoutButton.onClick.AddListener(Logout);
            savePlayerNameButton.onClick.AddListener(SavePlayerName);
            LoadUserInfo();
            playerNameInputField.onValueChanged.AddListener((string value) => EnableSavePlayerNameButton(true));
        }

        private void OnDestroy()
        {
            logoutButton.onClick.RemoveListener(Logout);
            savePlayerNameButton.onClick.RemoveListener(SavePlayerName);
            playerNameInputField.onValueChanged.RemoveListener((string value) => EnableSavePlayerNameButton(true));
        }

        private void SavePlayerName()
        {
            var updatedPlayerName = playerNameInputField.text;
            // TODO send updated player name to api
            Debug.Log("TODO send updated player name to api");
        }

        private void EnableSavePlayerNameButton(bool enable)
        {
            savePlayerNameButton.GetComponent<CanvasGroup>().interactable = enable;
            savePlayerNameButton.GetComponent<CanvasGroup>().blocksRaycasts = enable;
            savePlayerNameButton.GetComponent<CanvasGroup>().alpha = enable ? 1 : 0.3f;
        }

        private static void Logout()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.JWT_TOKEN);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.USER_ID);
            stateMachine.ChangeState(ScreenName.LoginScreen);
        }

        private async void LoadUserInfo()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                var response = await UserAuthService.GetUserProfileAsync(userId);
                if (!response.success)
                {
                    Debug.LogError($"Failed to get user profile {userId}");
                    return;
                }
                var user = response.data!;
            
                Debug.Log($"user profile:\n{user.userId}\n{user.email}\n{user.userFullName}");
                
                emailText.text = $"Email: {user.email}";
                playerNameInputField.text = user.userFullName;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static Claim GetClaimFromJwtToken(string token, string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            return jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == claimType);
        }

    }
}