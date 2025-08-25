using System;
using Framework.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Screens
{
    public class MatchCardHolder : MonoBehaviour, IPoolable
    {
        public Transform matchesContent;
        public Button button;
        public TMP_Text headerText;
        public Image downArrow;
        public Image upArrow;
        public RawImage flagIcon;

        private bool _matchesEnabled = true;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent as RectTransform);
        }

        private void OnEnable()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent as RectTransform);
        }

        public void OnDespawn()
        {
            headerText.text = string.Empty;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            matchesContent.gameObject.SetActive(!_matchesEnabled);
            downArrow.gameObject.SetActive(!_matchesEnabled);
            upArrow.gameObject.SetActive(_matchesEnabled);
            _matchesEnabled = !_matchesEnabled;
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent as RectTransform);
        }
    }
}