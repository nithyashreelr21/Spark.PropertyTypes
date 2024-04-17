namespace Spark.PropertyTypes.Models
{
    public class PropertyTypeNode(int propertyTypeId)
    {
        public int PropertyTypeId { get; set; } = propertyTypeId;
        public List<PropertyTypeNode> Children { get; set; } = [];
    }

}
