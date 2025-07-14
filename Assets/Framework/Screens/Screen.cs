using System;
using UnityEngine;
namespace Framework.Screens
{
    public class Screen : MonoBehaviour
    {
        public ScreenName screenName;
        protected static StateMachine stateMachine => StateMachine.Instance;
    }
}