namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using System.Collections.Generic;

    internal static class ButtonColorExtensions
    {
        private static readonly Dictionary<IEventHandler, (Button button, Color color)> entries = new();

        public static void SetButtonColor(this Button button, Color color)
        {
            entries[button] = (button, color);

            button.style.backgroundColor = color;
            button.RegisterCallbackOnce<MouseOverEvent>(MouseOver);
            button.RegisterCallbackOnce<MouseOutEvent>(MouseOut);
        }

        public static void ResetButtonColor(this Button button)
        {
            button.UnregisterCallback<MouseOverEvent>(MouseOver);
            button.UnregisterCallback<MouseOutEvent>(MouseOut);
            button.style.backgroundColor = StyleKeyword.Null;
            entries.Remove(button);
        }

        private static void MouseOver(MouseOverEvent evt)
        {
            if (entries.TryGetValue(evt.currentTarget, out var entry))
            {
                Color.RGBToHSV(entry.color, out var h, out var s, out var v);
                entry.button.style.backgroundColor = Color.HSVToRGB(h, s, Mathf.Lerp(v, 1f, 0.1f));
            }
        }

        private static void MouseOut(MouseOutEvent evt)
        {
            if (entries.TryGetValue(evt.currentTarget, out var entry))
            {
                entry.button.style.backgroundColor = entry.color;
            }
        }
    }
}
