using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class PredictionCard : MonoBehaviour, IPoolable
    {
        public Image lockIcon;
        public TMP_Text dateTime;
        public TMP_Text result;
        public TMP_Text homeTeam;
        public TMP_Text awayTeam;
        public RawImage homeTeamLogo;
        public RawImage awayTeamLogo;
        public TMP_InputField homeScoreInput;
        public TMP_InputField awayScoreInput;
        public Button editButton;
        public Button submitButton;
        
        public Fixture Fixture { get; set; }
        public bool Locked
        {
            get => _isLocked;
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    UpdateLockState();
                }
            }
        }
        private bool _isLocked;
        
        public void OnDespawn()
        {
            homeTeam.text = string.Empty;
            awayTeam.text = string.Empty;
            dateTime.text = string.Empty;
            homeScoreInput.text = string.Empty;
            awayScoreInput.text = string.Empty;
            homeTeamLogo.texture = null;
            awayTeamLogo.texture = null;
            Locked = false;
            homeScoreInput.onValueChanged.RemoveAllListeners();
            awayScoreInput.onValueChanged.RemoveAllListeners();
            Fixture = null;
        }

        private void Start()
        {
            homeScoreInput.onValueChanged.AddListener(OnHomeScoreInputChanged);
            editButton.onClick.AddListener(Edit);
            submitButton.onClick.AddListener(Submit);
        }

        private void Edit()
        {
            submitButton.gameObject.SetActive(true);
            editButton.gameObject.SetActive(false);
            homeScoreInput.interactable = true;
            awayScoreInput.interactable = true;
            homeScoreInput.Select();
        }

        private void Submit()
        {
            submitButton.gameObject.SetActive(false);
            editButton.gameObject.SetActive(true);
            homeScoreInput.interactable = false;
            awayScoreInput.interactable = false;
            SubmitPrediction();
        }

        private async void SubmitPrediction()
        {
            if (string.IsNullOrEmpty(homeScoreInput.text))
                return;
            if (string.IsNullOrEmpty(awayScoreInput.text))
                return;
            var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
            var fixtureId = Fixture.Id.ToString();
            var existingPredictionResponse = await PredictionsService.GetPredictionAsync(userId, fixtureId);
            if (!existingPredictionResponse.success)
            {
                var response = await PredictionsService.SubmitPredictionAsync(userId, 
                    fixtureId, int.Parse(homeScoreInput.text), 
                    int.Parse(awayScoreInput.text));
                if (!response.success)
                    Debug.LogError(response.message);
                else
                    Debug.Log(response.message);
                return;
            }
            var updateResponse = await PredictionsService.UpdatePredictionAsync(userId, fixtureId,
                int.Parse(homeScoreInput.text),
                int.Parse(awayScoreInput.text));
            if (!updateResponse.success)
                Debug.LogError(updateResponse.message);
            else
                Debug.Log(updateResponse.message);
        }

        private void OnHomeScoreInputChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            awayScoreInput.Select();
        }
        
        private void UpdateLockState()
        {
            editButton.gameObject.SetActive(!_isLocked);
            lockIcon.gameObject.SetActive(_isLocked);
        }
    }
}