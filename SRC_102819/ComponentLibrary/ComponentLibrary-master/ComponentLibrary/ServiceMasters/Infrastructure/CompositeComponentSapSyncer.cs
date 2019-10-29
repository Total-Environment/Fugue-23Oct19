using System;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    public class CompositeComponentSapSyncer : ICompositeComponentSapSyncer
    {
        private readonly IServiceAndCompositeComponentSapSyncer _serviceAndCompositeComponentSapSyncer;

        public CompositeComponentSapSyncer(IServiceAndCompositeComponentSapSyncer serviceAndCompositeComponentSapSyncer)
        {
            _serviceAndCompositeComponentSapSyncer = serviceAndCompositeComponentSapSyncer;
        }

        public void Sync(CompositeComponent compositeComponent, bool update, string componentType)
        {
            var general = compositeComponent.Headers.FirstOrDefault(h => h.Key == "general");
            var purchase = compositeComponent.Headers.FirstOrDefault(h => h.Key == "purchase");
            var systemLogs = compositeComponent.Headers.FirstOrDefault(h => h.Key == "system_logs");
            var shortDescription = Convert.ToString(general?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "short description",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var baseUnitOfMeasure = Convert.ToString(general?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "unit of measure",
                    StringComparison.CurrentCultureIgnoreCase))?.Value).ToUpper();
            var sacCode = Convert.ToString(general?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "SAC",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var gstApplicability = Convert.ToString(purchase?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "GST Applicability",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);
            var createdAt = Convert.ToDateTime(systemLogs?.Columns
                .FirstOrDefault(c => c.Key == "date_created")?.Value);
            var request = new ServiceAndCompositeComponentRequest
            {
                Code = compositeComponent.Code,
                ComponentType = componentType,
                ShortDescription = shortDescription,
                SACCode = sacCode,
                UnitOfMeasure = baseUnitOfMeasure,
                GSTApplicability = gstApplicability,
                CreatedAt = createdAt,
                Update = update
            };
            _serviceAndCompositeComponentSapSyncer.Sync(request);
        }
    }
}