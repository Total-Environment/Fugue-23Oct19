using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// The App service that abstracts all material related services.
    /// </summary>
    public interface IMaterialService
    {
        /// <summary>
        /// Creates a material based on the data provided.
        /// </summary>
        /// <param name="materialData">Material data to be parsed and created.</param>
        /// <returns>Created Material</returns>
        Task<IMaterial> Create(IMaterial materialData);

        /// <summary>
        /// Searches the within group.
        /// </summary>
        /// <param name="filterData">The filter data.</param>
        /// <param name="group">The group.</param>
        /// <param name="searchKeywords">The search keywords.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        Task<List<IMaterial>> SearchWithinGroup(List<FilterData> filterData, string group, List<string> searchKeywords,
            int pageNumber, int batchSize, string sortColumn = "material Name",
            SortOrder sortOrder = SortOrder.Ascending);

        /// <summary>
        /// Search with keywords and group name
        /// </summary>
        /// <param name="searchKeywords">The search keywords.</param>
        /// <param name="materialLevel2">The group</param>
        /// <param name="pageNumber">The page number</param>
        /// <param name="batchSize">Size of the batch</param>
        /// <param name="sortColumn">The sort column</param>
        /// <param name="sortOrder">The sort order</param>
        /// <returns></returns>
        Task<List<IMaterial>> Search(List<string> searchKeywords, string materialLevel2, int pageNumber,
            int batchSize, string sortColumn, SortOrder sortOrder);

        /// <summary>
        /// Search with keywords and without group name
        /// </summary>
        /// <param name="searchKeywords"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task<List<IMaterial>> Search(List<string> searchKeywords, int pageNumber, int batchSize);

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="searchKeywords"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<long> Count(List<string> searchKeywords, string group);

        /// <summary>
        /// Counts the within group by keyword and filters.
        /// </summary>
        /// <param name="filterData">The filter data.</param>
        /// <param name="group">The group.</param>
        /// <param name="searchKeywords">The search keywords.</param>
        /// <returns></returns>
        Task<long> CountWithinGroup(List<FilterData> filterData, string group, List<string> searchKeywords);

        /// <summary>
        /// Get material with passed material Id.
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        Task<IMaterial> Find(string materialId);

        /// <summary>
        /// Return the total count of the material in the group where column has value.
        /// </summary>
        /// <param name="materialGroup">The material group.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns></returns>
        Task<long> GetCountOfMaterialsHavingAttachmentColumnDataInGroup(string materialGroup,
            string columnName, List<string> keywords = null);

        Task<long> GetCountOfBrandsHavingAttachmentColumnDataInGroup(string materialGroup,
            string columnName, List<string> keywords = null);

        Task<List<Dictionary<string, object>>> GetBrandAttachmentsByGroupAndColumnNameKeywods(string group, string brandColumnKey,
            List<string> keywords,
            int pageNumber,
            int batchSize);

        /// <summary>
        /// Return material in the group where column name has values.
        /// </summary>
        /// <param name="materialGroup">The material group.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="keywords">The keywords.</param>
        /// <returns></returns>
        Task<IList<IMaterial>> GetMaterialHavingAttachmentColumnDataInGroup(string materialGroup,
            string columnName, int pageNumber, int batchSize, List<string> keywords = null);

        /// <summary>
        /// Returns the material controller by pageNumber and batchSize.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task<List<IMaterial>> GetRecentMaterials(int pageNumber, int batchSize);

        /// <summary>
        /// Return the total material count.
        /// </summary>
        /// <returns></returns>
        Task<long> GetMaterialCount();

        /// <summary>
        /// Updates material with passed material Id.
        /// </summary>
        /// <param name="materialData"></param>
        /// <returns></returns>
        Task<IMaterial> Update(IMaterial materialData);

        /// <summary>
        /// Search for keywords in material and return material group which have material having
        /// these keywords.
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        Task<List<string>> SearchForGroups(List<string> keywords);

        /// <summary>
        /// Gets all rates.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MaterialRateSearchResult>> GetAllRates(List<FilterData> filters);
    }
}