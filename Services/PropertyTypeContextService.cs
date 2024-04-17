using Spark.PropertyTypes.GrpcClients.Services;
using Spark.PropertyTypes.Models;

namespace Spark.PropertyTypes.Services
{
    public class PropertyTypeContextService(IPropertyTypesService propertyTypesService)
    {
        private List<PropertyType> _propertyTypes;
        private List<ContextMapping> _contextMappings;
        private readonly IPropertyTypesService _propertyTypesService = propertyTypesService;

        public async Task<IReadOnlyList<PropertyType>> GetAllPropertyTypes(CancellationToken cancellationToken)
        {
            await LoadPropertyTypesAsync(cancellationToken);
            return _propertyTypes?.AsReadOnly();
        }

        public async Task<IReadOnlyList<ContextMapping>> GetAllContextMappings(CancellationToken cancellationToken)
        {
            await LoadContextMappingsAsync(cancellationToken);
            return _contextMappings?.AsReadOnly();
        }

        private async Task LoadPropertyTypesAsync(CancellationToken cancellationToken)
        {
            if (_propertyTypes != null)
            {
                return;
            }

            var response = await _propertyTypesService.GetPropertyTypes(cancellationToken);
            _propertyTypes = response.Mappings
                .Select(m => new PropertyType
                {
                    PropertyTypeId = m.PropertyTypeId,
                    Name = m.Name,
                    ParentPropertyTypeId = m.ParentPropertyTypeId
                })
                .ToList();

            BuildPropertyTypeTree();
        }

        private async Task LoadContextMappingsAsync(CancellationToken cancellationToken)
        {
            if (_contextMappings != null)
            {
                return;
            }

            var response = await _propertyTypesService.GetContextPropertyMappings(cancellationToken);
            _contextMappings = response.ContextMappings
                .Select(m => new ContextMapping
                {
                    Id = m.PropertyTypeId,
                    ContextName = m.ContextName,
                    PropertyTypeId = m.PropertyTypeId,
                    ParentPropertyTypeId = m.ParentPropertyTypeId
                })
                .ToList();
        }

        private void BuildPropertyTypeTree()
        {
            var idToPropertyTypeMap = _propertyTypes.ToDictionary(pt => pt.PropertyTypeId);

            foreach (var propertyType in _propertyTypes)
            {
                if (propertyType.ParentPropertyTypeId != 0 && idToPropertyTypeMap.TryGetValue(propertyType.ParentPropertyTypeId, out PropertyType? value))
                {
                    value.Children.Add(propertyType);
                }
            }
        }

        public async Task<List<PropertyType>> GetPropertyTypesForContext(string contextName, CancellationToken cancellationToken)
        {
            await LoadPropertyTypesAsync(cancellationToken);
            return _propertyTypes.Where(pt => IsInSubset(pt, contextName)).ToList();
        }

        public async Task<List<int>> GetPropertyTypeIdsForContext(string contextName, CancellationToken cancellationToken)
        {
            await LoadPropertyTypesAsync(cancellationToken);
            return _propertyTypes
                .Where(pt => IsInSubset(pt, contextName))
                .Select(pt => pt.PropertyTypeId)
                .ToList();
        }

        // This checks whether it Belongs To Context
        public bool IsInSubset(PropertyType checkThis, string subsetName)
        {
            return _contextMappings.Any(mapping => mapping.PropertyTypeId == checkThis.PropertyTypeId &&
                                                   mapping.ContextName == subsetName);
        }
    }
}
