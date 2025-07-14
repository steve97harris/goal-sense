using Framework.Screens;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Screen = Framework.Screens.Screen;
namespace Framework
{
    public enum ScreenName
    {
        HomeScreen,
        PredictionsScreen,
        LoginScreen,
        RegisterScreen,
        ProfileScreen,
        MiniLeaguesScreen
    }
    public class StateMachine : MonoBehaviour
    {
        public static StateMachine Instance;

        public List<Screen> screens;

        private Dictionary<ScreenName, Screen> _screenDict;
        private Screen _currentScreen;
        private Screen _previousScreen;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this.gameObject);
            
            _screenDict = new Dictionary<ScreenName, Screen>();
            foreach (var screen in screens)
                _screenDict[screen.screenName] = screen;
            
            DisableAllScreens();
            // ChangeState(ScreenName.HomeScreen);
        }

        public void ChangeState(ScreenName screenName)
        {
            if (_currentScreen != null)
            {
                _previousScreen = _currentScreen;
                _currentScreen.gameObject.SetActive(false);
            }

            if (_screenDict.TryGetValue(screenName, out var nextScreen))
            {
                _currentScreen = nextScreen;
                _currentScreen.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Screen '{screenName}' not found.");
            }
        }

        public void GoBack()
        {
            ChangeState(_previousScreen.screenName);
        }

        private void DisableAllScreens()
        {
            foreach (var uiScreen in _screenDict)
                uiScreen.Value.gameObject.SetActive(false);
        }
    }
}