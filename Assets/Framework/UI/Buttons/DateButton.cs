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
        
        private static HomeScreen HomeScreen => HomeScreen.instance;
        
        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            HomeScreen.EnableMatchesByDate(DateTime);
            HomeScreen.SetDateButtonsView(this);
        }
    }
}