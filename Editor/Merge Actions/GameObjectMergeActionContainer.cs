namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityObject = UnityEngine.Object;
    using System.Linq;

    /// <summary>
    /// A container that contains all available <see cref="MergeAction"/>s for one specific GameObject.
    /// </summary>
    internal class GameObjectMergeActionContainer
    {
        public readonly string Name;
        public readonly ReadOnlyCollection<MergeAction> MergeActions;
        public bool HasActions => mergeActions.Count > 0;
        public bool IsCompleted => mergeActions.All(action => action.DecisionState == DecisionState.Complete);
        public bool HasIncompleteActions => mergeActions.Any(action => action.DecisionState == DecisionState.Incomplete);
        public GameObject TargetGameObject => ourGameObject;

        private readonly GameObject ourGameObject;
        private readonly GameObject theirGameObject;
        private readonly List<MergeAction> mergeActions;


        public GameObjectMergeActionContainer(GameObject ourGameObject, GameObject theirGameObject)
        {
            this.ourGameObject = ourGameObject;
            this.theirGameObject = theirGameObject;

            mergeActions = new();
            MergeActions = mergeActions.AsReadOnly();

            if (ourGameObject && !theirGameObject)
            {
                Name = ourGameObject.GetPath();
                mergeActions.Add(new MergeActionOurGameObject(ourGameObject));
            }
            else if (theirGameObject && !ourGameObject)
            {
                Name = theirGameObject.GetPath();
                mergeActions.Add(new MergeActionTheirGameObject(theirGameObject));
            }
            else
            {
                Name = ourGameObject.GetPath();
                FindComponentDifferences();
                FindGameObjectPropertyDifferences();
            }

            // TODO CheckIfMerged
        }

        public void UseOurs()
        {
            foreach (var action in mergeActions)
            {
                action.UseOurs();
            }
        }

        public void UseTheirs()
        {
            foreach (var action in mergeActions)
            {
                action.UseTheirs();
            }
        }

        private void FindComponentDifferences()
        {
            var ourComponents = ourGameObject.GetComponents<Component>();

            var theirComponents = new Dictionary<ObjectId, Component>();
            foreach (var component in theirGameObject.GetComponents<Component>())
            {
                if (component == null) continue;

                var id = ObjectId.GetFor(component);
                theirComponents.Add(ObjectId.GetFor(component), component);
            }

            foreach (var ourComponent in ourComponents)
            {
                if (ourComponent == null) continue;

                var id = ObjectId.GetFor(ourComponent);
                if (theirComponents.TryGetValue(id, out var theirComponent))
                {
                    var id2 = ObjectId.GetFor(theirComponent);
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
                throw new System.ArgumentException($"The two objects must be of the same type, but are {ours.GetType().Name} and {theirs.GetType().Name}.");
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

                    if (DifferentValues(ourProperty, theirProperty))
                    {
                        mergeAction ??= new MergeActionPropertyValues(ours);

                        mergeAction.AddProperty(ourProperty.Copy(), theirProperty.Copy());
                    }

                    shouldEnterChildren = ShouldEnterChildren(ourProperty);
                }
            }
            
            if (mergeAction != null)
            {
                if (ours.GetType() == typeof(Transform))
                {
                    // Position, Rotation and Scale happen to be in the correct order when sorted alphabetically.
                    // And we want to do this because the serialized properties are not in the expected order for some reason.
                    mergeAction.SortPropertiesAlphabetically();
                }

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

            if (ourProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                var our = ourProperty.GetValue();
                var their = theirProperty.GetValue();

                if (our != null && their != null)
                {
                    our = ObjectId.GetFor(our as UnityObject);
                    their = ObjectId.GetFor(their as UnityObject);
                }
                return !Equals(our, their);
            }

            return !SerializedProperty.DataEquals(ourProperty, theirProperty);
        }

        private bool ShouldEnterChildren(SerializedProperty serializedProperty)
        {
            return serializedProperty.propertyType == SerializedPropertyType.Generic &&
                serializedProperty.hasVisibleChildren &&
                serializedProperty.isExpanded;
        }
    }
}
