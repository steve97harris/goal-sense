using Framework.Extensions;
using Framework.Services;
using System;
using DG.Tweening;
using Framework.UI.Components.PopUps;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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
        public Button homeScoreEditButton;
        public Button awayScoreEditButton;
        
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
        private bool _isEditingMode;
        private string _homePrediction = null;
        private string _awayPrediction = null;
        
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
            var popUp = GetComponentInChildren<SubmittedPopUp>();
            if (popUp != null)
                Destroy(popUp.gameObject);
            var failedPopUp = GetComponentInChildren<SubmissionFailedPopUp>();
            if (failedPopUp != null)
                Destroy(failedPopUp.gameObject);
        }

        private void Start()
        {
            homeScoreInput.onValueChanged.AddListener(OnHomeScoreInputChanged);
            homeScoreEditButton.onClick.AddListener(EditHomeMode);
            awayScoreEditButton.onClick.AddListener(EditAwayMode);
        }

        private void OnDestroy()
        {
            homeScoreInput.onValueChanged.RemoveListener(OnHomeScoreInputChanged);
            homeScoreEditButton.onClick.RemoveListener(EditHomeMode);
            awayScoreEditButton.onClick.RemoveListener(EditAwayMode);
        }

        private void Update()
        {
            if (!_isEditingMode) 
                return;

            var currentSelected = EventSystem.current.currentSelectedGameObject;
        
            if (currentSelected == null || 
                (currentSelected != homeScoreInput.gameObject && 
                 currentSelected != awayScoreInput.gameObject))
                ExitEditMode();
        }

        public void Initialize(Fixture fixture, DateTime dateTimeNowGmt, Prediction existingPrediction)
        {
            Fixture = fixture;
            homeTeam.text = fixture.HomeTeam;
            awayTeam.text = fixture.AwayTeam;
            dateTime.text = fixture.Kickoff.ToString("h:mm tt").ToLower();
                
            // set prediction if already submitted
            homeScoreInput.text = existingPrediction != default ? 
                existingPrediction.PredictedHomeScore.ToString() : "";
            awayScoreInput.text = existingPrediction != default ? 
                existingPrediction.PredictedAwayScore.ToString() : "";
            _homePrediction = homeScoreInput.text;
            _awayPrediction = awayScoreInput.text;
            
            // lock prediction if game started
            Locked = dateTimeNowGmt >= fixture.Kickoff;
            if (Locked && existingPrediction is { IsProcessed: true })
            {
                result.text = $"Result: {fixture.HomeScore} - {fixture.AwayScore}";
                pointsAwarded.text = $"Points awarded: {existingPrediction.PointsAwarded}";
            }
            
            gameObject.SetActive(true);
        }
        
        private void OnHomeScoreInputChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            awayScoreInput.Select();
        }
        
        private void EditHomeMode()
        {
            SetBothScoreInputsInteractable(true);
            homeScoreInput.Select();
            SnapTo(this.GetComponent<RectTransform>());
        }

        private void EditAwayMode()
        {
            SetBothScoreInputsInteractable(true);
            awayScoreInput.Select();
            SnapTo(this.GetComponent<RectTransform>());
        }

        private void SetBothScoreInputsInteractable(bool interactable)
        {
            _isEditingMode = interactable;
            homeScoreEditButton.gameObject.SetActive(!interactable);
            awayScoreEditButton.gameObject.SetActive(!interactable);
            homeScoreInput.interactable = interactable;
            awayScoreInput.interactable = interactable;
        }
        
        private void ExitEditMode()
        {
            if (!string.IsNullOrEmpty(homeScoreInput.text) && 
                !string.IsNullOrEmpty(awayScoreInput.text) &&
                (_homePrediction != homeScoreInput.text || 
                 _awayPrediction != awayScoreInput.text))
            {
                _homePrediction = homeScoreInput.text;
                _awayPrediction = awayScoreInput.text;
                SubmitPrediction();    
            }
            
            SetBothScoreInputsInteractable(false);
            var vlg = PredictionsScreen.predictionsContent
                    .GetComponent<VerticalLayoutGroup>();
            vlg.padding.bottom = 60;
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                vlg.GetComponent<RectTransform>());
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
        
        private void UpdateLockState()
        {
            homeScoreEditButton.gameObject.SetActive(!_isLocked);
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

        private static void SnapTo(RectTransform target)
        {
            PredictionsScreen.predictionsContent
                .GetComponent<VerticalLayoutGroup>().padding.bottom = 1000;
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                PredictionsScreen.predictionsContent.GetComponent<RectTransform>());
            
            Canvas.ForceUpdateCanvases();

            var contentPanel = PredictionsScreen.predictionsContent.GetComponent<RectTransform>();
            var scrollRect = PredictionsScreen.predictionsScrollRect;
            var destination =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
            var halfScrollHeight = (scrollRect.GetComponent<RectTransform>().rect.size.y / 2);
            destination = new Vector2(destination.x, 
                destination.y - halfScrollHeight);
            
            contentPanel.DOAnchorPos(destination, 1f);
        }
    }
}