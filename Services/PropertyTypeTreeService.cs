using Spark.PropertyTypes.Models;

namespace Spark.PropertyTypes.Services
{
    public class PropertyTypeTreeService(PropertyTypeContextService propertyTypeContextService, PropertyTypeNode rootNode)
    {
        private readonly PropertyTypeContextService _propertyTypeContextService = propertyTypeContextService;
        private PropertyTypeNode _rootNode = rootNode;

        public List<int> GetAllChildPropertyTypeIds(int propertyTypeId)
        {
            List<int> childIds = new();

            PropertyTypeNode node = FindPropertyTypeNodeById(propertyTypeId, _rootNode);

            if (node != null)
            {
                TraverseChildNodes(node, childIds);
            }

            return childIds;
        }

        private static void TraverseChildNodes(PropertyTypeNode node, List<int> childIds)
        {
            childIds.Add(node.PropertyTypeId);
            foreach (var childNode in node.Children)
            {
                TraverseChildNodes(childNode, childIds);
            }
        }

        public List<PropertyType> GetChildTypes(PropertyType type)
        {
            List<PropertyType> childTypes = new();

            PropertyTypeNode node = FindPropertyTypeNodeById(type.PropertyTypeId, _rootNode);
            if (node != null)
            {
                TraverseChildren(node, childTypes);
            }

            return childTypes;
        }

        private void TraverseChildren(PropertyTypeNode node, List<PropertyType> childTypes)
        {
            foreach (var childNode in node.Children)
            {
                childTypes.Add(new PropertyType { PropertyTypeId = childNode.PropertyTypeId });
                TraverseChildren(childNode, childTypes);
            }
        }

        private static PropertyTypeNode FindPropertyTypeNodeById(int propertyTypeId, PropertyTypeNode currentNode)
        {
            if (currentNode == null)
            {
                return null;
            }

            if (currentNode.PropertyTypeId == propertyTypeId)
            {
                return currentNode;
            }

            foreach (var childNode in currentNode.Children)
            {
                var foundNode = FindPropertyTypeNodeById(propertyTypeId, childNode);
                if (foundNode != null)
                {
                    return foundNode; 
                }
            }

            return null; // Node not found
        }

        public PropertyType GetSubsetEntryFor(PropertyType forThis, string subsetName)
        {
            // Find the property type node corresponding to the provided property type
            PropertyTypeNode node = FindPropertyTypeNodeById(forThis.PropertyTypeId, _rootNode);

            // Traverse up the tree until we find a property type explicitly mentioned in the subset
            while (node != null)
            {
                // Check if the property type node belongs to the subset
                if (_propertyTypeContextService.IsInSubset(new PropertyType { PropertyTypeId = node.PropertyTypeId }, subsetName))
                {
                    // If the property type node belongs to the subset, return it
                    return new PropertyType
                    {
                        PropertyTypeId = node.PropertyTypeId
                    };
                }

                // Move up to the parent node
                node = FindParentPropertyTypeNode(node);
            }

            // If no property type explicitly mentioned in the subset is found in the tree, return null
            return null;
        }

        // Helper method to find the parent property type node
        private PropertyTypeNode FindParentPropertyTypeNode(PropertyTypeNode node)
        {
            // Iterate through all nodes in the tree to find the parent node of the given node
            foreach (var parentNode in GetAllNodes(_rootNode))
            {
                if (parentNode.Children.Contains(node))
                {
                    return parentNode;
                }
            }

            return null; // If parent node is not found
        }

        // Helper method to get all nodes in the tree
        private IEnumerable<PropertyTypeNode> GetAllNodes(PropertyTypeNode rootNode)
        {
            // Use breadth-first search to traverse all nodes in the tree
            var queue = new Queue<PropertyTypeNode>();
            queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                yield return currentNode;

                foreach (var childNode in currentNode.Children)
                {
                    queue.Enqueue(childNode);
                }
            }
        }
    }

}
