using Framework.UI.StyleSystem;
using TMPro;

namespace Framework.Extensions
{
    public static class UIStyleExtensions
    {
        public static void ApplyStyle(this TMP_Text text, TextStyle style)
        {
            UIStyleSystem.Instance.ApplyStyle(text, style);
        }
    }
}