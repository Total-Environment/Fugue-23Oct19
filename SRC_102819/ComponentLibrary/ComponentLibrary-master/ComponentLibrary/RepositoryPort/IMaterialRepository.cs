using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    public interface IMaterialRepository
    {
        Task Add(IMaterial material);

        Task<long> Count(Dictionary<string, Tuple<string, object>> filterCriteria);

        Task<int> Count(List<string> list, string componentLevel2);

        Task<IMaterial> Find(string id);

        Task<IEnumerable<MaterialRateSearchResult>> GetAllRates(List<FilterData> filterDatas);

        Task<List<Dictionary<string, object>>> GetBrandAttachmentsByGroupAndColumnNameKeywods(string group, string brandColumnName, ISimpleColumnDefinition columnDataType, List<string> keywords, int pageNumber = -1, int batchSize = -1);

        Task<List<IMaterial>> GetByGroupAndColumnName(string group, string columnName, int pageNumber = -1, int batchSize = -1);

        Task<List<IMaterial>> GetByGroupAndColumnNameAndKeyWords(string group, string columnName, List<string> keywords, int pageNumber = -1, int batchSize = -1);

        Task<long> GetTotalBrandCountByGroupAndColumnNameAndKeywords(string group, string columnName, List<string> keywords);

        Task<long> GetTotalCountByGroupAndColumnName(string group, string columnName);

        Task<long> GetTotalCountByGroupAndColumnNameAndKeyWords(string group, string columnName, List<string> keywords);

        Task<List<IMaterial>> ListComponents(int pageNumber = 1, int batchSize = -1);

        Task<List<IMaterial>> Search(Dictionary<string, Tuple<string, object>> filterCriteria, int pageNumber = -1, int batchSize = -1, string sortColumn = "material_name", SortOrder sortOrder = SortOrder.Ascending);

        Task<List<IMaterial>> Search(List<string> searchKeywords, int pageNumber = -1, int batchSize = -1);

        Task<List<IMaterial>> Search(List<string> searchKeywords, string materialLevel2, int pageNumber = -1, int batchSize = -1, string sortCriteria = "material_name", SortOrder sortOrder = SortOrder.Descending);

        Task<List<string>> SearchInGroup(List<string> keywords);

        Task Update(IMaterial material);
    }
}