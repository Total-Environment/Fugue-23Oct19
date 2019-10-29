using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class CostPriceRatioList
    {

        /// <summary>
        /// 
        /// </summary>
        public CostPriceRatioList()
        {
            costPriceRatios = new List<CostPriceRatio>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costPriceRatioList"></param>
        public CostPriceRatioList(List<CostPriceRatio> costPriceRatioList)
        {
            costPriceRatios = costPriceRatioList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="costPriceRatioDaos"></param>
        /// <param name="costPriceRatioDefinition"></param>
        public CostPriceRatioList(List<CostPriceRatioDao> costPriceRatioDaos, CostPriceRatioDefinition costPriceRatioDefinition)
        {
            costPriceRatios = new List<CostPriceRatio>();
            foreach (var costPriceRatioDao in costPriceRatioDaos)
            {
                costPriceRatios.Add(costPriceRatioDao.ToCostPriceRatio(costPriceRatioDefinition));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<CostPriceRatio> costPriceRatios;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="costPriceRatio"></param>
        public void Add(CostPriceRatio costPriceRatio)
        {
            costPriceRatios.Add(costPriceRatio);
        }
    }
}