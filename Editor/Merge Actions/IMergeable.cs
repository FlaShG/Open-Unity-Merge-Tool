namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;

    internal interface IMergeable
    {
        string Title { get; }

        string ApplyOursButtonLabel { get; }
        string ApplyTheirsButtonLabel { get; }

        DecisionState DecisionState { get; }

        SerializedProperty SerializedProperty { get; }
        object OurValue { get; }
        object TheirValue { get; }

        bool IsUsingOurs { get; }
        bool IsUsingTheirs { get; }

        bool OurValueIsPrefabDefault { get; }
        bool TheirValueIsPrefabDefault { get; }

        void UseOurs();
        void UseTheirs();
        void AcceptNewValue();
    }
}
