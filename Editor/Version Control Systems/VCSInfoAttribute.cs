namespace ThirteenPixels.OpenUnityMergeTool
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class VCSInfoAttribute : Attribute
    {
        internal string title { get; }

        public VCSInfoAttribute(string title)
        {
            this.title = title;
        }
    }
}
