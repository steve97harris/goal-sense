using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class GameweekButton : MonoBehaviour
    {
        public string Gameweek { get; set; }
        
        public TMP_Text text;
        public Button button;
        public Image underline;
        
        private static PredictionsScreen _predictionsScreen => PredictionsScreen.Instance;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _predictionsScreen.EnableGameweekFixtures(Gameweek);
            _predictionsScreen.SetGameweekButtonsView(this);
        }
    }
}