namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using System.Collections.Generic;

    internal class DualSourceGameObjectDictionary
    {
        private readonly Dictionary<ObjectId, GameObject> ourObjects = new();
        private readonly Dictionary<ObjectId, GameObject> theirObjects = new();

        public void AddOurObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                ourObjects.Add(ObjectId.GetFor(gameObject), gameObject);
            }
        }

        public void AddTheirObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                theirObjects.Add(ObjectId.GetFor(gameObject), gameObject);
            }
        }

        public GameObject GetTheirEquivalent(GameObject ourGameObject)
        {
            return theirObjects.GetOptional(ObjectId.GetFor(ourGameObject));
        }

        public List<GameObjectMergeActionContainer> GenerateMergeActions()
        {
            var result = new List<GameObjectMergeActionContainer>();

            foreach (var ourObject in ourObjects)
            {
                var container = new GameObjectMergeActionContainer(ourObject.Value, theirObjects.GetOptional(ourObject.Key));
                if (container.HasActions)
                {
                    result.Add(container);
                }
            }

            foreach (var theirObject in theirObjects)
            {
                if (!ourObjects.ContainsKey(theirObject.Key))
                {
                    var container = new GameObjectMergeActionContainer(null, theirObject.Value);
                    if (container.HasActions)
                    {
                        result.Add(container);
                    }
                }
                theirObject.Value.SetActiveForMerging(false);
            }

            return result;
        }
    }
}
