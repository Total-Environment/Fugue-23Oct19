using System;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICostPriceRatioFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="costPriceRatioList"></param>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        CostPriceRatioList Filter(CostPriceRatioList costPriceRatioList, string projectCode);
    }
}