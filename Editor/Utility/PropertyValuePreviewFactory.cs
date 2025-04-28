namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;

    internal static class PropertyValuePreviewFactory
    {
        public static VisualElement GetPreview(object obj)
        {
            if (obj is string s)
            {
                const int limit = 12;
                if (s.Length > limit)
                {
                    var sShort = s.Substring(0, limit) + "...";
                    var label = new Label(sShort);
                    label.tooltip = s;
                    return label;
                }
                return new Label(s);
            }

            if (obj is Quaternion quaternion)
            {
                return new Label(quaternion.eulerAngles.ToString());
            }

            if (obj is Color color)
            {
                var box = new Box();
                box.style.backgroundColor = color;
                box.style.SetMargin(2, 2, 0, 0);

                var label = new Label(ColorToHex(color));
                label.style.color = IsBright(color) ? Color.black : Color.white;
                box.Add(label);

                return box;
            }

            return new Label(obj.ToString());
        }

        private static string ColorToHex(Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255).ToString("X2");
            var g = Mathf.RoundToInt(color.g * 255).ToString("X2");
            var b = Mathf.RoundToInt(color.b * 255).ToString("X2");
            return $"#{r}{g}{b}";
        }

        private static bool IsBright(Color color)
        {
            Color.RGBToHSV(color, out _, out _, out var value);
            return value > 0.65f;
        }
    }
}
