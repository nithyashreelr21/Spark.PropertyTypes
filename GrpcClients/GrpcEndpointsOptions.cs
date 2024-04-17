using System.ComponentModel.DataAnnotations;

namespace Spark.Properties.Infrastructure.GrpcClients;

public class GrpcEndpointsOptions
{
    public const string SectionName = "GrpcEndpoints";

    [Url]
    [Required]
    public string PropertiesService { get; set; } = null!;
}
