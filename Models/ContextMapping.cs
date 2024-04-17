namespace Spark.PropertyTypes.Models
{
    public class ContextMapping
    {
        public int Id { get; set; }
        public string ContextName { get; set; }
        public int PropertyTypeId { get; set; }
        public int ParentPropertyTypeId { get; set; }
    }
}
