namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        public static TValue GetOptional<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            return default;
        }
    }
}
