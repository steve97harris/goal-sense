using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class GameweekButton : MonoBehaviour, IPoolable
    {
        public string Gameweek { get; set; }
        
        public TMP_Text text;
        public TMP_Text gameweekPoints;
        public Button button;
        public Image underline;
        public CanvasGroup canvasGroup;
        
        private static PredictionsScreen PredictionsScreen => PredictionsScreen.instance;
        
        public void OnDespawn()
        {
            text.text = string.Empty;
            underline.gameObject.SetActive(false);
            gameweekPoints.text = "-";
            Gameweek = string.Empty;
            button.onClick.RemoveAllListeners();
        }

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            PredictionsScreen.LoadMatchesByGameweek(Gameweek);
            PredictionsScreen.EnableGameweekFixtures(Gameweek);
            PredictionsScreen.SetGameweekButtonsView(this);
        }
    }
}