using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class RegisterScreen : Screen
    {
        public override ScreenName screenName => ScreenName.RegisterScreen;
        public override ScreenViewport screenViewport => ScreenViewport.ForegroundView;
        
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TMP_InputField _confirmPasswordInput;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _registerButton;
        [SerializeField] private TMP_Text _errorText;

        private void Start()
        {
            _backButton.onClick.AddListener(stateMachine.GoBack);
            _registerButton.onClick.AddListener(HandleRegister);
        }

        private void Update()
        {
            if (_passwordInput.text.Equals(_confirmPasswordInput.text))
            {
                if (_errorText.gameObject.activeSelf)
                    _errorText.gameObject.SetActive(false);
                if (_registerButton.GetComponent<CanvasGroup>().interactable)
                    return;
                _registerButton.GetComponent<CanvasGroup>().interactable = true;
                _registerButton.GetComponent<CanvasGroup>().alpha = 1f;
                return;
            }
            _errorText.text = "Passwords do not match";
            _errorText.gameObject.SetActive(true);
            _registerButton.GetComponent<CanvasGroup>().interactable = false;
            _registerButton.GetComponent<CanvasGroup>().alpha = 0.35f;
        }

        private async void HandleRegister()
        {
            try
            {
                if (string.IsNullOrEmpty(_nameInput.text))
                {
                    Debug.LogError("Name is empty");
                    return;
                }
                if (!_passwordInput.text.Equals(_confirmPasswordInput.text))
                {
                    Debug.LogError("Passwords do not match");
                    return;
                }

                var response = await RegisterService.RegisterAsync(_emailInput.text, 
                    _passwordInput.text, _confirmPasswordInput.text, _nameInput.text);
                
                if (response.success && response.data != null)
                {
                    Debug.Log($"Registration successful! Token: {response.data.token}");
                    PlayerPrefs.SetString(PlayerPrefsKeys.JWT_TOKEN, response.data.token);
                    PlayerPrefs.SetString(PlayerPrefsKeys.USER_ID, response.data.userId);
                    PlayerPrefs.SetString(PlayerPrefsKeys.USER_FULL_NAME, response.data.userFullName);
                }
                else
                {
                    Debug.LogError($"Registration failed: {response.message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Registration process failed: {ex.Message}");
            }
        }
    }
}