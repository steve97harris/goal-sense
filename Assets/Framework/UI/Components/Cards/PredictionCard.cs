using Framework.Extensions;
using Framework.Services;
using System;
using DG.Tweening;
using Framework.UI.Components.PopUps;
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
        public TMP_Text pointsAwarded;
        public TMP_Text homeTeam;
        public TMP_Text awayTeam;
        public RawImage homeTeamLogo;
        public RawImage awayTeamLogo;
        public TMP_InputField homeScoreInput;
        public TMP_InputField awayScoreInput;
        public Button editButton;
        public Button submitButton;
        
        [SerializeField] private SubmittedPopUp submittedPopUp;
        [SerializeField] private SubmissionFailedPopUp submissionFailedPopUp;
        
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
        
        private static PredictionsScreen PredictionsScreen => PredictionsScreen.instance;
        
        public void OnDespawn()
        {
            homeTeam.text = string.Empty;
            awayTeam.text = string.Empty;
            dateTime.text = string.Empty;
            homeScoreInput.text = string.Empty;
            awayScoreInput.text = string.Empty;
            homeTeamLogo.texture = null;
            awayTeamLogo.texture = null;
            result.text = "Result: -";
            pointsAwarded.text = "Points awarded: -";
            Locked = false;
            homeScoreInput.onValueChanged.RemoveAllListeners();
            awayScoreInput.onValueChanged.RemoveAllListeners();
            Fixture = null;
            var submittedPopUp = GetComponentInChildren<SubmittedPopUp>();
            if (submittedPopUp != null)
                Destroy(submittedPopUp.gameObject);
            var submissionFailedPopUp = GetComponentInChildren<SubmissionFailedPopUp>();
            if (submissionFailedPopUp != null)
                Destroy(submissionFailedPopUp.gameObject);
        }

        private void Start()
        {
            homeScoreInput.onValueChanged.AddListener(OnHomeScoreInputChanged);
            editButton.onClick.AddListener(Edit);
            submitButton.onClick.AddListener(Submit);
        }

        public void Initialize(Fixture fixture, DateTime dateTimeNowGmt, Prediction existingPrediction)
        {
            Fixture = fixture;
            homeTeam.text = fixture.HomeTeam;
            awayTeam.text = fixture.AwayTeam;
            dateTime.text = fixture.Kickoff.ToString("h:mm tt").ToLower();
                
            ImageLoaderService.LoadImageToRawImage(fixture.HomeTeamLogo, homeTeamLogo);
            ImageLoaderService.LoadImageToRawImage(fixture.AwayTeamLogo, awayTeamLogo);
                
            // set prediction if already submitted
            homeScoreInput.text = existingPrediction != default ? 
                existingPrediction.PredictedHomeScore.ToString() : "";
            awayScoreInput.text = existingPrediction != default ? 
                existingPrediction.PredictedAwayScore.ToString() : "";
                
            // lock prediction if game started
            Locked = dateTimeNowGmt >= fixture.Kickoff;
            if (Locked && existingPrediction is { IsProcessed: true })
            {
                result.text = $"Result: {fixture.HomeScore} - {fixture.AwayScore}";
                pointsAwarded.text = $"Points awarded: {existingPrediction.PointsAwarded}";
            }
            
            gameObject.SetActive(true);
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
            try
            {
                if (string.IsNullOrEmpty(homeScoreInput.text))
                    return;
                if (string.IsNullOrEmpty(awayScoreInput.text))
                    return;
                var userId = PlayerPrefs.GetString(PlayerPrefsKeys.USER_ID);
                var fixtureId = Fixture.Id.ToString();
                var predictionResponse = await PredictionsService.GetPredictionAsync(userId, fixtureId);
                if (!predictionResponse.success)
                {
                    SubmitNewPrediction(userId, fixtureId);
                    return;
                }
                UpdatePrediction(userId, fixtureId);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async void SubmitNewPrediction(string userId, string fixtureId)
        {
            try
            {
                var response = await PredictionsService.SubmitPredictionAsync(userId, 
                    fixtureId, 
                    int.Parse(homeScoreInput.text), 
                    int.Parse(awayScoreInput.text));
                if (!response.success)
                {
                    Debug.LogError(response.message);
                    LoadSubmissionFailedPopUp(response.message);
                }
                else
                {
                    Debug.Log(response.message);
                    PredictionsScreen.UpdatePredictionListWithNewPrediction(response.data);
                    LoadSubmittedPopUp();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async void UpdatePrediction(string userId, string fixtureId)
        {
            try
            {
                var updateResponse = await PredictionsService.UpdatePredictionAsync(userId, 
                    fixtureId,
                    int.Parse(homeScoreInput.text),
                    int.Parse(awayScoreInput.text));
                if (!updateResponse.success)
                {
                    Debug.LogError(updateResponse.message);
                    LoadSubmissionFailedPopUp(updateResponse.message);
                }
                else
                {
                    Debug.Log(updateResponse.message);
                    PredictionsScreen.UpdatePredictionListWithNewPrediction(updateResponse.data);
                    LoadSubmittedPopUp();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
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

        private void LoadSubmittedPopUp()
        {
            var popUp = Instantiate(submittedPopUp, this.transform);
            popUp.canvasGroup.alpha = 0f;
        }

        private void LoadSubmissionFailedPopUp(string message)
        {
            var popUp = Instantiate(submissionFailedPopUp, this.transform);
            popUp.canvasGroup.alpha = 0f;
            popUp.message.text = $"Error, submission failed - {message}";
        }
    }
}