using System;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ICostPriceRatioValidatorFactory"/>
	public class CostPriceRatioValidatorFactory : ICostPriceRatioValidatorFactory
	{
		private readonly IMaterialRepository _materialRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly ICompositeComponentRepository _compositeComponentRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioValidatorFactory"/> class.
		/// </summary>
		/// <param name="materialRepository">The material repository.</param>
		/// <param name="serviceRepository">The service repository.</param>
		/// <param name="compositeComponentRepository">The composite component repository.</param>
		/// <param name="projectRepository">The project repository.</param>
		/// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
		public CostPriceRatioValidatorFactory(IMaterialRepository materialRepository, IServiceRepository serviceRepository,
			ICompositeComponentRepository compositeComponentRepository, IProjectRepository projectRepository,
			IDependencyDefinitionRepository dependencyDefinitionRepository)
		{
			_materialRepository = materialRepository;
			_serviceRepository = serviceRepository;
			_compositeComponentRepository = compositeComponentRepository;
			_projectRepository = projectRepository;
			_dependencyDefinitionRepository = dependencyDefinitionRepository;
		}

		/// <summary>
		/// Gets the cost price ratio validator.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns></returns>
		public ICostPriceRatioValidator GetCostPriceRatioValidator(ComponentType componentType)
		{
			switch (componentType)
			{
				case ComponentType.Material:
					return new MaterialCostPriceRatioValidator(_materialRepository, _projectRepository,
						_dependencyDefinitionRepository);

				case ComponentType.Service:
					return new ServiceCostPriceRatioValidator(_serviceRepository, _projectRepository,
						_dependencyDefinitionRepository);

				case ComponentType.Asset:
					throw new NotImplementedException($"{componentType} is not implemented. Try with {ComponentType.Material}.");

				case ComponentType.SFG:
					return new SemiFinishedGoodCostPriceRatioValidator(_compositeComponentRepository, _projectRepository,
						_dependencyDefinitionRepository);

				case ComponentType.Package:
					return new PackageCostPriceRatioValidator(_compositeComponentRepository, _projectRepository,
						_dependencyDefinitionRepository);

				default:
					throw new NotImplementedException(componentType + " is not implemented.");
			}
		}
	}
}