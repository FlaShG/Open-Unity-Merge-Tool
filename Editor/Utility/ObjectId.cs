namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using System;
    using UnityObject = UnityEngine.Object;

    internal readonly struct ObjectId
    {
        public readonly Type type;
        public readonly ulong id;
        public readonly ulong prefabId;

        private ObjectId(Type type, ulong id, ulong prefabId)
        {
            this.type = type;
            this.id = id;
            this.prefabId = prefabId;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectId other)
            {
                return type == other.type &&
                    id == other.id &&
                    prefabId == other.prefabId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, id, prefabId);
        }

        public override string ToString()
        {
            return "[" + id + (prefabId != 0 ? "/" + prefabId : "") + "]";
        }

        public static ObjectId GetFor(UnityObject obj)
        {
            var type = obj.GetType();
            var goid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
            var id = goid.targetObjectId;
            var prefabId = goid.targetPrefabId;

            return new ObjectId(type, id, prefabId);
        }
    }
}
