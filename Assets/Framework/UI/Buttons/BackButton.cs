using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Screens
{
    public class BackButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        
        private static StateMachine StateMachine => StateMachine.Instance;
        
        private void Awake()
        {
            button.onClick.AddListener(() => StateMachine.GoBack());
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(() => StateMachine.GoBack());
        }
    }
}