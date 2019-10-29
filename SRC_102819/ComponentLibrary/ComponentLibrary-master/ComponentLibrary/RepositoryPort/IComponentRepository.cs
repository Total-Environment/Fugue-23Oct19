using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComponentRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="searchQuery"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        Task<object> FindReplacements(ComponentType type, JObject searchQuery, bool merge = false);

        /// <summary>
        /// Update EDesign Specification rfa details.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="rfaLink"></param>
        /// <param name="jsonLink"></param>
        /// <param name="revitFamilyType"></param>
        Task<bool> UpdateRfaDetails(ComponentType type, string code, string rfaLink, string jsonLink, string revitFamilyType);
    }
}
