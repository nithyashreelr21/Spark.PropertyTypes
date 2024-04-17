using Microsoft.Extensions.Options;
using Spark.Properties.Infrastructure.GrpcClients;
using Spark.PropertyTypes.GrpcClients.Services;
using PropertyTypesService = Spark.PropertyTypes.Grpc.PropertyTypesService;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddOptions<GrpcEndpointsOptions>()
           .BindConfiguration(GrpcEndpointsOptions.SectionName)
           .ValidateDataAnnotations()
           .ValidateOnStart();

builder.Services.AddGrpcClient<PropertyTypesService.PropertyTypesServiceClient>((sp, o) =>
{
    var propertiesServiceAddress = sp.GetRequiredService<IOptions<GrpcEndpointsOptions>>().Value.PropertiesService;
    o.Address = new Uri(propertiesServiceAddress);
});

builder.Services.AddScoped<IPropertyTypesService, Spark.PropertyTypes.GrpcClients.Services.PropertyTypesService>();

var app = builder.Build();

app.Run();
