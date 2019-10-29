using System;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ICostPriceRatioBuilderFactory"/>
	public class CostPriceRatioBuilderFactory : ICostPriceRatioBuilderFactory
	{
		private readonly IMaterialRepository _materialRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly ICompositeComponentRepository _compositeComponentRepository;
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _compositeComponentDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioBuilderFactory"/> class.
		/// </summary>
		/// <param name="materialRepository">The material repository.</param>
		/// <param name="serviceRepository">The service repository.</param>
		/// <param name="compositeComponentRepository">The composite component repository.</param>
		/// <param name="compositeComponentDefinitionRepository">
		/// The composite component definition repository.
		/// </param>
		public CostPriceRatioBuilderFactory(IMaterialRepository materialRepository, IServiceRepository serviceRepository,
			ICompositeComponentRepository compositeComponentRepository,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> compositeComponentDefinitionRepository)
		{
			_materialRepository = materialRepository;
			_serviceRepository = serviceRepository;
			_compositeComponentRepository = compositeComponentRepository;
			_compositeComponentDefinitionRepository = compositeComponentDefinitionRepository;
		}

		/// <summary>
		/// Gets the cost price ratio Builder.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns></returns>
		public ICostPriceRatioBuilder GetCostPriceRatioBuilder(ComponentType componentType)
		{
			switch (componentType)
			{
				case ComponentType.Material:
					return new MaterialCostPriceRatioBuilder(_materialRepository);

				case ComponentType.Service:
					return new ServiceCostPriceRatioBuilder(_serviceRepository);

				case ComponentType.Asset:
					throw new NotImplementedException($"{componentType} is not implemented. Try with {ComponentType.Material}.");

				case ComponentType.SFG:
					return new SemiFinishedGoodCostPriceRatioBuilder(_compositeComponentRepository, _compositeComponentDefinitionRepository);

				case ComponentType.Package:
					return new PackageCostPriceRatioBuilder(_compositeComponentRepository, _compositeComponentDefinitionRepository);

				default:
					throw new NotImplementedException(componentType + " is not implemented.");
			}
		}
	}
}