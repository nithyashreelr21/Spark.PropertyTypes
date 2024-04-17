namespace Spark.PropertyTypes.Models
{
    public class PropertyType
    {
        public int PropertyTypeId { get; set; }
        public string Name { get; set; }
        public int ParentPropertyTypeId { get; set; }
        public List<PropertyType> Children { get; set; } = [];
    }
}
