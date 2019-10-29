using System;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    public class ServiceSapSyncer : IServiceSapSyncer
    {
        private readonly IServiceAndCompositeComponentSapSyncer _serviceAndCompositeComponentSapSyncer;

        public ServiceSapSyncer(IServiceAndCompositeComponentSapSyncer serviceAndCompositeComponentSapSyncer)
        {
            _serviceAndCompositeComponentSapSyncer = serviceAndCompositeComponentSapSyncer;
        }

        public async Task<bool> Sync(Service service, bool update)
        {
            var general = service.Header("General");
            var purchase = service.Header("Purchase");
            var shortDescription = Convert.ToString(general?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "short description",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var baseUnitOfMeasure = Convert.ToString(general?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "unit of measure",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var sacCode = Convert.ToString(general?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "SAC",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var gstApplicability = Convert.ToString(purchase?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "GST Applicability",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var request = new ServiceAndCompositeComponentRequest
            {
                Code = service.Id,
                ComponentType = "service",
                ShortDescription = shortDescription,
                SACCode = sacCode,
                UnitOfMeasure = baseUnitOfMeasure,
                GSTApplicability = gstApplicability,
                CreatedAt = service.CreatedAt,
                Update = update
            };
            return await _serviceAndCompositeComponentSapSyncer.Sync(request);
        }
    }
}