namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityObject = UnityEngine.Object;
    using System;

    internal readonly struct ObjectId
    {
        public readonly ulong id;
        public readonly ulong prefabId;

        private ObjectId(ulong id, ulong prefabId)
        {
            this.id = id;
            this.prefabId = prefabId;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectId other)
            {
                return id == other.id && prefabId == other.prefabId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, prefabId);
        }

        public override string ToString()
        {
            return "[" + id + (prefabId != 0 ? "/" + prefabId : "") + "]";
        }

        public static ObjectId GetFor(UnityObject obj)
        {
            var goid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
            var id = goid.targetObjectId;
            var prefabId = goid.targetPrefabId;

            return new ObjectId(id, prefabId);
        }
    }
}
