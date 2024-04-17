using Google.Protobuf.WellKnownTypes;
using Spark.PropertyTypes.Grpc;

namespace Spark.PropertyTypes.GrpcClients.Services;

public interface IPropertyTypesService
{
    Task<GetPropertyTypesResponse> GetPropertyTypes(CancellationToken cancellationToken = default);
    Task<GetContextPropertyMappingResponse> GetContextPropertyMappings(CancellationToken cancellationToken = default);
}

public class PropertyTypesService(Grpc.PropertyTypesService.PropertyTypesServiceClient propertyTypesServiceClient, ILogger<PropertyTypesService> logger) : IPropertyTypesService
{
    private readonly Grpc.PropertyTypesService.PropertyTypesServiceClient _propertyTypesServiceClient = propertyTypesServiceClient;
    private readonly ILogger<PropertyTypesService> _logger = logger;

    public async Task<GetPropertyTypesResponse> GetPropertyTypes(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _propertyTypesServiceClient.GetPropertyTypesAsync(new Empty(), cancellationToken: cancellationToken);

            _logger.LogDebug("Received response from PropertyTypesService.GetPropertyTypes", new { Response = response });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to get PropertyTypes from PropertyTypesService");

            return new GetPropertyTypesResponse();
        }
    }

    public async Task<GetContextPropertyMappingResponse> GetContextPropertyMappings(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _propertyTypesServiceClient.GetContextPropertyMappingsAsync(new Empty(), cancellationToken: cancellationToken);

            _logger.LogDebug("Received response from PropertyTypesService.GetContextPropertyMappings", new { Response = response });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to get PropertyTypes from PropertyTypesService");

            return new GetContextPropertyMappingResponse();
        }
    }
}
