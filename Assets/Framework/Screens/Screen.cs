using System;
using UnityEngine;
namespace Framework.Screens
{
    public abstract class Screen : MonoBehaviour
    {
        public abstract ScreenName screenName { get; }
        public abstract ScreenViewport screenViewport { get; }
        protected static StateMachine stateMachine => StateMachine.Instance;
    }
}