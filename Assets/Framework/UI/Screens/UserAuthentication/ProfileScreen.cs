using Framework.Extensions;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Framework.UI.Components.PopUps;
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

        public DeleteAccountPopUp deleteAccountPopUp;
        
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField playerNameInputField;
        [SerializeField] private Button savePlayerNameButton;
        [SerializeField] private Button saveEmailButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button deleteAccountButton;

        private string _userFullName;
        private string _email;

        private void Awake()
        {
            LoadUserInfo();
            logoutButton.onClick.AddListener(Logout);
            deleteAccountButton.onClick.AddListener(DeleteAccount);
            savePlayerNameButton.onClick.AddListener(SavePlayerName);
            saveEmailButton.onClick.AddListener(SaveEmail);
            playerNameInputField.onValueChanged.AddListener(PlayerNameChanged);
            emailInputField.onValueChanged.AddListener(EmailChanged);
        }

        private void OnDestroy()
        {
            logoutButton.onClick.RemoveListener(Logout);
            deleteAccountButton.onClick.RemoveListener(DeleteAccount);
            savePlayerNameButton.onClick.RemoveListener(SavePlayerName);
            saveEmailButton.onClick.RemoveListener(SaveEmail);
            playerNameInputField.onValueChanged.RemoveListener(PlayerNameChanged);
            emailInputField.onValueChanged.RemoveListener(EmailChanged);
        }

        private async void SavePlayerName()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User id is null");
                    return;
                }
                var updatedPlayerName = playerNameInputField.text;
                await UserAuthService.UpdateProfileAsync(userId, "", updatedPlayerName);
                EnableSavePlayerNameButton(
                    savePlayerNameButton.GetComponent<CanvasGroup>(), 
                    false);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private async void SaveEmail()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User id is null");
                    return;
                }
                var updatedEmail = emailInputField.text;
                await UserAuthService.UpdateProfileAsync(userId, updatedEmail, "");
                EnableSavePlayerNameButton(
                    saveEmailButton.GetComponent<CanvasGroup>(), 
                    false);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void PlayerNameChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || 
                string.IsNullOrEmpty(_userFullName) ||
                value == _userFullName)
                return;
            _userFullName = value;
            EnableSavePlayerNameButton(
                savePlayerNameButton.GetComponent<CanvasGroup>(), 
                true);
        }
        
        private void EmailChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || 
                string.IsNullOrEmpty(_email) ||
                value == _email)
                return;
            _email = value;
            EnableSavePlayerNameButton(
                saveEmailButton.GetComponent<CanvasGroup>(),
                true);
        }

        private void EnableSavePlayerNameButton(CanvasGroup cg, bool enable)
        {
            cg.interactable = enable;
            cg.blocksRaycasts = enable;
            cg.alpha = enable ? 1 : 0.3f;
        }

        private static void Logout()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.JWT_TOKEN);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.USER_ID);
            stateMachine.ChangeState(ScreenName.LoginScreen);
        }

        private void DeleteAccount()
        {
            deleteAccountPopUp.gameObject.SetActive(true);
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
                _userFullName = user.userFullName;
                _email = user.email;
            
                Debug.Log($"user profile:\n{user.userId}\n{user.email}\n{user.userFullName}");

                emailInputField.text = user.email;
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