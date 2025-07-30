using System;
using Framework.Screens.MiniLeagues;
using Framework.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class MiniLeagueButton : MonoBehaviour
    {
        public MiniLeague MiniLeague { get; set; }
        
        public TMP_Text text;
        public Button button;
        
        private static MiniLeaguesScreen MiniLeaguesScreen => MiniLeaguesScreen.instance;
        private static StateMachine StateMachine => StateMachine.Instance;
        
        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            StateMachine.ChangeState(ScreenName.MiniLeagueTableScreen, MiniLeague);
        }
    }
}