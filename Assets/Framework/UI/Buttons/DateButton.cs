using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class DateButton : MonoBehaviour
    {
        public DateTime DateTime { get; set; }
        
        public TMP_Text text;
        public Button button;
        public Image underline;
        public CanvasGroup canvasGroup;
        
        private static MatchesScreen MatchesScreen => MatchesScreen.instance;
        
        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            MatchesScreen.LoadMatchesByDate(DateTime);
            MatchesScreen.SetDateButtonsView(this);
            MatchesScreen.RebuildMatchCardLayouts();
        }
    }
}