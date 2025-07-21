using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class FirstLoadScreen : Screen
    {
        public override ScreenName screenName => ScreenName.FirstLoadScreen;
        public override ScreenViewport screenViewport => ScreenViewport.ForegroundView;
        
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _registerButton;

        private void Start()
        {
            _registerButton.onClick.AddListener(Register);
            _loginButton.onClick.AddListener(Login);
        }

        private void Login()
        {
            stateMachine.ChangeState(ScreenName.LoginScreen);
        }

        private void Register()
        {
            stateMachine.ChangeState(ScreenName.RegisterScreen);
        }
    }
}