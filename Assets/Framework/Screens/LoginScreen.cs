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
                
                if (response.success && response.data != null)
                {
                    Debug.Log($"Login successful! Token: {response.data.token}");
                    PlayerPrefs.SetString(PlayerPrefsKeys.JWT_TOKEN, response.data.token);
                    PlayerPrefs.SetString(PlayerPrefsKeys.USER_ID, response.data.userId);
                }
                else
                {
                    Debug.LogError($"Login failed: {response.message}");
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