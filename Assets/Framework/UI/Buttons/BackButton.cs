using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Screens
{
    public class BackButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        
        [SerializeField] private Screen[] returnScreen;
        
        private static StateMachine StateMachine => StateMachine.Instance;
        
        private void Awake()
        {
            if (returnScreen.Length == 0)
                button.onClick.AddListener(() => 
                    StateMachine.GoBack());
            else 
                button.onClick.AddListener(() => 
                    StateMachine.ChangeState(returnScreen[0].screenName));
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}