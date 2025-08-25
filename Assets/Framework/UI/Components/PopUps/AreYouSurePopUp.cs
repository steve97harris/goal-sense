using System;
using Framework.Extensions;
using Framework.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Components.PopUps
{
    public class AreYouSurePopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        private void Awake()
        {
            cancelButton.onClick.AddListener(() => Destroy(this.gameObject));  
        }

        public void Initialize(string areYouSureText, Action callback)
        {
            text.text = areYouSureText;
            confirmButton.onClick.AddListener(() => callback?.Invoke());   
        }

        private void OnDestroy()
        {
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}