using Framework.Extensions;
using Framework.Screens;
using System;
using System.Collections.Generic;
using Framework.Services;
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
        MiniLeaguesScreen,
        FirstLoadScreen,
        CreateMiniLeagueScreen,
        JoinMiniLeagueScreen,
        MiniLeagueTableScreen
    }
    public enum ScreenViewport
    {
        MainView,
        ForegroundView
    }
    
    public class StateMachine : MonoBehaviour
    {
        public static StateMachine Instance;

        public List<Screen> screens;

        private Dictionary<ScreenName, Screen> _screenDict;
        private Screen _currentScreen;
        private Screen _previousScreen;

        [SerializeField] private Transform _mainView;
        [SerializeField] private Transform _foregroundView;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(this.gameObject);
            
            _screenDict = new Dictionary<ScreenName, Screen>();
            foreach (var screen in screens)
                _screenDict[screen.screenName] = screen;
            
            var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
            ChangeState(!string.IsNullOrEmpty(userId) ? 
                ScreenName.HomeScreen : ScreenName.FirstLoadScreen);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                ScreenCapture.CaptureScreenshot($"C:\\YR\\Screenshots\\{DateTime.Now.ToString("yyyy-M-d-HH-mm-ss")}.png");
        }

        private void OnDestroy()
        {
            ImageLoaderService.ClearCache();
        }

        public void ChangeState(ScreenName screenName, object screenData = null)
        {
            if (_currentScreen != null && 
                _currentScreen.screenName == screenName)
                return;
            
            if (_currentScreen != null)
            {
                _previousScreen = _currentScreen;
                Destroy(_currentScreen.gameObject);
            }

            if (_screenDict.TryGetValue(screenName, out var nextScreen))
            {
                _currentScreen = Instantiate(nextScreen, 
                    GetViewport(nextScreen.screenViewport));
                
                if (screenData != null && _currentScreen is IScreenData screenWithData)
                    screenWithData.SetScreenData(screenData);
            }
            else
                Debug.LogWarning($"Screen '{screenName}' not found.");
        }

        public void GoBack()
        {
            ChangeState(_previousScreen.screenName);
        }

        private Transform GetViewport(ScreenViewport viewport)
        {
            switch (viewport)
            {
                case ScreenViewport.ForegroundView:
                    return _foregroundView;
                case ScreenViewport.MainView:
                default:
                    return _mainView;
            }
        }
    }
}