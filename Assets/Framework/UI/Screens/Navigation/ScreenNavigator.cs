using Framework.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class ScreenNavigator : MonoBehaviour
    {
        [SerializeField] private ScreenNavigatorButton _homeButton;
        [SerializeField] private ScreenNavigatorButton _predictionsButton;
        [SerializeField] private ScreenNavigatorButton _miniLeaguesButton;
        [SerializeField] private ScreenNavigatorButton _profileButton;
     
        private static StateMachine _stateMachine => StateMachine.Instance;
        private static Color _defaultColour
        {
            get
            {
                var colour = Color.white;
                colour.a = 0.5f;
                return colour;
            }
        }
        private static Color _selectedColour
        {
            get
            {
                var colour = Color.green;
                colour.a = 0.5f;
                return colour;
            }
        }
        
        private ScreenNavigatorButton[] _buttons;
        
        private void Start()
        {
            _buttons = new[] {_homeButton, _predictionsButton, _miniLeaguesButton, _profileButton};
            _homeButton.button.onClick.AddListener(() => NavigateToScreen(_homeButton, ScreenName.HomeScreen));
            _predictionsButton.button.onClick.AddListener(() => NavigateToScreen(_predictionsButton, ScreenName.PredictionsScreen));
            _miniLeaguesButton.button.onClick.AddListener(() => NavigateToScreen(_miniLeaguesButton, ScreenName.MiniLeaguesScreen));
            _profileButton.button.onClick.AddListener(() => NavigateToProfile(_profileButton));
            SetButtonColours(_homeButton);
        }
        
        private void NavigateToScreen(ScreenNavigatorButton buttonClicked, ScreenName screenName)
        {
            SetButtonColours(buttonClicked);
            _stateMachine.ChangeState(screenName);
        }

        private void SetButtonColours(ScreenNavigatorButton buttonClicked)
        {
            foreach (var button in _buttons)
            {
                button.icon.color = _defaultColour;
                button.tmpText.color = _defaultColour;
            }
            buttonClicked.icon.color = _selectedColour;  
            buttonClicked.tmpText.color = _selectedColour;
        }

        private void NavigateToProfile(ScreenNavigatorButton button)
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.JWT_TOKEN))
            {
                NavigateToScreen(button, ScreenName.LoginScreen);
                return;
            }
            NavigateToScreen(button, ScreenName.ProfileScreen);
        }
    }
}