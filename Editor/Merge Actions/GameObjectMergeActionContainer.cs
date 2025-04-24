namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// A container that contains all available <see cref="MergeAction"/>s for one specific GameObject.
    /// </summary>
    internal class GameObjectMergeActionContainer
    {
        public readonly string Name;
        private readonly GameObject ourGameObject;
        private readonly GameObject theirGameObject;
        private readonly List<MergeAction> mergeActions;

        public readonly ReadOnlyCollection<MergeAction> MergeActions;
        public bool HasActions => mergeActions.Count > 0;

        public GameObjectMergeActionContainer(GameObject ours, GameObject theirs)
        {
            ourGameObject = ours;
            theirGameObject = theirs;

            mergeActions = new();
            MergeActions = mergeActions.AsReadOnly();

            if (ours && !theirs)
            {
                Name = ours.GetPath();
                mergeActions.Add(new MergeActionOurGameObject(ours));
            }
            else if (theirs && !ours)
            {
                Name = theirs.GetPath();
                mergeActions.Add(new MergeActionTheirGameObject(theirs));
            }
            else
            {
                Name = ours.GetPath();
                FindComponentDifferences();
                FindGameObjectPropertyDifferences();
            }

            // TODO CheckIfMerged
        }

        private void FindComponentDifferences()
        {
            var ourComponents = ourGameObject.GetComponents<Component>();

            theirGameObject.SetActiveForMerging(true);
            var theirComponents = new Dictionary<ObjectId, Component>();
            foreach (var component in theirGameObject.GetComponents<Component>())
            {
                if (component == null) continue;

                theirComponents.Add(ObjectId.GetFor(component), component);
            }
            theirGameObject.SetActiveForMerging(false);

            foreach (var ourComponent in ourComponents)
            {
                if (ourComponent == null) continue;

                var id = ObjectId.GetFor(ourComponent);
                if (theirComponents.TryGetValue(id, out var theirComponent))
                {
                    // Component exists in both versions.
                    FindPropertyDifferences(ourComponent, theirComponent);
                    theirComponents.Remove(id);
                }
                else
                {
                    // Component only exists in our version.
                    mergeActions.Add(new MergeActionOurComponent(ourGameObject, ourComponent));
                }
            }

            foreach (var theirExclusiveComponent in theirComponents.Values)
            {
                // Component only exists in their version.
                mergeActions.Add(new MergeActionTheirComponent(ourGameObject, theirExclusiveComponent));
            }
        }

        private void FindGameObjectPropertyDifferences()
        {
            // TODO Check for parent?
            FindPropertyDifferences(ourGameObject, theirGameObject);
        }

        private void FindPropertyDifferences(UnityObject ours, UnityObject theirs)
        {
            if (ours.GetType() != theirs.GetType())
            {
                throw new System.InvalidOperationException($"The two objects must be of the same type, but are {ours.GetType().Name} and {theirs.GetType().Name}.");
            }

            MergeActionPropertyValues mergeAction = null;

            var ourSerializedObject = new SerializedObject(ours);
            var theirSerializedObject = new SerializedObject(theirs);

            using var ourProperty = ourSerializedObject.GetIterator();
            if (ourProperty.Next(true))
            {
                using var theirProperty = theirSerializedObject.GetIterator();
                theirProperty.Next(true);

                var shouldEnterChildren = ourProperty.hasVisibleChildren;

                while (ourProperty.NextVisible(shouldEnterChildren))
                {
                    theirProperty.NextVisible(shouldEnterChildren);

                    // TODO Ignore prefab name when merging a prefab

                    if (DifferentValues(ourProperty, theirProperty))
                    {
                        if (mergeAction == null)
                        {
                            mergeAction = new MergeActionPropertyValues(ours);
                        }

                        mergeAction.AddProperty(ourProperty.Copy(), theirProperty.Copy());
                    }
                }
            }
            
            if (mergeAction != null)
            {
                mergeActions.Add(mergeAction);
            }
        }

        /// <summary>
        /// Returns true when the two properties have different values, false otherwise.
        /// </summary>
        private static bool DifferentValues(SerializedProperty ourProperty, SerializedProperty theirProperty)
        {
            if (!ourProperty.IsRealArray())
            {
                return DifferentValuesFlat(ourProperty, theirProperty);
            }
            else
            {
                if (ourProperty.arraySize != theirProperty.arraySize)
                {
                    return true;
                }

                using var op = ourProperty.Copy();
                using var tp = theirProperty.Copy();

                // Enter, then skip past the arraySize property, onto the [0] element.
                op.Next(true);
                op.Next(true);
                tp.Next(true);
                tp.Next(true);

                for (int i = 0; i < ourProperty.arraySize; ++i)
                {
                    op.Next(false);
                    tp.Next(false);

                    if (DifferentValuesFlat(op, tp))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool DifferentValuesFlat(SerializedProperty ourProperty, SerializedProperty theirProperty)
        {
            if (ourProperty.IsPrefabDefault() != theirProperty.IsPrefabDefault())
            {
                return true;
            }

            var our = ourProperty.GetValue();
            var their = theirProperty.GetValue();

            if (ourProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (our != null && their != null)
                {
                    our = ObjectId.GetFor(our as UnityObject);
                    their = ObjectId.GetFor(their as UnityObject);
                }
            }

            return !Equals(our, their);
        }
    }
}
