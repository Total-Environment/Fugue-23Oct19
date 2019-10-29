using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    public interface ICompositeComponentSapSyncer
    {
        void Sync(CompositeComponent compositeComponent, bool update, string componentType);
    }
}