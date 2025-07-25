using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class DateButton : MonoBehaviour
    {
        public DateTime Date { get; set; }
        
        public TMP_Text text;
        public Button button;
        public Image underline;
        
        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            // TODO
        }
    }
}