using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository
{
    public interface IServiceRepository
    {
        Task Add(Service service);

        Task<long> Count(Dictionary<string, Tuple<string, object>> filterCriteria);

        Task<int> Count(List<string> list, string componentLevel2);

        Task<IService> Find(string id);

        Task<IEnumerable<ServiceRateSearchResult>> GetAllRates(List<FilterData> filterData);

        Task<List<Service>> GetByGroupAndColumnName(string group, string columnName, int pageNumber = -1, int batchSize = -1);

        Task<List<Service>> GetByGroupAndColumnNameAndKeyWords(string group, string columnName, List<string> keywords,
            int pageNumber = -1, int batchSize = -1);

        Task<long> GetTotalCountByGroupAndColumnName(string group, string columnName);

        Task<long> GetTotalCountByGroupAndColumnNameAndKeyWords(string group, string columnName, List<string> keywords);

        Task<List<Service>> ListComponents(int pageNumber = -1, int batchSize = -1);

        Task<List<Service>> Search(Dictionary<string, Tuple<string, object>> filterCriteria, int pageNumber, int batchSize, string sortColumn = "material Name",
            SortOrder sortOrder = SortOrder.Ascending);

        Task<List<Service>> Search(List<string> searchKeywords, int pageNumber = -1, int batchSize = -1);

        Task<List<Service>> Search(List<string> searchKeywords, string serviceLevel1, int pageNumber = -1, int batchSize = -1, string sortCriteria = "service_code", SortOrder sortOrder = SortOrder.Descending);

        Task<List<string>> SearchInGroup(List<string> keywords);

        Task Update(IService service);
    }
}