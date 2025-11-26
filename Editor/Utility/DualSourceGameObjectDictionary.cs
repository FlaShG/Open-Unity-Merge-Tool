namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityObject = UnityEngine.Object;

    internal class DualSourceGameObjectDictionary : IDisposable
    {
        private readonly Dictionary<ObjectId, GameObject> ourGameObjects = new();
        /// <summary>
        /// Maps our "real" objects (aka the objects in the prefab)
        /// onto the corresponding object in our prefab stage.
        /// </summary>
        private readonly Dictionary<GameObject, GameObject> ourRealGameObjects = new();
        /// <summary>
        /// All UnityEngine.Objects in "our" scene - GameObjects and their components.
        /// </summary>
        private readonly Dictionary<ObjectId, UnityObject> allOurObjects = new();
        private readonly Dictionary<ObjectId, GameObject> theirGameObjects = new();

        public void AddOurObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var gameObjectId = ObjectId.GetFor(gameObject);
                ourGameObjects.Add(gameObjectId, gameObject);

                allOurObjects.Add(gameObjectId, gameObject);
                foreach (var component in gameObject.GetComponents<Component>())
                {
                    allOurObjects.Add(ObjectId.GetFor(component), component);
                }
            }
        }

        /// <summary>
        /// Adds "our" objects, but we have "real" and "fake" versions of them.
        /// For example, the "fake" versions would be the instances that were loaded into a PrefabStage - those don't have an ObjectId.
        /// </summary>
        public void AddOurObjects(IEnumerable<GameObject> gameObjectInstances, IEnumerable<GameObject> realGameObjects)
        {
            var realGameObjectsIterator = realGameObjects.GetEnumerator();
            foreach (var gameObjectInstance in gameObjectInstances)
            {
                realGameObjectsIterator.MoveNext();

                var realObject = realGameObjectsIterator.Current;
                var id = ObjectId.GetFor(realObject);
                ourGameObjects.Add(id, gameObjectInstance);
                ourRealGameObjects.Add(gameObjectInstance, realObject);
            }
            ObjectId.backupMapping = ourRealGameObjects.GetValueOrDefault;
        }

        public void AddTheirObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                theirGameObjects.Add(ObjectId.GetFor(gameObject), gameObject);
            }
        }

        public GameObject GetOurEquivalentToTheir(GameObject gameObject)
        {
            if (gameObject == null) return null;

            var id = ObjectId.GetFor(gameObject);
            return ourGameObjects.GetValueOrDefault(id, gameObject);
        }

        public UnityObject GetOurEquivalentToTheir(UnityObject obj)
        {
            if (obj == null) return null;

            var id = ObjectId.GetFor(obj);
            return allOurObjects.GetValueOrDefault(id, obj);
        }

        public List<GameObjectMergeActionContainer> GenerateMergeActions()
        {
            var result = new List<GameObjectMergeActionContainer>();

            GameObject deletedPotentialParent = null;

            foreach (var (ourObjectId, ourObject) in ourGameObjects)
            {
                if (deletedPotentialParent && ourObject.HasParent(deletedPotentialParent))
                {
                    continue;
                }

                deletedPotentialParent = null;

                var container = new GameObjectMergeActionContainer(this, ourObject, theirGameObjects.GetValueOrDefault(ourObjectId));
                if (container.HasActions)
                {
                    result.Add(container);
                    if (container.IsRelatedToGameObjectExistence)
                    {
                        deletedPotentialParent = ourObject;
                    }
                }
            }

            deletedPotentialParent = null;

            foreach (var (theirObjectId, theirObject) in theirGameObjects)
            {
                if (deletedPotentialParent && theirObject.HasParent(deletedPotentialParent))
                {
                    continue;
                }

                deletedPotentialParent = null;

                if (!ourGameObjects.ContainsKey(theirObjectId))
                {
                    var container = new GameObjectMergeActionContainer(this, null, theirObject);
                    if (container.HasActions)
                    {
                        result.Add(container);
                        if (container.IsRelatedToGameObjectExistence)
                        {
                            deletedPotentialParent = theirObject;
                        }
                    }
                }
                theirObject.SetActiveForMerging(false);
            }

            return result;
        }

        public void Dispose()
        {
            ObjectId.backupMapping = null;
        }
    }
}
