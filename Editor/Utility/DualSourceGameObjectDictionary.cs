namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    internal class DualSourceGameObjectDictionary : IDisposable
    {
        private readonly Dictionary<ObjectId, GameObject> ourObjects = new();
        /// <summary>
        /// Maps our "real" objects (aka the objects in the prefab)
        /// onto the corresponding object in our prefab stage.
        /// </summary>
        private readonly Dictionary<GameObject, GameObject> ourRealObjects = new();
        private readonly Dictionary<ObjectId, GameObject> theirObjects = new();

        public void AddOurObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                ourObjects.Add(ObjectId.GetFor(gameObject), gameObject);
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
                ourObjects.Add(id, gameObjectInstance);
                ourRealObjects.Add(gameObjectInstance, realObject);
            }
            ObjectId.backupMapping = ourRealObjects.GetOptional;
        }

        public void AddTheirObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                theirObjects.Add(ObjectId.GetFor(gameObject), gameObject);
            }
        }

        public GameObject GetOurEquivalentToTheir(GameObject gameObject)
        {
            if (gameObject == null) return null;

            var id = ObjectId.GetFor(gameObject);
            return ourObjects.GetOptional(id);
        }

        public List<GameObjectMergeActionContainer> GenerateMergeActions()
        {
            var result = new List<GameObjectMergeActionContainer>();

            GameObject deletedPotentialParent = null;

            foreach (var ourObject in ourObjects)
            {
                if (deletedPotentialParent && ourObject.Value.HasParent(deletedPotentialParent))
                {
                    continue;
                }

                deletedPotentialParent = null;

                var container = new GameObjectMergeActionContainer(this, ourObject.Value, theirObjects.GetOptional(ourObject.Key));
                if (container.HasActions)
                {
                    result.Add(container);
                    if (container.IsRelatedToGameObjectExistence)
                    {
                        deletedPotentialParent = ourObject.Value;
                    }
                }
            }

            deletedPotentialParent = null;

            foreach (var theirObject in theirObjects)
            {
                if (deletedPotentialParent && theirObject.Value.HasParent(deletedPotentialParent))
                {
                    continue;
                }

                deletedPotentialParent = null;

                if (!ourObjects.ContainsKey(theirObject.Key))
                {
                    var container = new GameObjectMergeActionContainer(this, null, theirObject.Value);
                    if (container.HasActions)
                    {
                        result.Add(container);
                        if (container.IsRelatedToGameObjectExistence)
                        {
                            deletedPotentialParent = theirObject.Value;
                        }
                    }
                }
                theirObject.Value.SetActiveForMerging(false);
            }

            return result;
        }

        public void Dispose()
        {
            ObjectId.backupMapping = null;
        }
    }
}
