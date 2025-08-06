using System;
using Framework.Extensions;
using Framework.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Components.PopUps
{
    public class DeleteAccountPopUp : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        private static StateMachine StateMachine => StateMachine.Instance;
        
        private void Awake()
        {
            confirmButton.onClick.AddListener(ConfirmDeleteAccount);   
            cancelButton.onClick.AddListener(() => this.gameObject.SetActive(false));  
        }

        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(ConfirmDeleteAccount);   
            cancelButton.onClick.RemoveListener(() => this.gameObject.SetActive(false));  
        }

        private async void ConfirmDeleteAccount()
        {
            try
            {
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("User id is null");
                    return;
                }

                await UserAuthService.DeleteUserAccountAsync(userId);
                StateMachine.ChangeState(ScreenName.LoginScreen);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}