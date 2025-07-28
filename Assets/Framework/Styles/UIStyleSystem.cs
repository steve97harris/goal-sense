using TMPro;
using UnityEngine;

namespace Framework.UI.StyleSystem
{
    public enum TextStyle
    {
        Header,
        Subheader,
        Body,
        Small,
        FontOnly
    }
    
    [CreateAssetMenu(fileName = "UIStyle", menuName = "Styles/UI Style")]
    public class UIStyleSystem : ScriptableObject
    {
        [Header("Fonts")]
        public TMP_FontAsset PrimaryFont;
        public TMP_FontAsset SecondaryFont;
        
        [Header("Font Sizes")]
        public float HeaderSize = 54f;
        public float SubheaderSize = 42f;
        public float BodySize = 36f;
        public float SmallSize = 24f;
        
        [Header("Colors")]
        public Color PrimaryTextColor = Color.white;
        public Color SecondaryTextColor = Color.white;

        // Singleton pattern for easy access
        private static UIStyleSystem instance;
        public static UIStyleSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<UIStyleSystem>("Style/UIStyle");
                    if (instance == null)
                    {
                        Debug.LogError("UIStyle not found in Resources folder!");
                    }
                }
                return instance;
            }
        }

        public void ApplyStyle(TMP_Text text, TextStyle style)
        {
            switch (style)
            {
                case TextStyle.Header:
                    text.font = PrimaryFont;
                    text.fontSize = HeaderSize;
                    text.color = PrimaryTextColor;
                    break;
                case TextStyle.Subheader:
                    text.font = PrimaryFont;
                    text.fontSize = SubheaderSize;
                    text.color = PrimaryTextColor;
                    break;
                case TextStyle.Body:
                    text.font = SecondaryFont;
                    text.fontSize = BodySize;
                    text.color = SecondaryTextColor;
                    break;
                case TextStyle.Small:
                    text.font = SecondaryFont;
                    text.fontSize = SmallSize;   
                    text.color = SecondaryTextColor;
                    break;
                case TextStyle.FontOnly:
                    text.font = PrimaryFont;
                    break;
            }
        }
    }
}