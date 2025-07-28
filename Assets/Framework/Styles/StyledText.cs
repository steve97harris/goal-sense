using TMPro;
using UnityEngine;

namespace Framework.UI.StyleSystem
{
    [RequireComponent(typeof(TMP_Text))]
    public class StyledText : MonoBehaviour
    {
        public TextStyle style;
        
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            ApplyStyle();
        }

        public void ApplyStyle()
        {
            if (_text != null)
                UIStyleSystem.Instance.ApplyStyle(_text, style);
        }

        private void OnValidate()
        {
            if (_text == null) 
                _text = GetComponent<TMP_Text>();
            ApplyStyle();
        }
    }

}