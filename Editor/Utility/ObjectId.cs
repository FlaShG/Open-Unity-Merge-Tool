namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using UnityObject = UnityEngine.Object;

    internal readonly struct ObjectId
    {
        /// <summary>
        /// Maps GameObjects onto backup objects, if available.
        /// </summary>
        internal static Func<GameObject, GameObject> backupMapping;

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

        public static ObjectId GetFor(GameObject gameObject)
        {
            if (!TryGetIds(gameObject, out var id, out var prefabId) && backupMapping != null)
            {
                var realGameObject = backupMapping(gameObject);
                if (realGameObject != null)
                {
                    TryGetIds(realGameObject, out id, out prefabId);
                }
            }

            return new ObjectId(typeof(GameObject), id, prefabId);
        }

        public static ObjectId GetFor(Component component)
        {
            var type = component.GetType();
            if (!TryGetIds(component, out var id, out var prefabId) && backupMapping != null)
            {
                var index = component.GetComponentIndex();
                var realGameObject = backupMapping(component.gameObject);
                if (realGameObject != null)
                {
                    TryGetIds(realGameObject.GetComponentAtIndex(index), out id, out prefabId);
                }
            }

            return new ObjectId(type, id, prefabId);
        }

        public static ObjectId GetFor(UnityObject obj)
        {
            var type = obj.GetType();
            TryGetIds(obj, out var id, out var prefabId);

            return new ObjectId(type, id, prefabId);
        }

        private static bool TryGetIds(UnityObject obj, out ulong id, out ulong prefabId)
        {
            var goid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
            id = goid.targetObjectId;
            prefabId = goid.targetPrefabId;

            return goid.ToString() != "GlobalObjectId_V1-0-00000000000000000000000000000000-0-0";
        }
    }
}
