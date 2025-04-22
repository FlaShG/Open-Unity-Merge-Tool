namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;

    internal interface IMergeable
    {
        string Title { get; }

        string ApplyOursButtonLabel { get; }
        string ApplyTheirsButtonLabel { get; }

        MergeAction.Resolution State { get; }

        SerializedProperty SerializedProperty { get; }
        object OurValue { get; }
        object TheirValue { get; }

        bool OurValueIsPrefabDefault { get; }
        bool TheirValueIsPrefabDefault { get; }

        void UseOurs();
        void UseTheirs();
        void AcceptNewValue();
    }
}
