using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class CostPriceListDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CostPriceListDto()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costPriceRatioList"></param>
        public CostPriceListDto(CostPriceRatioList costPriceRatioList)
        {
            costPriceRatioDtos = new List<CostPriceRatioDto>();
            foreach (var costPriceRatio in costPriceRatioList.costPriceRatios)
            {
                costPriceRatioDtos.Add(new CostPriceRatioDto(costPriceRatio));
            }
        }

        public List<CostPriceRatioDto> costPriceRatioDtos { get; set; }
    }
}