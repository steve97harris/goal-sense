using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Components.PopUps
{
    public class SubmissionFailedPopUp : MonoBehaviour
    {
        public TMP_Text message;
        public CanvasGroup canvasGroup;

        [SerializeField] private Button closeButton;

        private void Awake()
        {
            canvasGroup.DOFade(1f, 0.5f);
            closeButton.onClick.AddListener(() => Destroy(this.gameObject));
            StartCoroutine(SelfDestruct());
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(() => Destroy(this.gameObject));       
        }
        
        private IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(3f);
            canvasGroup.DOFade(0, 1f).OnComplete(() =>
            {
                if (this.gameObject != null)
                    Destroy(this.gameObject);
            });
        }
    }
}