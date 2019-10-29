using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    public interface IServiceSapSyncer
    {
        Task<bool> Sync(Service service, bool update);
    }
}