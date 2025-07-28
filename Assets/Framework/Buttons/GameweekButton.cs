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
        
        private static PredictionsScreen PredictionsScreen => PredictionsScreen.Instance;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            PredictionsScreen.EnableGameweekFixtures(Gameweek);
            PredictionsScreen.SetGameweekButtonsView(this);
        }
    }
}