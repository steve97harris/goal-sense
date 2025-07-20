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
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _logoutButton;

        private void Start()
        {
            _logoutButton.onClick.AddListener(HandleLogout);
            var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
            var jwtToken = PlayerPrefs.GetString(PlayerPrefsKeys.JWT_TOKEN);
            var email = GetEmailFromJwtToken(jwtToken);
            _text.text = $"User ID: {userId}\nEmail: {email}";
        }

        private void HandleLogout()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.JWT_TOKEN);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.USER_ID);
            stateMachine.ChangeState(ScreenName.LoginScreen);
        }
        
        public static Claim GetEmailFromJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            return jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        }

    }
}