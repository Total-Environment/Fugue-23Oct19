using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public class CostPriceRatioFilterFactory : ICostPriceRatioFilterFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public ICostPriceRatioFilter GetCostPriceRatioFilter(ComponentType componentType)
        {
            switch (componentType)
            {
                case ComponentType.Material:
                    return new MaterialCostPriceRatioFilter();

                case ComponentType.Service:
                    return new CostPriceRatioFilter();

                case ComponentType.Package:
                    return new CostPriceRatioFilter();

                case ComponentType.SFG:
                    return new CostPriceRatioFilter();

                case ComponentType.Asset:
                    throw new NotImplementedException(
                        $"{componentType} is not implemented. Try with {ComponentType.Material}.");

                default:
                    throw new NotImplementedException(componentType + " is not implemented.");
            }
        }
    }
}