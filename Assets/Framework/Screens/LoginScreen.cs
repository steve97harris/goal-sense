using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class LoginScreen : Screen
    {
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _registerButton;

        private void Start()
        {
            _registerButton.onClick.AddListener(HandleRegister);
            _loginButton.onClick.AddListener(HandleLogin);
        }

        private async void HandleLogin()
        {
            try
            {
                var response = await LoginService.LoginAsync(_emailInput.text, _passwordInput.text);
                
                if (response.Success && response.Data != null)
                {
                    Debug.Log($"Login successful! Token: {response.Data.Token}");
                    PlayerPrefs.SetString(PlayerPrefsKeys.JWT_TOKEN, response.Data.Token);
                    PlayerPrefs.SetString(PlayerPrefsKeys.USER_ID, response.Data.UserId);
                }
                else
                {
                    Debug.LogError($"Login failed: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Login process failed: {ex.Message}");
            }
        }

        private void HandleRegister()
        {
            stateMachine.ChangeState(ScreenName.RegisterScreen);
        }
    }
}